using System.Collections;
using Singleplayer.Field;
using Singleplayer.Player;
using Singleplayer.Turrets;
using TMPro;
using UnityEngine;
using Image = UnityEngine.UIElements.Image;

namespace Singleplayer.Enemies
{

    public class EnemyMovement : MonoBehaviour
    {
        [HideInInspector] public HCell[] path;

        public int pathIndex;

        public EnemySpawn enemySpawn;

        private Animator animator;
        private int isAttackingHash;
        private int isDyingHash;
        public GameObject healthbar;

        // Stats
        public float life = 100;
        private float _totalLife;
        public bool isSlowed;
        [HideInInspector] public float moveSpeed;
        public float defaultSpeed;


        
        public bool isAttacking;
        private double _attackCountdown = 0f;
        private double attackRate = 1f;

        public bool isTurret;
        public int dmgOnBase;
        public int goldValue;

        // Start is called before the first frame update
        void Start()
        {
            pathIndex = 0;
            moveSpeed = defaultSpeed;
            _totalLife = life;
            
            animator = GetComponent<Animator>();
            isAttackingHash = Animator.StringToHash("isAttacking");
            isDyingHash = Animator.StringToHash("isDying");
            //_text = GetComponentInChildren<TextMeshProUGUI>();
        }

        // Update is called once per frame
        void Update()
        {
            if(isTurret) return;
            // kann auch ersetzt werden durch path[].gameObject.transform.position
            //vecPath = HGrid.Instance.HCellPositions(path);
            Vector3 position = new Vector3 (path[pathIndex].gameObject.transform.position.x, path[pathIndex].gameObject.transform.position.y + 2.3f, path[pathIndex].gameObject.transform.position.z);
            if (Vector3.Distance(transform.position, position) < 0.01f)
            {
                if (pathIndex < path.Length - 1)
                    pathIndex++;
                else
                {
                    if (isAttacking)
                    {
                        Attacking();
                    }
                    else
                    {
                        Die(false);
                    }
                }

            }

            //rotation funktioniert nicht
            Vector3 lookingDir = position - transform.position;
            float angle = Mathf.Atan2(lookingDir.y, lookingDir.x) * Mathf.Rad2Deg;
            //transform.rotation = Quaternion.AngleAxis(angle,Vector3.forward);

            //funkt. aber ist snappy
            //transform.rotation = Quaternion.LookRotation(lookingDir);
            if(lookingDir != Vector3.zero)
                transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(lookingDir),
                Time.deltaTime * moveSpeed * 2);

            transform.position = Vector3.MoveTowards(transform.position, position, moveSpeed * Time.deltaTime);

            if (!isSlowed)
                moveSpeed = defaultSpeed;

        }

        private void Attacking()
        {
            if (_attackCountdown <= 0f)
            {
                _attackCountdown = 1f / attackRate;
                animator.SetBool(isAttackingHash, true);
            }

            _attackCountdown -= Time.deltaTime;
        }

        public void TakeDamage(float damage)
        {
            if (damage < life)
            {
                if (isTurret)
                {
                    Debug.Log("damage: " + damage);
                }
                life -= damage;
            }
            else
            {
                Die(true);

            }
            if(!isTurret) healthbar.GetComponent<UnityEngine.UI.Image>().fillAmount = (life / _totalLife);
            // kann später gelöscht werden ist nur zum debugen

            // //StartCoroutine("DeactivateText");
        }

        public void Die(bool gotKilled)
        {
            if (gotKilled)
            {
                IncomeManager.Instance.MinionGold(goldValue);
            }
            else
            {
                HGameManager.instance.loseLife(dmgOnBase);
            }
            //nur minions müssen aus der liste removed werden
            if (!isTurret)
                enemySpawn.deleteEnemy(gameObject);
            else
            {
                // wichtiges TODO
                // cell darf kein hasBuilding mehr haben!!
                HGameManager.instance.TowerDestroyed(GetComponent<Turret>().Coordinates);
            }
                
            
            Destroy(gameObject);
        }


        public void Slow(float slow)
        {
            isSlowed = true;
            //Debug.Log("isSlowed");
            moveSpeed = defaultSpeed * (1 - slow);
        }
        
        
        IEnumerator KillEnemy()
        {
            animator.SetBool(isDyingHash, true);
            moveSpeed = 0;
            yield return new WaitForSeconds(2f);
            Destroy(gameObject);
        }
    }
}