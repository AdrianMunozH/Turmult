using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HGameManager : Singleton<HGameManager>
{
    public static HGameManager instance;
    public EnemySpawn spawnPoint;

    private EnemySpawn[] _enemySpawns;
    private HCell[] spath;
    int input;
    
    //sollte raus
    HCell start;
    HCell end;

    [Header("Setup Endpoint")] public int distanceFromSpawn = 5;



    //test für shortest path

    private bool cooldown;

    private bool deleteLater;

    void Awake()
    {
        if (instance != null)
        {
            Debug.LogError("Es gibt mehr als einen Buildmanager in der Szene: Bitte nur eine pro Szene!");
            return;
        }

        instance = this;
    }

    void Start()
    {
        // test erstmal nur die eine
        _enemySpawns = new EnemySpawn[3];
        _enemySpawns[0] = Instantiate<EnemySpawn>(spawnPoint);
        _enemySpawns[1] = Instantiate<EnemySpawn>(spawnPoint);
        _enemySpawns[2] = Instantiate<EnemySpawn>(spawnPoint);

        _enemySpawns[0].end = HGrid.Instance.GetCellIndex(0, -distanceFromSpawn, distanceFromSpawn);
        _enemySpawns[1].end = HGrid.Instance.GetCellIndex(distanceFromSpawn, 0, -distanceFromSpawn);
        _enemySpawns[2].end = HGrid.Instance.GetCellIndex(-distanceFromSpawn, distanceFromSpawn, 0);
    }

    void Update()
    {

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