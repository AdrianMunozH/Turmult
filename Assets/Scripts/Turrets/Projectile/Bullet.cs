using UnityEngine;

namespace Turrets.Projectile
{
    public class Bullet : Projectile
    {
    
        public GameObject impactEffect;
        public float particelDestroyTime = 2f;
 

        // Update is called once per frame


        public override void HitTarget()
        {
            //Partikeleffekt
            GameObject particleEffect = (GameObject) Instantiate(impactEffect, transform.position, transform.rotation);
            //TODO: Hit implementieren
            //target.GetComponent<EnemyMovement>().TakeDamage(damage);
            foreach (var effect in effects)
            {
                effect.Hit();
            }
            Destroy(gameObject);
            Destroy(particleEffect, particelDestroyTime);
        }
    }
}