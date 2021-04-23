using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Effect : MonoBehaviour
{
    public EffectType _type;
    public EnemyMovement targetEnemy;
    // Start is called before the first frame update
    

    // Update is called once per frame
    void Update()
    {
        
    }

    public abstract void Hit();
}


public enum EffectType
{
    SLOW,
    STUN,
    DMG,
    DOT,
    AOE,
    SPREAD,
    CHAIN,
    DEBUFF
}
