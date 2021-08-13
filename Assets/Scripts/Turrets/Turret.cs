using Enemies;
using Field;
using JetBrains.Annotations;
using Turrets.Projectile;
using UnityEngine;


namespace Turrets
{
    public class Turret : MonoBehaviour
    {
    
        private Transform target;
        // sehr viel perfomanter weil wir in der update sonst immer getComponent bräuchten
        protected EnemyMovement targetEnemy;
    
        [Header("Attributes")] public float range = 1f;
        public float fireRate = 1f;
        public float turnSpeed = 10f;
        private float fireCountdown = 0f;

        public Ressource.RessourceType ressourceType;
        public TurretType turretType;


        // only laser
        public LineRenderer lineRenderer;


        [Header("Unity Setup Fields")] public Transform partToRotate;
        public Transform firePoint;
        public string enemyTag = "Enemy";


        public bool canRotate;
        
        public GameObject projectilePrefab;
        
    
        // MultiTargetTurret
        public Transform[] targets;
        public int maxTargets;
        public Transform[] firePoints;
        private float[] fireCountdowns;

        // Start is called before the first frame update
        void Start()
        {
            if(turretType == TurretType.MULTI)
                InvokeRepeating(nameof(UpdateTargets), 0f, 0.25f);
            else
                InvokeRepeating(nameof(UpdateTarget), 0f, 0.5f);
            

            if (turretType == TurretType.MULTI)
            {
                targets = new Transform[maxTargets];
                fireCountdowns = new float[maxTargets];
            }
                

            //Range auf Hexradius anpassen
            range *= HexMetrics.outerRadius;
        }

        /**
 * Methode wird 2x pro Sekunde aufgerufen und ermittelt den am wenigsten entfernten Gegner
 * und speichert dessen Transform in der KLassenvariablen target
 */
        protected virtual void UpdateTarget()
        {
            
            // sollte ersetzt werden weil es bestimmt perfomance spart aber wir haben noch keine spieler klasse und wissen deshalb nicht welchen enemy spawn wir brauchen
            // Außerdem verhindert es das wir die enemys der gegner abschießen wenn sie in range sind.
            // HGameManager.Instance.enemySpawns[1].enemys
            GameObject[] enemies = GameObject.FindGameObjectsWithTag(enemyTag);
            float shortestDistance = Mathf.Infinity;
            GameObject nearestEnemy = null;

            foreach (GameObject enemy in enemies)
            {
                float distanceToEnemy = Vector3.Distance(transform.position, enemy.transform.position);
                if (distanceToEnemy < shortestDistance )
                {
                    // sollte der nearest enemy wirklich geändert werden solange das target noch in range ist ?
                    shortestDistance = distanceToEnemy;
                    nearestEnemy = enemy;
                    targetEnemy = enemy.GetComponent<EnemyMovement>();
                }
            }

            if (nearestEnemy != null && shortestDistance <= range)
            {
                target = nearestEnemy.transform;
            }
            else
            {
                target = null;
            }
        }

        void UpdateTargets()
        {
            for (int i = 0; i < targets.Length; i++)
            {
                UpdateTarget(i);
            }
            
        }

