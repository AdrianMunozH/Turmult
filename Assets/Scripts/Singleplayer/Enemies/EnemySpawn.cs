using System;
using System.Collections.Generic;
using Singleplayer.Field;
using UnityEngine;

namespace Singleplayer.Enemies
{
    //NetworkBehaviour erbt von MonoBehaviour
    public class EnemySpawn : MonoBehaviour
    {
        [SerializeField] public GameObject[] enemyPrefab;
        [HideInInspector] public List<GameObject> enemys;

        //test für shortest path
        private HCell[] spath;
        int input;
        public HCell defaultStart;
        public HCell end;
        private HGrid _hexGrid;
        
        //test für shortest path
        private void Awake()
        {
            _hexGrid = GameObject.Find("HexGrid").GetComponent<HGrid>();
            if (_hexGrid == null) throw new Exception("Kein Objekt HexGrid in der Szene gefunden oder es keine Komponente HGrid an diese! ");
        }

        // Start is called before the first frame update
        void Start()
        {
            enemys = new List<GameObject>();
        }

        // checkt ob der weg eines minions überhaupt geöndert werden muss
        public void RecheckPath(HCell turretCell)
        {
            for (int i = 0; i < enemys.Count; i++)
            {
                EnemyMovement mov = enemys[i].GetComponent<EnemyMovement>();
                for (int j = mov.pathIndex; j < mov.path.Length; j++)
                {
                    // null check muss vllt drin bleibern
                    if (mov != null && mov.path[j].coordinates.CompareCoord(turretCell.coordinates))
                    {
                        RebuildPath(i, mov.pathIndex);
                    }

                    if (mov.isAttacking)
                        RebuildPath(i, mov.pathIndex);
                }
            }
        }

        public void RebuildPath(int enemyIndex, int startIndex)
        {
            EnemyMovement mov = enemys[enemyIndex].GetComponent<EnemyMovement>();
            HCell[] newPath = Solve(mov.path[startIndex]);
            List<HCell> sp = RecPath(newPath);
            sp = ShortestPath(sp, mov.path[startIndex]);

            //TODO: muss noch der attacking modus rein
            if (sp.Count > 0)
            {
                mov.pathIndex = 0;
                mov.path = sp.ToArray();
                mov.isAttacking = false;
                Debug.Log("hier");
            }
            else
            {
                
                Debug.Log("hier2");
                var attackPath = SolveAttack(mov.path[startIndex]);
                Debug.Log(_hexGrid.ArrayToString(attackPath )+  " attackPath" );
                // es wird erstmal der kürzeste weg zur base gesucht
                var attackList = RecPath(attackPath);
                Debug.Log(_hexGrid.ArrayToString(attackList.ToArray()) + " recpath" );
                attackList = ShortestPath(attackList,mov.path[startIndex]);
                Debug.Log(_hexGrid.ArrayToString(attackList.ToArray()) +" shortestpath" );
                
                // danach wird am ersten turm gestoppt
                int towerIndex = (int) TowerFinder(attackList); // +1
                // von towerindex bis zum letzten element
                Debug.Log(_hexGrid.ArrayToString(attackList.ToArray()) );
                attackList.RemoveRange(towerIndex,attackList.Count-towerIndex);
                
                // neu setzen des weges
                mov.path = attackList.ToArray();
                mov.isAttacking = true;
                mov.pathIndex = 0;

                /*alt 
                // attacking modus
                newPath = SolveAttack(mov.path[startIndex]);
                sp = RecPath(newPath);
                sp = ShortestPath(sp);
                
                // danach wird am ersten turm gestoppt (vllt +1)
                int towerIndex = (int) TowerFinder(sp);
                // von towerindex bis zum letzten element
                sp.RemoveRange(towerIndex, sp.Count - towerIndex);
                mov.isAttacking = true;
                mov.path = sp.ToArray();
                */
            }
            // ich glaube das muss raus -- prefabs werden in hgamemanager neu gesetzt 
            //_hexGrid.ShortestPathPrefabs(sp.ToArray());
        }

