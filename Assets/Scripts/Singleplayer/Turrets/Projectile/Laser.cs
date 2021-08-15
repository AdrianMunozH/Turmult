using System.Collections.Generic;
using Singleplayer.Enemies;
using Singleplayer.Turrets.Effects;
using UnityEngine;

namespace Singleplayer.Turrets.Projectile
{
    public class Laser : MonoBehaviour
    {
        [HideInInspector]public Transform target;
        public List<Effect> effects;
        public AudioSource audioSource;
        
        // Start is called before the first frame update
        void Start()
        {
            audioSource = GetComponent<AudioSource>();
            foreach (var effect in effects)
            {
                if(target != null)
                    effect.targetEnemy = target.gameObject.GetComponent<EnemyMovement>();
            
            }
        }

        // Update is called once per frame
        void Update()
        {
            if (target != null)
            {
                
                HitTarget();
                if (audioSource != null && !audioSource.isPlaying)
                {
                    audioSource.Play();
                }

            } else 
            {
                if(audioSource != null)
                    audioSource.Stop();
            }
        }
        
        public void Seek(Transform _target)
        {
            //TODO: SET EFFECTS OR SPEED
            target = _target;
        }

        private void HitTarget()
        {
            foreach (var effect in effects)
            {
                effect.targetEnemy = target.GetComponent<EnemyMovement>();
                effect.Hit();
            }
        }
    }
}
