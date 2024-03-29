using UnityEngine;

namespace Singleplayer.Turrets.Effects
{
    public class DpsEffect : Effect
    {
        public float damagePerSecond;
        //ist gerade noch sinnlos
        private bool _isHitting;
        // Start is called before the first frame update
        void Start()
        {
            Type = EffectType.DPS;

        }

        // Update is called once per frame
        void Update()
        {
            if (targetEnemy != null)
            {
                targetEnemy.TakeDamage(damagePerSecond * Time.deltaTime);
            }
        
            _isHitting = false;
        }

        // hier sollte vllt nicht hit benutzt werden.
        public override void Hit()
        {
            _isHitting = true;
        }

    }
}
