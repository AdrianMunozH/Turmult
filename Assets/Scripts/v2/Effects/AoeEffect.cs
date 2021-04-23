using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AoeEffect : Effect
{
    public float damage;

    public Collider AOE;
    // Start is called before the first frame update
    void Start()
    {
        _type = EffectType.AOE;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public override void Hit()
    {
        Collider[] hit = Physics.OverlapSphere(transform.position,0.6f);
        foreach (Collider collider in hit)
        {
            if (collider.gameObject.tag.Equals("Enemy"))
            {
                collider.GetComponent<EnemyMovement>().TakeDamage(damage);
            }
        }
    }
}
