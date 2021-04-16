using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawn : MonoBehaviour
{
    public GameObject enemy;
    private GameObject[] enemys;

    //test für shortest path
    private HCell[] spath;
    int input;
    public HCell start;
    public HCell end;

    //test für shortest path

    // Start is called before the first frame update
    void Start()
    {
        enemys = new GameObject[10];
        start = HGrid.Instance.GetCellIndex(0, 0);
    }

    public void SpawnEnemy(Vector3[] path)
    {
        // ich mache noch nichts mit den enemys darum 0 
        GameObject gameObject = enemys[0] = Instantiate(enemy);
        gameObject.transform.SetParent(transform, false);
        gameObject.transform.position = path[0];
        EnemyMovement enemyMovement = gameObject.GetComponent<EnemyMovement>();
        enemyMovement.moveSpeed = 3f;
        enemyMovement.path = path;
    }

    //public HCell[] Solve(HCell start)
    public HCell[] Solve()
    {
        Queue<HCell> queue = new Queue<HCell>();
        queue.Enqueue(start);
        //index stellen
        List<int> visited = new List<int>();
        visited.Add(start.index);
        HCell[] prev = new HCell[HGrid.Instance.cells.Length];
        int p = 0;
        while (queue.Count > 0)
        {
            HCell node = queue.Dequeue();
            HCell[] neighb = HGrid.Instance.Neighb(node);
            for (int i = 0; i < neighb.Length; i++)
            {
                //nicht visited
                //if (!visited.Contains(CellsIndex(neighb[i])))
                if (!Visited(visited, neighb[i].index))
                {
                    queue.Enqueue(neighb[i]);
                    //visited.Add(CellsIndex(neighb[i]));
                    visited.Add(neighb[i].index);
                    //prev[i] = node;
                    prev[neighb[i].index] = node;
                    prev[neighb[i].index].spindex = p;
                    p++;
                }
            }
        }

        Debug.Log("solve list: " + HGrid.Instance.ArrayToString(prev));
        return prev;
    }

    public bool Visited(List<int> list, int i)
    {
        foreach (int item in list)
        {
            if (item.Equals(i))
            {
                return true;
            }
        }

        return false;
    }

    //public List<HCell> RecPath(HCell start,HCell end, HCell[] list)
    public List<HCell> RecPath(HCell[] list)
    {
        List<HCell> path = new List<HCell>();
        Debug.Log("sp start" + start.spindex + " index start" + start.index);
        Debug.Log("ende: " + end.coordinates.ToString());


        for (HCell at = end; at != null; at = list[at.index])
        {
            path.Add(at);
        }

        return path;
    }

    public List<HCell> ShortestPath(List<HCell> path)
    {
        path.Reverse();
        // ich glaub ich war hier bisschen faul und es sollte mehr gecheckt werden :D
        if (path.Count > 0 && path[0].coordinates.X == start.coordinates.X)
        {
            return path;
        }

        Debug.Log("leer");
        // wenn der weg nicht möglich ist kommt eine leere liste zurück // muss also gecheckt werden
        return new List<HCell>();
    }

    // Update is called once per frame
}