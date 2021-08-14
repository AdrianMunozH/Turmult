using UnityEngine;

namespace Singleplayer.Turrets.Projectile
{
    public class Bullet : Projectile
    {
    
        public GameObject impactEffect;
        public float particelDestroyTime = 2f;
 

        // Update is called once per frame


        public override void HitTarget()
        {
            
            //TODO: Hit implementieren
            //target.GetComponent<EnemyMovement>().TakeDamage(damage);
            foreach (var effect in effects)
            {
                effect.Hit();
            }
            //Partikeleffekt
            if(impactEffect != null)
            {
                var particleEffect = (GameObject) Instantiate(impactEffect, transform.position, transform.rotation);
                Destroy(particleEffect, particelDestroyTime);
            }

            Destroy(gameObject);
            
        }
    }
}