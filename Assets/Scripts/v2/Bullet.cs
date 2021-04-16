using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    private Transform target;
    public float speed = 70f;
    public GameObject impactEffect;
    public float particelDestroyTime = 2f;

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

        transform.Translate(dir.normalized*frameDistance,Space.World);
    }

    void HitTarget()
    {
        //Partikeleffekt
        GameObject particleEffect = (GameObject)Instantiate(impactEffect, transform.position, transform.rotation);
        //TODO: Hit implementieren
        Destroy(gameObject);
        Destroy(particleEffect,particelDestroyTime);
    }
    

}
