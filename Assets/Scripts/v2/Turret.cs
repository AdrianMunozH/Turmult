using UnityEngine;
public class Turret : MonoBehaviour
{
    private Transform target;
    public float range = 15f;
    public string enemyTag = "Enemy";
    public Transform partToRotate;
    public float turnSpeed = 10f;
    

    // Start is called before the first frame update
    void Start()
    { 
        InvokeRepeating(nameof(UpdateTarget),0f,0.5f);
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
                shortestDistance = distanceToEnemy;
                nearestEnemy = enemy;
            }
        }

        if (nearestEnemy != null && shortestDistance <= range)
        {
            target = nearestEnemy.transform;
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position,range);
    }

    void Update()
    {
        //Wenn kein Target verhanden keine Updates
        if (target == null)
        {
            return;
        }
        //Point-Direction Vektor ermitteln
        Vector3 direction = target.position - transform.position;
        Quaternion lookRotation = Quaternion.LookRotation(direction);
        //Berechnung der Rotation mit Smooth Lerp
        Vector3 rotation = Quaternion.Lerp(partToRotate.rotation,lookRotation,Time.deltaTime * turnSpeed).eulerAngles;
        partToRotate.rotation = Quaternion.Euler(0f,rotation.y,0f);
    }
}
