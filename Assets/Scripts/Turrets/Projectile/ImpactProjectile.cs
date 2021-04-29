using Enemies;
using Field;
using UnityEngine;

namespace Turrets.Projectile
{
    public class ImpactProjectile : Projectile
    {
        private Vector3 impact;
        // Start is called before the first frame update
        void Start()
        {
        
        }

        private void Awake()
        {
            if (target != null)
            {

                int index;
                EnemyMovement targetEnemy = target.GetComponent<EnemyMovement>();
                Vector3[] path = HGrid.Instance.HCellPositions(targetEnemy.path);
            
                if (targetEnemy.pathIndex < path.Length-1)
                    index = targetEnemy.pathIndex + 1;
                else
                    index = path.Length - 1;

                // die skalierung muss vllt noch überarbeitet werden 
                Debug.Log(index + " : " + path.Length);
                impact = (path[index] - target.position) * speed/100;
                impact += target.position + new Vector3(0,1,0);
            }
        }

        // Update is called once per frame
        new void Update()
        {
            if (target == null)
            {
                Destroy(gameObject);
                return;
            }
        
            Vector3 dir = impact - transform.position;
            float frameDistance = speed * Time.deltaTime;
        
            if (dir.magnitude <= frameDistance)
            {
                HitTarget();
                return;
            }
            transform.Translate(dir.normalized * frameDistance, Space.World);
        }

        public override void HitTarget()
        {
            foreach (var effect in effects)
            {
                effect.Hit();
            }
            Destroy(gameObject);
        }
    }
}
