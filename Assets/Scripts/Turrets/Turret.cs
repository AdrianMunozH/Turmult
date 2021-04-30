using System.Collections.Generic;
using Enemies;
using Field;
using JetBrains.Annotations;
using Turrets.Projectile;
using UnityEngine;
using UnityEngine.Rendering.Universal;

namespace Turrets
{
    public class Turret : MonoBehaviour
    {
    
        private Transform target;
        // sehr viel perfomanter weil wir in der update sonst immer getComponent bräuchten
        private EnemyMovement targetEnemy;
    
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
        [CanBeNull] public TurretScriptableObject turret;
    

        // Start is called before the first frame update
        void Start()
        {
            InvokeRepeating(nameof(UpdateTarget), 0f, 0.5f);
            
            if (turret != null)
            {
                range = turret.range;
                fireRate = turret.fireRate;
                turnSpeed = turret.turnSpeed;
                ressourceType = turret.ressourceType;
                turretType = turret.turretType;
                enemyTag = turret.enemyTag;
                canRotate = turret.canRotate;
            }

            //Range auf Hexradius anpassen
            range *= HexMetrics.outerRadius;
        }

        /**
 * Methode wird 2x pro Sekunde aufgerufen und ermittelt den am wenigsten entfernten Gegner
 * und speichert dessen Transform in der KLassenvariablen target
 */
        void UpdateTarget()
        {
            GameObject[] enemies = GameObject.FindGameObjectsWithTag(enemyTag);
            float shortestDistance = Mathf.Infinity;
            GameObject nearestEnemy = null;

            foreach (GameObject enemy in enemies)
            {
                float distanceToEnemy = Vector3.Distance(transform.position, enemy.transform.position);
                if (distanceToEnemy < shortestDistance)
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

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.cyan;
            Gizmos.DrawWireSphere(transform.position, range);
        }

        void Update()
        {
            //Wenn kein Target verhanden keine Updates
            if (target == null)
            {
                if (turretType == TurretType.LASER)
                {
                    lineRenderer.enabled = false;
                }
                return;
            }

            // soll mit if statement ausgemacht werden können für statische türme
            if(canRotate)
                rotate();
        
            checkType();
        
        }
        // nur zum testen
        void ShootProjectile()
        {
       
            GameObject projectileGameObject = (GameObject) Instantiate(projectilePrefab, firePoint.position, firePoint.rotation);
            Projectile.Projectile projectile = projectilePrefab.GetComponent<Projectile.Projectile>();

            if (projectile != null)
            {
                projectile.Seek(target);
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
        SPELL
    }
}