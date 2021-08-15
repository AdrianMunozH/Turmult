using System;
using System.Collections;
using System.Collections.Generic;
using Singleplayer.Player;
using Singleplayer.Enemies;
using Singleplayer.Turrets;
using Singleplayer.Ui.Input;
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
            PlayerInputManager.Instance.SetState(new BuildState());

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
            if (_isAttacking)
            {
                spawnPoint.SpawnEnemy(minionPath.ToArray(), true);
            }else 
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
            Debug.Log(_hexGrid.ArrayToString(minionPath.ToArray()) );
            minionPath.RemoveRange(towerIndex,sp.Count-towerIndex);
            
        }
        /// <summary>
        /// Singleplayer modus: Nur wegberechnung für einen Spawnpoint
        /// </summary>
        public void SpawnEnemyWave()
        {
            _spawnCounter = 0;
            // könnte in der methode init werden (spath)
            /*
            spath = spawnPoint.Solve();
            List<HCell> sp = spawnPoint.RecPath(spath);
            minionPath = spawnPoint.ShortestPath(sp);
            for (int i = 0; i < minionsPerWave; i++)
            {
                StartCoroutine(nameof(SpawnEnemyWithDelay));
                _spawnCounter++;
            }
            */
            ///
            /// attack test
            /// 

            if (CalculatePath())
            {
                // normaler modus                Nicht angreifen
                
                _isAttacking = false;
                for (int i = 0; i < minionsPerWave; i++)
                {
                    StartCoroutine(nameof(SpawnEnemyWithDelay));
                    _spawnCounter++;
                }
            }
            
            else
            {
                // attacking modus
                CalulateAttack();
                _isAttacking = true;
                for (int i = 0; i < minionsPerWave; i++)
                {
                    StartCoroutine(nameof(SpawnEnemyWithDelay));
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
            CalculatePath();
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
    }
}