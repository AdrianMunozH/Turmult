using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyMovement : MonoBehaviour
{
    public Vector3[] path;
    public float moveSpeed;

    private int pathIndex;

    // Start is called before the first frame update
    void Start()
    {
        pathIndex = 0;
    }

    // Update is called once per frame
    void Update()
    {
        if (Vector3.Distance(transform.position, path[pathIndex]) < 0.1f)
        {
            if (pathIndex < path.Length - 1)
                pathIndex++;
            else
                Destroy(gameObject);
        }

        //rotation funktioniert nicht
        Vector3 lookingDir = path[pathIndex] - transform.position;
        float angle = Mathf.Atan2(lookingDir.y, lookingDir.x) * Mathf.Rad2Deg;
        //transform.rotation = Quaternion.AngleAxis(angle,Vector3.forward);

        //funkt. aber ist snappy
        //transform.rotation = Quaternion.LookRotation(lookingDir);
        transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(lookingDir),
            Time.deltaTime * moveSpeed * 2);

        transform.position = Vector3.MoveTowards(transform.position, path[pathIndex], moveSpeed * Time.deltaTime);
    }
}