        bool TargetIsTargeted(int index,Transform t)
        {
            
            for (int i = 0; i < targets.Length; i++)
            {
                // Distance ist kacke weil zwei verschiedene gegner  ruhig aufeinander stehen dürfen und trotzdem beide gefocused werden sollten
                // wenn die enemyspawns mit spieler zuweis impl. werden könnte man min enemy index arbeiten
                if (targets[i] != null && Vector3.Distance(targets[i].position ,t.position) < 0.2f && i != index)
                {
                    
                    return true;
                }
            }

            return false;
        }
        protected virtual void UpdateTarget(int index)
        {
            
            // sollte ersetzt werden weil es bestimmt perfomance spart aber wir haben noch keine spieler klasse und wiesen deshalb nicht welchen enemy spawn wir brauchen
            // Außerdem verhindert es das wir die enemys der gegner abschießen wenn sie in range sind.
            // HGameManager.Instance.enemySpawns[1].enemys
            GameObject[] enemies = GameObject.FindGameObjectsWithTag(enemyTag);
            float shortestDistance = Mathf.Infinity;
            GameObject nearestEnemy = null;
            foreach (GameObject enemy in enemies)
            {
                float distanceToEnemy = Vector3.Distance(transform.position, enemy.transform.position);
                if (distanceToEnemy < shortestDistance && !TargetIsTargeted(index,enemy.transform))
                {    
                    shortestDistance = distanceToEnemy;
                    nearestEnemy = enemy;
                    targetEnemy = enemy.GetComponent<EnemyMovement>();
                    /*
                    if (index == 0)
                    {
                        shortestDistance = distanceToEnemy;
                        nearestEnemy = enemy;
                        targetEnemy = enemy.GetComponent<EnemyMovement>();
                               //enemy.transform.position != targets[index - 1].transform.position                                                                     
                    } else if (targets[index - 1] != null && TargetIsTargeted())
                    {
                        shortestDistance = distanceToEnemy;
                        nearestEnemy = enemy;
                        targetEnemy = enemy.GetComponent<EnemyMovement>();
                    }
                    */
                    // sollte der nearest enemy wirklich geändert werden solange das target noch in range ist ?
                    
                }
            }
            
            if (nearestEnemy != null && shortestDistance <= range)
            {
                targets[index] = nearestEnemy.transform;
            }
            else
            {
                targets[index] = null;
            }
        }

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.cyan;
            Gizmos.DrawWireSphere(transform.position, range);
        }

        void Update()
        {
            //Wenn kein Target verhanden keine Updates
            if (target == null && turretType != TurretType.MULTI)
            {
                if (turretType == TurretType.LASER)
                {
                    lineRenderer.enabled = false;
                }
                return;
            }
            checkType();
            

            // soll mit if statement ausgemacht werden können für statische türme
            if (canRotate)
            {
                rotate();
                
            }
                
            
        
            
        
        }
        // nur zum testen
        void ShootProjectile()
        {
       
            GameObject projectileGameObject = Instantiate(projectilePrefab, firePoint.position, firePoint.rotation);
            Projectile.Projectile projectile = projectileGameObject.GetComponent<Projectile.Projectile>();
    
            
            if (projectile != null)
            {
                Debug.Log("projectile nicht null");
                projectile.Seek(target);
            }
            else
            {
                Debug.Log("null projectile");
            }
        }
    
    
        //void checkType(TurretEffects turretEffects)
        void checkType()
        {
            if (turretType == TurretType.PROJECTILE)
            {
                if (fireCountdown <= 0f)
                {
                    ShootProjectile();
                    fireCountdown = 1f / fireRate;
                }

                fireCountdown -= Time.deltaTime;
            }

            if (turretType == TurretType.LASER)
            {
                ShootLaser();
            }

            if (turretType == TurretType.MULTI)
            {
                for (int i = 0; i < firePoints.Length; i++){

                    if (fireCountdowns[i] <= 0f)
                    {
                        if (targets[i] != null)
                        {
                            ShootMulti(i);
                            fireCountdowns[i] = 1f / fireRate;
                        }
                            
                    }
                    fireCountdowns[i] -= Time.deltaTime;

                }
            }
        
        }

        private void ShootMulti(int index)
        {
            GameObject projectileGameObject = (GameObject) Instantiate(projectilePrefab, firePoints[index].position, firePoints[index].rotation);
            Projectile.Projectile projectile = projectileGameObject.GetComponent<Projectile.Projectile>();
            
            if (projectile != null)
            {
                projectile.Seek(targets[index]);
            }
        }

        private void ShootLaser()
        {
            lineRenderer.enabled = true;
            lineRenderer.SetPosition(0,firePoint.position);
            lineRenderer.SetPosition(1,target.position);
            gameObject.GetComponent<Laser>().Seek(target);
        }

        public void rotate()
        {
            //Point-Direction Vektor ermitteln
            Vector3 direction = target.position - transform.position;
            Quaternion lookRotation = Quaternion.LookRotation(direction);
            //Berechnung der Rotation mit Smooth Lerp
            Vector3 rotation = Quaternion.Lerp(partToRotate.rotation, lookRotation, Time.deltaTime * turnSpeed).eulerAngles;
            partToRotate.rotation = Quaternion.Euler(0f, rotation.y, 0f);
        }
    }
    public enum TurretType
    {
        // ersetzen durch SHOOT,CAST,PERMSHOOT
        PROJECTILE,
        LASER,
        MULTI
    }
}