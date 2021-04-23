using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DmgEffect : Effect
{
    public float damage;
    
    
    // Start is called before the first frame update
    void Start()
    {
        _type = EffectType.DMG;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public override void Hit()
    {
        targetEnemy.TakeDamage(damage);
    }
}
