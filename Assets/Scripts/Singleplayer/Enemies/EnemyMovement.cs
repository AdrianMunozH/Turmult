using System.Collections;
using Singleplayer.Field;
using Singleplayer.Player;
using TMPro;
using UnityEngine;

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


        // Stats
        public float life = 100;
        public bool isSlowed;
        [HideInInspector] public float moveSpeed;
        public float defaultSpeed;

        //kann wahrscheinlich raus
        private Canvas _canvas;
        public TMP_Text _text;
        public bool isAttacking;
        private double _attackCountdown = 0f;
        private double attackRate = 1f;

        public int dmgOnBase;
        public int goldValue;

        // Start is called before the first frame update
        void Start()
        {
            pathIndex = 0;
            moveSpeed = defaultSpeed;
            _canvas = GetComponent<Canvas>();
            
            animator = GetComponent<Animator>();
            isAttackingHash = Animator.StringToHash("isAttacking");
            isDyingHash = Animator.StringToHash("isDying");
            //_text = GetComponentInChildren<TextMeshProUGUI>();
        }

        // Update is called once per frame
        void Update()
        {
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
                        _text.SetText("Attack!");
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
                _text.SetText("Hit!");
                _attackCountdown = 1f / attackRate;
                animator.SetBool(isAttackingHash, true);
            }

            _attackCountdown -= Time.deltaTime;
        }

        public void TakeDamage(float damage)
        {
            if (damage < life)
            {
                 life -= damage;
            }
            else
                 Die(true);
            // // die methode falls wir sowas wie deathanimation macehn 
            //

            // kann später gelöscht werden ist nur zum debugen
            _text.enabled = true;
            _text.SetText(life.ToString());
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
            enemySpawn.deleteEnemy(gameObject);
            Destroy(gameObject);
        }


        public void Slow(float slow)
        {
            isSlowed = true;
            //Debug.Log("isSlowed");
            moveSpeed = defaultSpeed * (1 - slow);
        }

        IEnumerator DeactivateText()
        {
            _text.SetText("");
            yield return new WaitForSeconds(0.2f);
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