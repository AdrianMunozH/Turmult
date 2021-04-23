using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AoeProjectile : MonoBehaviour
{
    private Transform target;
    public float speed = 50f;
    public GameObject impactEffect;
    public float particelDestroyTime = 2f;
    public float damage;

    public void Seek(Transform _target)
    {
        //TODO: SET EFFECTS OR SPEED
        target = _target;
    }

    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        // Falls Gegner schon tot oder Ende erreicht hat, Bullet zerstören
        if (target == null)
        {
            Destroy(gameObject);
            return;
        }

        Vector3 dir = target.position - transform.position;
        float frameDistance = speed * Time.deltaTime;
        //Treffer!
        if (dir.magnitude <= frameDistance)
        {
            HitTarget();
            return;
        }

        transform.Translate(dir.normalized * frameDistance, Space.World);
    }

    void HitTarget()
    {
        //Partikeleffekt
        GameObject particleEffect = (GameObject) Instantiate(impactEffect, transform.position, transform.rotation);
        //TODO: Hit implementieren
        Collider[] hit = Physics.OverlapSphere(transform.position,0.6f);
        foreach (Collider collider in hit)
        {
            if (collider.gameObject.tag.Equals("Enemy"))
            {
                collider.GetComponent<EnemyMovement>().TakeDamage(damage);
            }
        }
        Destroy(gameObject);
        Destroy(particleEffect, particelDestroyTime);
    }
}
