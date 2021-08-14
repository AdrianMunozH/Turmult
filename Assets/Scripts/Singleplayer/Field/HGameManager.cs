using System;
using System.Collections;
using System.Collections.Generic;
using Singleplayer.Enemies;
using Singleplayer.Turrets;
using Singleplayer.Ui.Input;
using UnityEngine;
using UnityEngine.UIElements;
using Image = UnityEngine.UI.Image;

namespace Singleplayer.Field
{
    public class HGameManager : MonoBehaviour
    {
        public static HGameManager instance;

        public float buildingPhaseTimer = 20;
        public float firstBuildingPhaseTimer = 2;
        public float betweenRoundsTimer = 10;
        public GameObject timeBar;
        public GameObject lifeBar;
        private static UnityEngine.UI.Image _timebar;
        private static UnityEngine.UI.Image _lifebar;

        [Header("Lifes")] 
        public static int totalLifes = 20;
        private static int _currentLifes;
        
        //WaveSpawning
        [Header("Spawning")] public EnemySpawn spawnPoint;
        public int waves = 50;
        public float timeBetweenMinionSpawn = 0.8f;
        public int minionsPerWave = 5;
        private static int _currentWave = 0;
        private List<HCell> minionPath;

        private int _spawnCounter = 0;

        //Wird benötigt für das checken, ob alle Minions getötet wurden
        private bool allMinionsSpawned;

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
            _timebar = timeBar.GetComponent<Image>();
            _lifebar = lifeBar.GetComponent<Image>();
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

            spawnPoint = Instantiate(spawnPoint);
            spawnPoint.end = _hexGrid.GetHCellByXyzCoordinates(distanceFromSpawn, 0, -distanceFromSpawn);
            spawnPoint.defaultStart = _hexGrid.GetHCellByXyCoordinates(0, 0);

            spath = spawnPoint.Solve();
            List<HCell> sp = spawnPoint.RecPath(spath);

            foreach (HCell hcell in sp)
            {
                //Ressource wird im shortestpath benötigt!
                hcell.Celltype = HCell.CellType.Acquired;
                hcell.resource = _hexGrid.GetHCellByIndex(hcell.index).resource;
            }

            _hexGrid.ShortestPathPrefabs(spawnPoint.ShortestPath(sp).ToArray());


        }

        private void Update()
        {
            if (_timer > 0 && !PlayerInputManager.Instance.GetState().name.Equals(StateEnum.Battle))
            {
                _timer -= Time.deltaTime;
                _timebar.fillAmount = (_timer / buildingPhaseTimer);
            }
            else if (!PlayerInputManager.Instance.GetState().name.Equals(StateEnum.Battle))
            {
                PlayerInputManager.Instance.SetState(new BattleState());
                if (_currentWave < waves)
                {
                    SpawnEnemyWave();

                    _currentWave++;
                }
            }

            if (allMinionsSpawned && spawnPoint.enemys.Count == 0)
            {
                allMinionsSpawned = false;
                PlayerInputManager.Instance.SetState(new BuildState());
                _timer = buildingPhaseTimer;
            }
        }

        IEnumerator SpawnEnemyWithDelay()
        {
            yield return new WaitForSeconds(timeBetweenMinionSpawn * _spawnCounter);
            // es muss gecheckt werden ob die weglänge grö0er als 0 ist
            Debug.Log("spawned...");
            spawnPoint.SpawnEnemy(minionPath.ToArray(), false);
            if (_spawnCounter == minionsPerWave) allMinionsSpawned = true;
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
            _spawnCounter = 0;
            // könnte in der methode init werden (spath)
            spath = spawnPoint.Solve();
            List<HCell> sp = spawnPoint.RecPath(spath);
            minionPath = spawnPoint.ShortestPath(sp);
            for (int i = 0; i < minionsPerWave; i++)
            {
                StartCoroutine(nameof(SpawnEnemyWithDelay));
                _spawnCounter++;
            }

            _hexGrid.ShortestPathPrefabs(minionPath.ToArray());
        }

        public static void loseLife(int value)
        {
            if (_currentLifes-value >= 1)
            {
                _currentLifes-=value;
                _lifebar.fillAmount = (_currentLifes / totalLifes);
            }
            else
            {
                //TODO: Hier hat der Spieler verloren
            }
        }
        
    }
}