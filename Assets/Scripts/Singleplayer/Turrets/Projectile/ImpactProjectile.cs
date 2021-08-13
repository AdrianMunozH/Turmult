using System;
using Singleplayer.Enemies;
using Singleplayer.Field;
using UnityEngine;

namespace Singleplayer.Turrets.Projectile
{
    public class ImpactProjectile : Projectile
    {
        private Vector3 impact;

        private HGrid _hexGrid;
        // Start is called before the first frame update

        private void Awake()
        {
            _hexGrid = GameObject.Find("HexGrid").GetComponent<HGrid>();
            if (_hexGrid == null) throw new Exception("Kein Objekt HexGrid in der Szene gefunden oder es keine Komponente HGrid an diese! ");
            if (target != null)
            {
                Debug.Log("nicht null");
                int index;
                EnemyMovement targetEnemy = target.GetComponent<EnemyMovement>();
                Vector3[] path = _hexGrid.HCellPositions(targetEnemy.path);
            
                if (targetEnemy.pathIndex < path.Length-1)
                    index = targetEnemy.pathIndex + 1;
                else
                    index = path.Length - 1;

                // die skalierung muss vllt noch überarbeitet werden 
                Debug.Log(index + " : " + path.Length);
                impact = (path[index] - target.position) * speed/100;
                impact += target.position + new Vector3(0,1,0);
            }
            else
            {
                Debug.Log("null");
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

            var position = transform.position;
            Vector3 dir = impact - position;
            float frameDistance = speed * Time.deltaTime;

            float curve = position.x * Mathf.PI / dir.magnitude;
            curve = Mathf.Sin(curve) - impact.y * position.y/ dir.magnitude;
            
            if (dir.magnitude <= frameDistance)
            {
                HitTarget();
                return;
            }
            transform.Translate(dir.normalized * frameDistance + new Vector3(0,curve,0), Space.World);
            
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
