using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class Turret : MonoBehaviour
{
    
    private Transform target;
    // sehr viel perfomanter weil wir in der update sonst immer getComponent bräuchten
    private EnemyMovement targetEnemy;

    [Header("Attributes")] public float range = 15f;
    public float fireRate = 1f;
    private float fireCountdown = 0f;
    public float turnSpeed = 10f;
    // sollte erweitert werden 
    public List<TurretType> turretTypes;
    //public TurretType turretType;
    public float damage = 15f;
    public float damageOverTime = 15f;
    public float slow = 0.2f;

    [Header("Unity Setup Fields")] public Transform partToRotate;
    public string enemyTag = "Enemy";
    public GameObject bulletPrefab;
    public Transform firePoint;
    public GameObject aoePrefab;
    public GameObject dotPrefab;
    
    //test 
    public GameObject projectilePrefab;
    
    // test
    //public TurretEffects[] turretEffects = new  TurretEffects[5];

    // Start is called before the first frame update
    void Start()
    {
        InvokeRepeating(nameof(UpdateTarget), 0f, 0.5f);
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
            return;
        }

        // soll mit if statement ausgemacht werden können für statische türme
        rotate();
        
        /* doesnt work yet
        for (int i = 0; i < turretEffects.Length || turretEffects[i] != null; i++)
        {
           checkType(turretEffects[i]);
        }
        */
        //checkType();
        if (fireCountdown <= 0f)
        {
            ShootProjectile();
            fireCountdown = 1f / fireRate;
        }

        fireCountdown -= Time.deltaTime;
        
        
    }
    // nur zum testen
    void ShootProjectile()
    {
       
        GameObject projectileGameObject = (GameObject) Instantiate(projectilePrefab, firePoint.position, firePoint.rotation);
        Projectile projectile = projectilePrefab.GetComponent<Projectile>();
        projectile.damage = damage;

        if (projectile != null)
        {
            projectile.Seek(target);
        }
    }

    void SlowEnemy()
    {
        targetEnemy.Slow(slow);
    }

    void Shoot()
    {
       
        GameObject bulletGameObject = (GameObject) Instantiate(bulletPrefab, firePoint.position, firePoint.rotation);
        Bullet bullet = bulletGameObject.GetComponent<Bullet>();
        bullet.damage = damage;

        if (bullet != null)
        {
            bullet.Seek(target);
        }
    }


    void ShootAoe()
    {
       
        GameObject aoeGameObject = (GameObject) Instantiate(aoePrefab, firePoint.position, firePoint.rotation);
        AoeProjectile aoe = aoeGameObject.GetComponent<AoeProjectile>();
        aoe.damage = damage;
        if (aoe != null)
        {
            aoe.Seek(target);
        }
    }
    //void checkType(TurretEffects turretEffects)
    void checkType()
    {
        foreach (var turretType in turretTypes)
        {
            if (turretType == TurretType.DAMAGE)
            {
                if (fireCountdown <= 0f)
                {
                    Shoot();
                    fireCountdown = 1f / fireRate;
                }

                fireCountdown -= Time.deltaTime;
            }

            if (turretType == TurretType.DAMAGEOT)
            {
                DamageOverTime();
                //(DamageOvertime) turretEffects.DamageOverTime();
            }

            if (turretType == TurretType.SLOW)
            {
                SlowEnemy();
            }

            if (turretType == TurretType.AOE)
            {
                if (fireCountdown <= 0f)
                {
                    ShootAoe();
                    fireCountdown = 1f / fireRate;
                }

                fireCountdown -= Time.deltaTime;
            }

            // tick system muss noch gemacht werden
            if (turretType == TurretType.DOT)
            {
                if (fireCountdown <= 0f)
                {
                    //DOT();
                    fireCountdown = 1f / fireRate;
                }

                fireCountdown -= Time.deltaTime;
            }
        }

    }
    
    
    void DamageOverTime()
    {
        targetEnemy.TakeDamage(damageOverTime * Time.deltaTime);
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
    DAMAGE,
    DAMAGEOT,
    NONDAMAGE,
    SLOW,
    STUN,
    AOE,
    DOT
}