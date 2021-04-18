using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyMovement : MonoBehaviour
{
    public HCell[] path;
    private Vector3[] vecPath;
    public float moveSpeed;

    private int pathIndex;

    public EnemySpawn enemySpawn;
    
    public int PathIndex => pathIndex;

    // Start is called before the first frame update
    void Start()
    {
        pathIndex = 0;
    }

    // Update is called once per frame
    void Update()
    {
        // kann auch ersetzt werden durch path.gameObject.transform.position
        vecPath = HGrid.Instance.HCellPositions(path);
        if (Vector3.Distance(transform.position, vecPath[pathIndex]) < 0.1f)
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
        Vector3 lookingDir = vecPath[pathIndex] - transform.position;
        float angle = Mathf.Atan2(lookingDir.y, lookingDir.x) * Mathf.Rad2Deg;
        //transform.rotation = Quaternion.AngleAxis(angle,Vector3.forward);

        //funkt. aber ist snappy
        //transform.rotation = Quaternion.LookRotation(lookingDir);
        transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(lookingDir),
            Time.deltaTime * moveSpeed * 2);

        transform.position = Vector3.MoveTowards(transform.position, vecPath[pathIndex], moveSpeed * Time.deltaTime);
    }
}