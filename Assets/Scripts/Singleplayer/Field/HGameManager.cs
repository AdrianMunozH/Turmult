using System;
using System.Collections;
using System.Collections.Generic;
using Singleplayer.Player;
using Singleplayer.Enemies;
using Singleplayer.Turrets;
using Singleplayer.Ui.Input;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using Image = UnityEngine.UI.Image;

namespace Singleplayer.Field
{
    public class HGameManager : MonoBehaviour
    {
        public static HGameManager instance;

        public float buildingPhaseTimer = 20;
        public float firstBuildingPhaseTimer = 2;
        public float betweenRoundsTimer = 10;
        [Header("UI")] 
        public GameObject timeBar;
        public GameObject lifeBar;
        private static Image _timebar;
        private static Image _lifebar;
        public GameObject gameOver;
        

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
        public static int timeTillSceneChange = 15;

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
            _lifebar.fillAmount = 1;
            
            _currentLifes = totalLifes;

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
            if (_currentLifes > 0)
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
                    IncomeManager.Instance.Interest();
                    PlayerInputManager.Instance.SetState(new BuildState());
                    _timer = buildingPhaseTimer;
                }
            }
            else
            {
                gameOver.SetActive(true);
            }
        }

        IEnumerator SpawnEnemyWithDelay()
        {
            yield return new WaitForSeconds(timeBetweenMinionSpawn * _spawnCounter);
            // es muss gecheckt werden ob die weglänge grö0er als 0 ist
            spawnPoint.SpawnEnemy(minionPath.ToArray(), false);
            if (_spawnCounter == minionsPerWave) allMinionsSpawned = true;
        }
        
        IEnumerator LevelTransition()
        {
            yield return new WaitForSeconds(timeTillSceneChange);
            SceneManager.LoadScene("MainMenu");
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

        public void loseLife(int value)
        {
            if (_currentLifes-value >= 0)
            {
                _currentLifes-=value;

            }
            else
            {
                StartCoroutine(nameof(LevelTransition));
            }
            _lifebar.fillAmount = (float)((float)_currentLifes / (float)totalLifes);
        }
        
    }
}