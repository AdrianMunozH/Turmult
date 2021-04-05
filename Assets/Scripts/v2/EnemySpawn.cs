using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawn : MonoBehaviour
{
    public GameObject enemy;
    private GameObject[] enemys;

    // Start is called before the first frame update
    void Start()
    {
        enemys = new GameObject[10];
    }

    public void SpawnEnemy(Vector3[] path )
    {
        // ich mache noch nichts mit den enemys darum 0 
        GameObject gameObject = enemys[0]  = Instantiate(enemy);
        gameObject.transform.SetParent(transform, false);
        gameObject.transform.position = path[0];
        EnemyMovement enemyMovement = gameObject.GetComponent<EnemyMovement>();
        enemyMovement.moveSpeed = 3f;
        enemyMovement.path = path;

    }

    // Update is called once per frame

}
