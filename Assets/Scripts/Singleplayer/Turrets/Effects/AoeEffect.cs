using Singleplayer.Enemies;
using UnityEngine;

namespace Singleplayer.Turrets.Effects
{
    public class AoeEffect : Effect
    {
        public float damage;
        public bool damageTargetOnce;
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
                    Debug.Log(!damageTargetOnce + " " + !collider.GetComponent<EnemyMovement>().Equals(targetEnemy));
                
                    //  klassen zu vergleichen ist kacke darum funkt. nicht, brauche noch eine ID oder sowas
                    if (!damageTargetOnce && !collider.GetComponent<EnemyMovement>().Equals(targetEnemy))
                    {
                        collider.GetComponent<EnemyMovement>().TakeDamage(damage);
                    }

                }
            }
        }
    }
}
