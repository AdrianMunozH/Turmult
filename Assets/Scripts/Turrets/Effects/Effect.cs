using System.Collections;
using System.Collections.Generic;
using Enemies;
using UnityEngine;

namespace Turrets.Effects
{
    public abstract class Effect : MonoBehaviour
    {
        [field: HideInInspector] public EffectType Type { get; set; }

        public EnemyMovement targetEnemy;
        // Start is called before the first frame update
    

        // Update is called once per frame

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
        DEBUFF,
        DPS
    }
}