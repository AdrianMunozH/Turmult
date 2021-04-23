using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageOvertime : TurretEffects
{
    public float damage;
    // Start is called before the first frame update
    void Start()
    {
        _type = TurretType.DAMAGEOT;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    void DamageOverTime(EnemyMovement targetEnemy)
    {
        targetEnemy.TakeDamage(damage * Time.deltaTime);
    }
}