        public void SpawnEnemy(HCell[] path, bool isAttacking,int prefabId)
        {
            GameObject enemy = Instantiate(enemyPrefab[prefabId]);
            enemys.Add(enemy);
            enemy.transform.SetParent(transform, false);
            enemy.transform.position = path[0].gameObject.transform.position;
            EnemyMovement enemyMovement = enemy.GetComponent<EnemyMovement>();
            enemyMovement.moveSpeed = 3f;
            enemyMovement.isAttacking = isAttacking;
            enemyMovement.path = path;
            enemyMovement.enemySpawn = this;
        }
        
        public HCell[] Solve()
        {
            Queue<HCell> queue = new Queue<HCell>();
            queue.Enqueue(defaultStart);
            //index stellen
            List<int> visited = new List<int>();
            visited.Add(defaultStart.index);
            HCell[] prev = new HCell[_hexGrid.cells.Length];
            int p = 0;
            while (queue.Count > 0)
            {
                HCell node = queue.Dequeue();
                HCell[] neighb = _hexGrid.Neighb(node);
                for (int i = 0; i < neighb.Length; i++)
                {
                    //nicht visited
                    if (!Visited(visited, neighb[i].index))
                    {
                        queue.Enqueue(neighb[i]);
                        visited.Add(neighb[i].index);
                        prev[neighb[i].index] = node;
                        prev[neighb[i].index].spindex = p;
                        p++;
                    }
                }
            }

            return prev;
        }

        public HCell[] Solve(HCell altStart)
        {
            Queue<HCell> queue = new Queue<HCell>();
            queue.Enqueue(altStart);
            //index stellen
            List<int> visited = new List<int>();
            visited.Add(altStart.index);
            HCell[] prev = new HCell[_hexGrid.cells.Length];
            int p = 0;
            while (queue.Count > 0)
            {
                HCell node = queue.Dequeue();
                HCell[] neighb = _hexGrid.Neighb(node);
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

            return prev;
        }

        public HCell[] SolveAttack(HCell altStart)
        {
            Queue<HCell> queue = new Queue<HCell>();
            queue.Enqueue(altStart);
            //index stellen
            List<int> visited = new List<int>();
            visited.Add(altStart.index);
            HCell[] prev = new HCell[_hexGrid.cells.Length];
            int p = 0;
            while (queue.Count > 0)
            {
                HCell node = queue.Dequeue();
                HCell[] neighb = _hexGrid.NeighbAttack(node);
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


            for (HCell at = end; at != null; at = list[at.index])
            {
                path.Add(at);
            }

            return path;
        }

        public int TowerFinder(List<HCell> path)
        {
            for (int j = 0; j < path.Count; j++)
            {
                if (path[j].hasBuilding && path[j].Celltype != HCell.CellType.Base)
                    return j;
            }

            // wenn es kein tower gibt dann ist was falsch gelaufen
            return 0;
        }

        public List<HCell> ShortestPath(List<HCell> path)
        {
            path.Reverse();
            // ich glaub ich war hier bisschen faul und es sollte mehr gecheckt werden :D
            if (path.Count > 0 && path[0].coordinates.CompareCoord(defaultStart.coordinates))
            {
                return path;
            }


            // wenn der weg nicht möglich ist kommt eine leere liste zurück // muss also gecheckt werden
            return new List<HCell>();
        }

        public List<HCell> ShortestPath(List<HCell> path, HCell altStart)
        {
            path.Reverse();
            
            // ich glaub ich war hier bisschen faul und es sollte mehr gecheckt werden :D
            if (path.Count > 0 && path[0].coordinates.CompareCoord(altStart.coordinates))
            {
                return path;
            }

            // wenn der weg nicht möglich ist kommt eine leere liste zurück // muss also gecheckt werden
            return new List<HCell>();
        }

        // Update is called once per frame
        public void deleteEnemy(GameObject go)
        {
            enemys.Remove(go);
        }
    }
}