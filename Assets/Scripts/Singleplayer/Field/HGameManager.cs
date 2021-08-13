using System;
using System.Collections;
using System.Collections.Generic;
using Singleplayer.Enemies;
using Singleplayer.Turrets;
using UnityEngine;

namespace Singleplayer.Field
{
    public class HGameManager : MonoBehaviour
    {
        public static HGameManager instance;
        public EnemySpawn spawnPoint;

        [HideInInspector] public EnemySpawn[] enemySpawns;
        
        private HCell[] spath;
        private HGrid _hexGrid;
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
            _hexGrid = GameObject.Find("HexGrid").GetComponent<HGrid>();
            if (_hexGrid == null) throw new Exception("Kein Objekt HexGrid in der Szene gefunden oder es keine Komponente HGrid an diese! ");
            instance = this;
        }

        void Start()
        {
            TimeTickSystem.OnTick += delegate(object sender, TimeTickSystem.OnTickEventArgs args)
            {
            
            };
            
            enemySpawns = new EnemySpawn[3];
            enemySpawns[0] = Instantiate<EnemySpawn>(spawnPoint);
            enemySpawns[1] = Instantiate<EnemySpawn>(spawnPoint);
            enemySpawns[2] = Instantiate<EnemySpawn>(spawnPoint);

            enemySpawns[0].end = _hexGrid.GetHCellByXyzCoordinates(0, -distanceFromSpawn, distanceFromSpawn);
            enemySpawns[0].defaultStart = _hexGrid.GetHCellByXyCoordinates(0, 0);
            enemySpawns[1].end = _hexGrid.GetHCellByXyzCoordinates(distanceFromSpawn, 0, -distanceFromSpawn);
            enemySpawns[1].defaultStart = _hexGrid.GetHCellByXyCoordinates(0, 0);
            enemySpawns[2].end = _hexGrid.GetHCellByXyzCoordinates(-distanceFromSpawn, distanceFromSpawn, 0);
            enemySpawns[2].defaultStart = _hexGrid.GetHCellByXyCoordinates(0, 0);

            foreach (EnemySpawn enemySpawn in enemySpawns)
            {
                spath = enemySpawn.Solve();
                List<HCell> sp = enemySpawn.RecPath(spath);

                foreach (HCell hcell in sp)
                {
                    //Ressource wird im shortestpath benötigt!
                    hcell.ressource = _hexGrid.GetHCellByIndex(hcell.index).GetComponentInChildren<Ressource>();
                }

                _hexGrid.ShortestPathPrefabs(enemySpawn.ShortestPath(sp).ToArray());
            }
            
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

        public void SpawnEnemyWave()
        {
            foreach (EnemySpawn enemySpawn in enemySpawns)
            {
                // könnte in der methode init werden (spath)
                spath = enemySpawn.Solve();
                List<HCell> sp = enemySpawn.RecPath(spath);
                sp = enemySpawn.ShortestPath(sp);
                
                // es muss gecheckt werden ob die weglänge grö0er als 0 ist
                if (sp.Count > 0)
                    enemySpawn.SpawnEnemy(sp.ToArray(),false);
                    // normaler modus                Nicht angreifen
                else
                {
                    // attacking modus
                    spath = enemySpawn.SolveAttack(_hexGrid.GetHCellByXyzCoordinates(0,0,0));
                    // es wird erstmal der kürzeste weg zur base gesucht
                    sp = enemySpawn.RecPath(spath);
                    sp = enemySpawn.ShortestPath(sp);
                    // danach wird am ersten turm gestoppt
                    int towerIndex = (int) enemySpawn.TowerFinder(sp); // +1
                    // von towerindex bis zum letzten element
                    sp.RemoveRange(towerIndex,sp.Count-towerIndex);
                    enemySpawn.SpawnEnemy(sp.ToArray(),true);
                }
                _hexGrid.ShortestPathPrefabs(sp.ToArray());
            }
        }
        

        public void rerouteEnemys(HCell turretCell)
        {
            foreach (EnemySpawn enemySpawn in enemySpawns)
            {
                enemySpawn.recheckPath(turretCell);
            }
        }
    }
}