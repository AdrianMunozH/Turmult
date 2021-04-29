using System.Collections;
using Field;
using TMPro;
using UnityEngine;

namespace Enemies
{

    public class EnemyMovement : MonoBehaviour
    {
        [HideInInspector] public HCell[] path;

        public int pathIndex;

        public EnemySpawn enemySpawn;


        // Stats
        public float life = 100;
        public bool isSlowed;
        [HideInInspector] public float moveSpeed;
        public float defaultSpeed;

        //kann wahrscheinlich raus
        private Canvas _canvas;
        public TMP_Text _text;


        // Start is called before the first frame update
        void Start()
        {
            pathIndex = 0;
            moveSpeed = defaultSpeed;
            _canvas = GetComponent<Canvas>();
            //_text = GetComponentInChildren<TextMeshProUGUI>();
        }

        // Update is called once per frame
        void Update()
        {
            // kann auch ersetzt werden durch path[].gameObject.transform.position
            //vecPath = HGrid.Instance.HCellPositions(path);
            Vector3 position = path[pathIndex].gameObject.transform.position;
            if (Vector3.Distance(transform.position, position) < 0.1f)
            {
                if (pathIndex < path.Length - 1)
                    pathIndex++;
                else
                {
                    enemySpawn.deleteEnemy(gameObject);
                    Destroy(gameObject);
                }

            }

            //rotation funktioniert nicht
            Vector3 lookingDir = position - transform.position;
            float angle = Mathf.Atan2(lookingDir.y, lookingDir.x) * Mathf.Rad2Deg;
            //transform.rotation = Quaternion.AngleAxis(angle,Vector3.forward);

            //funkt. aber ist snappy
            //transform.rotation = Quaternion.LookRotation(lookingDir);
            transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(lookingDir),
                Time.deltaTime * moveSpeed * 2);

            transform.position = Vector3.MoveTowards(transform.position, position, moveSpeed * Time.deltaTime);

            if (!isSlowed)
                moveSpeed = defaultSpeed;

        }

        public void TakeDamage(float damage)
        {
            if (damage < life)
            {
                life -= damage;
            }
            else
                Destroy(gameObject);
            // die methode falls wir sowas wie deathanimation macehn 

            _text.enabled = true;
            _text.SetText(life.ToString());
            //StartCoroutine("DeactivateText",5f);
        }


        public void Slow(float slow)
        {
            isSlowed = true;
            Debug.Log("isSlowed");
            moveSpeed = defaultSpeed * (1 - slow);
        }

        IEnumerator DeactivateText()
        {
            _text.SetText("");
            yield return new WaitForSeconds(0.2f);
        }
    }
}