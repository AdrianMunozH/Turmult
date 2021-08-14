using UnityEngine;

namespace Singleplayer.Turrets.Effects
{
    public class DmgEffect : Effect
    {
        public float damage;
    
    
        // Start is called before the first frame update
        void Start()
        {
            Type = EffectType.DMG;
        }

        // Update is called once per frame
        void Update()
        {
        
        }

        public override void Hit()
        {
            Debug.Log("hit");
            targetEnemy.TakeDamage(damage);
        }
    }
}
