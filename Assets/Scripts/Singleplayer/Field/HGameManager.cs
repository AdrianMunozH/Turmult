using System;
using System.Collections;
using System.Collections.Generic;
using Singleplayer.Enemies;
using Singleplayer.Turrets;
using Singleplayer.Ui.Input;
using UnityEngine;

namespace Singleplayer.Field
{
    public class HGameManager : MonoBehaviour
    {
        public static HGameManager instance;
        
        public float buildingPhaseTimer = 5;
        public float firstBuildingPhaseTimer = 10;
        public float betweenRoundsTimer = 10;
        
        //WaveSpawning
        public EnemySpawn spawnPoint;
        public int waves;
        public float timeBetweenMinionSpawn;
        public float minionsPerWave;

        [HideInInspector] public EnemySpawn[] enemySpawns;

        private float _timer;
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
        
        [SerializeField] Animator transition;

        void Awake()
        {
            StartCoroutine(LevelTransition());
            
            if (instance != null)
            {
                Debug.LogError("Es gibt mehr als einen Buildmanager in der Szene: Bitte nur eine pro Szene!");
                return;
            }

            _hexGrid = GameObject.Find("HexGrid").GetComponent<HGrid>();
            if (_hexGrid == null)
                throw new Exception(
                    "Kein Objekt HexGrid in der Szene gefunden oder es keine Komponente HGrid an diese! ");
            instance = this;
        }

        void Start()
        {
            //Intiales setzen des Timers
            _timer = firstBuildingPhaseTimer;
            PlayerInputManager.Instance.SetState(new BuildState());

            TimeTickSystem.OnTick += delegate(object sender, TimeTickSystem.OnTickEventArgs args) { };

            enemySpawns = new EnemySpawn[1];
            enemySpawns[0] = Instantiate<EnemySpawn>(spawnPoint);
            enemySpawns[0].end = _hexGrid.GetHCellByXyzCoordinates(distanceFromSpawn, 0, -distanceFromSpawn);
            enemySpawns[0].defaultStart = _hexGrid.GetHCellByXyCoordinates(0, 0);


            foreach (EnemySpawn enemySpawn in enemySpawns)
            {
                spath = enemySpawn.Solve();
                List<HCell> sp = enemySpawn.RecPath(spath);

                foreach (HCell hcell in sp)
                {
                    //Ressource wird im shortestpath benötigt!
                    hcell.Celltype = HCell.CellType.Acquired;
                    hcell.resource = _hexGrid.GetHCellByIndex(hcell.index).resource;
                }

                _hexGrid.ShortestPathPrefabs(enemySpawn.ShortestPath(sp).ToArray());
            }

        }

        private void Update()
        {
            if (_timer > 0 && !PlayerInputManager.Instance.GetState().name.Equals(StateEnum.Battle))
            {
                _timer -= Time.deltaTime;
            }
            else
            {
                PlayerInputManager.Instance.SetState(new BattleState());
                SpawnEnemyWave();
            }



        }
        
        IEnumerator LevelTransition()
        {
            transition.SetTrigger("Start");
            transition.gameObject.SetActive(true);
            
            yield return new WaitForSeconds(1f);
            transition.gameObject.SetActive(false);
            
            
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

        /// <summary>
        /// Singleplayer modus: Nur wegberechnung für einen Spawnpoint
        /// </summary>
        public void SpawnEnemyWave()
        {
            // könnte in der methode init werden (spath)
            spath = spawnPoint.Solve();
            List<HCell> sp = spawnPoint.RecPath(spath);
            sp = spawnPoint.ShortestPath(sp);

            // es muss gecheckt werden ob die weglänge grö0er als 0 ist
            if (sp.Count > 0)
                spawnPoint.SpawnEnemy(sp.ToArray(), false);
            // normaler modus                Nicht angreifen
            else
            {
                // attacking modus
                spath = spawnPoint.SolveAttack(_hexGrid.GetHCellByXyzCoordinates(0, 0, 0));
                // es wird erstmal der kürzeste weg zur base gesucht
                sp = spawnPoint.RecPath(spath);
                sp = spawnPoint.ShortestPath(sp);
                // danach wird am ersten turm gestoppt
                int towerIndex = (int) spawnPoint.TowerFinder(sp); // +1
                // von towerindex bis zum letzten element
                sp.RemoveRange(towerIndex, sp.Count - towerIndex);
                spawnPoint.SpawnEnemy(sp.ToArray(), true);
            }

            _hexGrid.ShortestPathPrefabs(sp.ToArray());
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