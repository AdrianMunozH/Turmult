using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Singleplayer.Enemies;
using Singleplayer.Field;
using JetBrains.Annotations;
using Singleplayer.Turrets.Projectile;
using UnityEditor.Build.Content;
using UnityEditor.UIElements;
using UnityEngine;


namespace Singleplayer.Enemies
{
    public class EnemyAttack : MonoBehaviour
    {
    
        private Transform target;
        // sehr viel perfomanter weil wir in der update sonst immer getComponent bräuchten
        protected EnemyMovement targetEnemy;
    
        [Header("Attributes")] public float range = 1f;
        public float fireRate = 1f;

        private float fireCountdown = 0f;


        
        public TurretType turretType;


        // only laser
        public LineRenderer lineRenderer;
        
        public Transform firePoint;
        
        public string enemyTag = "Turret";
        
        // hier sind der dmg und die effecte drin
        public GameObject projectilePrefab;
        

        // Start is called before the first frame update
        void Start()
        {
            
            InvokeRepeating(nameof(UpdateTarget), 0f, 0.5f);
            

            
            //Range auf Hexradius anpassen
            range *= HexMetrics.outerRadius;
        }

        /**
 * Methode wird 2x pro Sekunde aufgerufen und ermittelt den am wenigsten entfernten Gegner
 * und speichert dessen Transform in der KLassenvariablen target
 */
        protected virtual void UpdateTarget()
        {
            if (GetComponent<EnemyMovement>().isAttacking == false) return;
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

                
            
        
            
        
        }
        // nur zum testen
        void ShootProjectile()
        {
       
            GameObject projectileGameObject = Instantiate(projectilePrefab, firePoint.position, firePoint.rotation);
            Projectile projectile = projectileGameObject.GetComponent<Projectile>();
    
            
            if (projectile != null)
            {
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

            
        
        }



        private void ShootLaser()
        {
            lineRenderer.enabled = true;
            lineRenderer.SetPosition(0,firePoint.position);
            lineRenderer.SetPosition(1,target.position);
            gameObject.GetComponent<Laser>().Seek(target);
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