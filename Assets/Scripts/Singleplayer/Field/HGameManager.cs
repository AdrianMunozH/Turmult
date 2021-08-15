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
        [Header("UI")] 
        public GameObject timeBar;
        public GameObject lifeBar;
        private static Image _timebar;
        private static Image _lifebar;
        private TextMeshProUGUI _timebarLabel;
        public GameObject gameOver;
        
//Todo: Lifes eigentlich an den Player auslagern
        [Header("Lifes")] 
        public int totalLifes = 20;
        private int _currentLifes;
        
        //WaveSpawning
        [Header("Spawning")] public EnemySpawn spawnPoint;
        public int waves = 50;
        public float timeBetweenMinionSpawn = 0.8f;
        public int minionsPerWave = 5;
        private static int _currentWave = 0;
        private List<HCell> minionPath;
        public static int timeTillSceneChange = 15;
        //setzt die alten SP wege zurück
        private HCell[] _lastShortestPath;

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
            _timebarLabel = _timebar.GetComponentInChildren<TextMeshProUGUI>();
            _timebarLabel.text = "1";
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

            HCell[] temp = spawnPoint.ShortestPath(sp).ToArray();
            _hexGrid.ShortestPathPrefabs(temp);
            _lastShortestPath = temp;
            StartCoroutine(LevelFadeIn());

        }

        private void Update()
        {
            //Spiel läuft
            if (_currentLifes > 0)
            {
                // Buildstate und Time nicht abgelaufen
                if (_timer > 0 && !PlayerInputManager.Instance.GetState().name.Equals(StateEnum.Battle))
                {
                    _timer -= Time.deltaTime;
                    _timebar.fillAmount = (_timer / buildingPhaseTimer);

                    //wenn Zeit abgelaufen -> Battlephase
                    if (_timer <= 0)
                    {
                        PlayerInputManager.Instance.SetState(new BattleState());
                    }
                }

                if (PlayerInputManager.Instance.GetState().name.Equals(StateEnum.Battle))
                {
                    if (_currentWave < waves && !allMinionsSpawned)
                    {
                        SpawnEnemyWave();
                        _currentWave++;
                    }
                    
                    //Zurück zu Buildstate!
                    if (allMinionsSpawned && spawnPoint.enemys.Count == 0)
                    {
                        allMinionsSpawned = false;
                        IncomeManager.Instance.Interest();
                        PlayerInputManager.Instance.SetState(new BuildState());
                        _timer = buildingPhaseTimer;
                        _timebarLabel.text = (_currentWave).ToString();
                    }
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

        IEnumerator LevelFadeIn()
        {
            transition.SetTrigger("Start");
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
            // wenns null ist brauchen wir nicht aufräumen
            if(_lastShortestPath == null) return;

            foreach (var lastSP in _lastShortestPath)
            {
                
                if (!IsInArray(lastSP, minionPath.ToArray()))
                {
                    lastSP.SetPrefab(lastSP.Celltype,lastSP.resource.GetResource());
                }
            }
            
            
            // am ende setzten wir den aktuellen kürzesten weg auf lastShortestPath
            _lastShortestPath = minionPath.ToArray();
        }

        private bool IsInArray(HCell hCell,HCell[] arr)
        {
            foreach (var currentCell in arr)
            {
                if (hCell.coordinates.CompareCoord(currentCell.coordinates))
                    return true;
            }

            return false;

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