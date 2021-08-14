using Singleplayer.Enemies;
using UnityEngine;

namespace Singleplayer.Turrets.Effects
{
    public abstract class Effect : MonoBehaviour
    {
        public EffectType Type { get; set; }

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