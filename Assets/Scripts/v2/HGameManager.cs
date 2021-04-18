using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HGameManager : Singleton<HGameManager>
{
    public EnemySpawn spawnPoint;

    private EnemySpawn[] _enemySpawns;
    private HCell[] spath;
    int input;
    
    //sollte raus
    HCell start;
    HCell end;


    //test für shortest path

    private bool cooldown;

    private bool deleteLater;

    // Start is called before the first frame update
    void Start()
    {
        // test erstmal nur die eine
        _enemySpawns = new EnemySpawn[3];
        _enemySpawns[0] = Instantiate<EnemySpawn>(spawnPoint);
        _enemySpawns[1] = Instantiate<EnemySpawn>(spawnPoint);
        _enemySpawns[2] = Instantiate<EnemySpawn>(spawnPoint);

        _enemySpawns[0].end = HGrid.Instance.GetCellIndex(0, -5, 5);
        _enemySpawns[1].end = HGrid.Instance.GetCellIndex(5, 0, -5);
        _enemySpawns[2].end = HGrid.Instance.GetCellIndex(-5, 5, 0);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButton(0) && !cooldown)
        {
            HandleInput();
        }
    }

    void HandleInput()
    {
        Ray inputRay = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (Physics.Raycast(inputRay, out hit))
        {
            //HighlightCell2(hit.collider.gameObject.transform.position);
            /*
            Debug.Log(input);
            if (input == 0)
            {
                OnCooldown();
                input++;
                start = hit.collider.gameObject.GetComponent<HCell>();
                Debug.Log("Start" + start.coordinates.ToString());
                
            } else if (input == 1)
            {
                OnCooldown();
                end = hit.collider.gameObject.GetComponent<HCell>();
                Debug.Log("end" + end.coordinates.ToString());
                input++;

            }
            */
            if (hit.collider.gameObject.name == "pseudoButton")
            {
                SpawnEnemyWave();
            }
        }

        /*
    void HandleInput () {
        Ray inputRay = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        
        if (Physics.Raycast(inputRay, out hit)) {
            //HighlightCell2(hit.collider.gameObject.transform.position);

            if (input == 0)
            {
                OnCooldown();
                input++;
                start = hit.collider.gameObject.GetComponent<HCell>();
                Debug.Log("Start" + start.coordinates.ToString());
                spath = Solve(start);
                
                

            } else if (input == 1)
            {
                OnCooldown();
                end = hit.collider.gameObject.GetComponent<HCell>();
                List<HCell> sp = RecPath(start, end, spath);
                sp = ShortestPath(sp);
                Debug.Log("***Ergebnis*** " + hGrid.ListToString(sp) + " : "+ sp.Count);
                input = 0;
                if(sp.Count > 0)
                    enemySpawn.SpawnEnemy(hGrid.HCellPositions(sp));
            }
            //Neighb(hit.collider.gameObject.GetComponent<HCell>());
            
            //TouchCell(hit.point);
        }
        */
    }

    private void OnCooldown()
    {
        cooldown = true;
        StartCoroutine("ResetCooldown");
    }

    IEnumerator ResetCooldown()
    {
        yield return new WaitForSeconds(1f);
        cooldown = false;
    }

    private void SpawnEnemyWave()
    {
        foreach (EnemySpawn enemySpawn in _enemySpawns)
        {
            spath = enemySpawn.Solve();
            List<HCell> sp = enemySpawn.RecPath(spath);
            sp = enemySpawn.ShortestPath(sp);
            Debug.Log("***Ergebnis*** " + HGrid.Instance.ListToString(sp) + " : " + sp.Count);
            // es muss gecheckt werden ob der weg länge grö0er ist als 0
            if (sp.Count > 0)
                enemySpawn.SpawnEnemy(sp.ToArray());
        }
    }

    public void rerouteEnemys(HCell turretCell)
    {
        foreach (EnemySpawn enemySpawn in _enemySpawns)
        {
            enemySpawn.recheckPath(turretCell);
        }
    }
}