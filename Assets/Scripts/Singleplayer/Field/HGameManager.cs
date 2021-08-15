using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Singleplayer.Player;
using Singleplayer.Enemies;
using Singleplayer.Turrets;
using Singleplayer.Ui.Input;
using TMPro;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.SceneManagement;
using Image = UnityEngine.UI.Image;
using Random = UnityEngine.Random;

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
        
        [Header("Enemies")]
        public List<int> sentEnemiesPrefabId;
        public int IncomeFromSentMinions = 0;

        private int _overAllMinions = 0;
        //SentEnemiesPrefab befüllen mit den normalen Waveminions + den gesendeten
        //IncomeFromSentMinoins erhöhen
        //Minions in EnemySpawn an richtiger Stelle hinterlegen
        
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
        //Liste aller zu senden Units: (PrefabId)
        private List<int> spawningList;
        //Wenn Minions gesendet werden, hält diese Variable die gesamte Anzahl der gesendeten Minions
        private int _spawnCounter = 0;
        //Ohne diesen bool wird die Wave doppelt gespawnt
        private bool _alreadySentState = false;

        //Wird benötigt für das checken, ob alle Minions getötet wurden
        private bool allMinionsSpawned;

        private float _timer;
        private HCell[] spath;
        private HGrid _hexGrid;
        int input;

        //sollte raus
        HCell start;
        HCell end;
        private bool _isAttacking;

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
            //Initiales setzen des Timers
            _timer = firstBuildingPhaseTimer;
            PlayerInputManager.Instance.SetState(new AcquireState());

            TimeTickSystem.OnTick += delegate(object sender, TimeTickSystem.OnTickEventArgs args) { };

            spawnPoint = Instantiate(spawnPoint);
            spawnPoint.end = _hexGrid.GetHCellByXyzCoordinates(distanceFromSpawn, 0, -distanceFromSpawn);
            spawnPoint.defaultStart = _hexGrid.GetHCellByXyCoordinates(0, 0);

            spath = spawnPoint.Solve();
            List<HCell> sp = spawnPoint.RecPath(spath);

            foreach (HCell hcell in sp)
            {
                //Resource wird im shortestpath benötigt!
                if (hcell.Celltype != HCell.CellType.Base)
                {
                    
                    hcell.Celltype = HCell.CellType.Acquired;
                    hcell.resource.SetSpecificType(Resource.ResourceType.Neutral);
                }
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
                        PlayerInputManager.Instance.BattleStateOn();
                    }
                }

                if (PlayerInputManager.Instance.GetState().name.Equals(StateEnum.Battle))
                {

                    if (_currentWave < waves && !allMinionsSpawned && !_alreadySentState)
                    {
                        _alreadySentState = true;
                        SpawnEnemyWave();
                        _currentWave++;
                    }
                    
                    //Zurück zu Buildstate!
                    if (allMinionsSpawned && spawnPoint.enemys.Count == 0)
                    {
                        _alreadySentState = false;
                        sentEnemiesPrefabId.Clear();
                        allMinionsSpawned = false;
                        IncomeManager.Instance.Interest();
                        IncomeManager.Instance.IncreasePlayerGold(IncomeFromSentMinions);
                        PlayerInputManager.Instance.BuildAndAcquireBlocked = false;
                        PlayerInputManager.Instance.AcquireModeOn();
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
        

        IEnumerator SpawnEnemyWithDelay(int prefabId)
        {
            yield return new WaitForSeconds(timeBetweenMinionSpawn * _spawnCounter);
            // es muss gecheckt werden ob die weglänge grö0er als 0 ist
            if (_isAttacking)
            {
                spawnPoint.SpawnEnemy(minionPath.ToArray(), true, prefabId);
            }else 
                spawnPoint.SpawnEnemy(minionPath.ToArray(), false,prefabId);
            
            if (_spawnCounter == _overAllMinions) allMinionsSpawned = true;
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

        public List<HCell> ArrayToList(HCell[] arr)
        {
            List<HCell> list = new List<HCell>();
            foreach (var currentCell in arr)
            {
                list.Add(currentCell);
            }
            return list;
        }

        public bool CalculatePath()
        {
            spath = spawnPoint.Solve();
            List<HCell> sp = spawnPoint.RecPath(spath);
            minionPath = spawnPoint.ShortestPath(sp);
            
            return minionPath.Count > 0;
        }

        public void CalulateAttack()
        {
            spath = spawnPoint.SolveAttack(_hexGrid.GetHCellByXyzCoordinates(0,0,0));
            // es wird erstmal der kürzeste weg zur base gesucht
            List<HCell> sp = spawnPoint.RecPath(spath);
            minionPath = spawnPoint.ShortestPath(sp);
                
            // danach wird am ersten turm gestoppt
            int towerIndex = (int) spawnPoint.TowerFinder(minionPath); // +1
            // von towerindex bis zum letzten element
            minionPath.RemoveRange(towerIndex,sp.Count-towerIndex);
            
        }
        /// <summary>
        /// Singleplayer modus: Nur wegberechnung für einen Spawnpoint
        /// </summary>
        public void SpawnEnemyWave()
        {
            _spawnCounter = 0;
            _overAllMinions = 0;

            spawningList = new List<int>();
            spawningList.Clear();
            
            for (int i = 0; i < minionsPerWave; i++)
            {
                //Erst einmal nur Typ 1 Minions in Standard waves!
                spawningList.Add(0);
            }
            
            spawningList.AddRange(sentEnemiesPrefabId);
            //Minions werden hier durcheinander geworfen, sodass Sie nicht immer in gleicher Reihenfolge starten
            for (int i = 0; i < spawningList.Count; i++) {
                int temp = spawningList[i];
                int randomIndex = Random.Range(i, spawningList.Count);
                spawningList[i] = spawningList[randomIndex];
                spawningList[randomIndex] = temp;
            }
            
            _overAllMinions = spawningList.Count;

            if (CalculatePath())
            {
                // normaler modus                Nicht angreifen
                
                _isAttacking = false;
                for (int i = 0; i < spawningList.Count; i++)
                {
                    StartCoroutine(nameof(SpawnEnemyWithDelay), spawningList[i]);
                    _spawnCounter++;

                }
            }
            else
            {
                // attacking modus
                CalulateAttack();
                _isAttacking = true;
                for (int i = 0; i < spawningList.Count; i++)
                {
                    StartCoroutine(nameof(SpawnEnemyWithDelay),spawningList[i]);
                    _spawnCounter++;
                }
            }


            HCell[] tempArr = minionPath.ToArray();
            ///
            ///
            /// attack test
            ///
            /*
            
            if (minionPath.Count == 0)
            {
                tempArr = _lastShortestPath;
                minionPath = ArrayToList(_lastShortestPath);
            }
            else
                tempArr = minionPath.ToArray();
                
            */
            _hexGrid.ShortestPathPrefabs(tempArr);

            // wenns null ist brauchen wir nicht aufräumen
            if(_lastShortestPath == null) return;

            foreach (var lastSP in _lastShortestPath)
            {
                
                if (!IsInArray(lastSP, tempArr) && lastSP.Celltype != HCell.CellType.Base)
                {
                    lastSP.SetPrefab(lastSP.Celltype,lastSP.resource.GetResource());
                }
            }
            
            
            // am ende setzten wir den aktuellen kürzesten weg auf lastShortestPath
            _lastShortestPath = tempArr;
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
    
        public void TowerDestroyed(HexCoordinates coordinates)
        {
            _hexGrid.GetHCellByXyCoordinates(coordinates.X, coordinates.Y).hasBuilding = false;
            
            for (int i = 0; i < spawnPoint.enemys.Count; i++)
            {
                EnemyMovement temp = spawnPoint.enemys[i].GetComponent<EnemyMovement>();
                _isAttacking = false;
                temp.isAttacking = false;
                spawnPoint.RebuildPath(i,temp.pathIndex);
                
            }


            // nur für die prefabs
            if(!CalculatePath())
                CalulateAttack();
            
            HCell[] tempArr = minionPath.ToArray();
            _hexGrid.ShortestPathPrefabs(tempArr);

            // wenns null ist brauchen wir nicht aufräumen
            if (_lastShortestPath == null) return;

            foreach (var lastSP in _lastShortestPath)
            {
                if (!IsInArray(lastSP, tempArr) && lastSP.Celltype != HCell.CellType.Base)
                {
                    lastSP.SetPrefab(lastSP.Celltype, lastSP.resource.GetResource());
                }
            }

            // am ende setzten wir den aktuellen kürzesten weg auf lastShortestPath
            _lastShortestPath = tempArr;
        }

        public void SendMinion(int value)
        {
            sentEnemiesPrefabId.Add(value);
            IncomeFromSentMinions += 1;
        }
        
        
        public string ListToString(List<int> list)
        {
            string s = "";
            foreach (int i in list)
            {
                //s += h.coordinates.ToString() + "\n";
                s += i + ", ";
            }
            
            return s;
        }
    }
}