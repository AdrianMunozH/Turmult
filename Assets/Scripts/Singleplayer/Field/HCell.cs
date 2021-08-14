using System;
using System.Collections;
using DG.Tweening;
using Singleplayer.Turrets;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

namespace Singleplayer.Field
{
    public class HCell : MonoBehaviour
    {
        public enum CellType
        {
            CanBeAcquired = 0,
            Acquired = 1,
            Neutral = 4,
            Base,
            Hidden = 6
        };
        //######## public ###########
        public HexCoordinates coordinates;
        public int index;
        [SerializeField] private CellType type = CellType.Hidden;
        
        //Wird gebraucht für das Tweening beim Einnehmen der Zelle
        //Wird in Buildstate.cs auf true gesetzt
        [HideInInspector]public bool recentlyBuild;
        [HideInInspector] public GameObject acquiredField;
        public HCell[] neighb;
        public Resource resource;
        public GameObject resPrefab;
        public GameObject previewTurret;
        
        public bool hasBuilding;
        public bool HasBuilding
        {
            get => hasBuilding;
            set => hasBuilding = value;
        }
        public CellType Celltype
        {
            get => type;
            set => type = value;
        }
        
        public Image gridImage;
        public int spindex;
        
        //######## private ###########
        private Color startColor;
        private GameObject turret;
        private BuildManager buildManager;
        private int distance;
        private Renderer rend;
        private HGrid _hexGrid;
        [SerializeField] private GameObject[] hexPrefabs;
        
        
        /*
        berg    unacq      :0
                acq        :1
                towerbase  :2
                straight   :3
                corner     :4
        sumpf   unacq      :5
                acq        :6
                towerbase  :7
                straight   :8
                corner     :9
        wald   unacq       :10
                acq        :11
                towerbase  :12
                straight   :13
                corner     :14
        neutral acq/unacq  :15
                straight   :16
                corner     :17
        portal  mid        :18
                site       :19
                path       :20
        base               :21
                
*/
        //Optimization: Cachen des Renderes auf dem Objekt

        //bool ob es schon bebaut wurde 

        public void SetCellType(CellType celltype)
        {
            type = celltype;
        }

        public Color SetColor(float r, float g, float b, float a)
        {
            var tempColor = new Color(r, g, b);
            tempColor.a = a;
            return tempColor;
        }

        public void DestroyPreviewTurret()
        {
            Destroy(previewTurret);
        }

        public GameObject InstantiateTurretPreview(GameObject previewTurret)
        {
           return Instantiate(previewTurret,transform.position,transform.rotation);
        }

        public void BuildTurret()
        {

            if (buildManager.GetTurretToBuild() != null)
            {
                GameObject turretToBuild = buildManager.GetTurretToBuild();
                Vector3 turPos = transform.position;
                turret = (GameObject) Instantiate(turretToBuild, new Vector3(turPos.x, turPos.y - 10, turPos.z),
                    transform.rotation);
                Turret t = turret.GetComponent<Turret>();

                // Animation
                var sequence = DOTween.Sequence();
                sequence.Append(turret.transform.DOLocalMoveY(turPos.y, 0.5f));
                sequence.Append(turret.transform.DOShakeScale(1f, new Vector3(0f, 0.01f, 0f), 5, 0, fadeOut: true));

                SetPrefab((int) t.ressourceType + 2);
                hasBuilding = true;
            }
        }



        private void Awake()
        {
            _hexGrid = GameObject.Find("HexGrid").GetComponent<HGrid>();
            if (_hexGrid == null) throw new Exception("Kein Objekt HexGrid in der Szene gefunden oder es keine Komponente HGrid an diese! ");
            
            //Setzen der Ressource standardmäßig auf Neutral, Berechnung am Server
            resource =  new Resource();
        }

        // Start is called before the first frame update
        void Start()
        {

            buildManager = BuildManager.instance;
            rend = GetComponent<Renderer>();
            rend.enabled = false;
            SetNeighb();
            gridImage.color = SetColor(0f, 0f, 0f, 0f);
        }
        

        private void SetNeighb()
        {
            neighb = new HCell[6];
            int i = 0;
            foreach (HCell cell in _hexGrid.cells)
            {
                if (cell.coordinates.X == coordinates.X - 1 && cell.coordinates.Y == coordinates.Y)
                {
                    neighb[i] = cell;
                    i++;
                }
                else if (cell.coordinates.X == coordinates.X && cell.coordinates.Y == coordinates.Y - 1)
                {
                    neighb[i] = cell;
                    i++;
                }
                else if (cell.coordinates.X == coordinates.X + 1 && cell.coordinates.Y == coordinates.Y)
                {
                    neighb[i] = cell;
                    i++;
                }
                else if (cell.coordinates.X == coordinates.X && cell.coordinates.Y == coordinates.Y + 1)
                {
                    neighb[i] = cell;
                    i++;
                }
                else if (cell.coordinates.X == coordinates.X - 1 && cell.coordinates.Y == coordinates.Y + 1)
                {
                    neighb[i] = cell;
                    i++;
                }
                else if (cell.coordinates.X == coordinates.X + 1 && cell.coordinates.Y == coordinates.Y - 1)
                {
                    neighb[i] = cell;
                    i++;
                }
            }
        }

        public void SetUnacquiredPrefab(HCell cell, CellType cellType, Resource.ResourceType ressource,
            Vector3? rotation = null,
            int path = 0)
        {
            int index = (int) cellType + (int) ressource + path;

                Vector3 pos = transform.position;
                GameObject hexagon = Instantiate(hexPrefabs[index], new Vector3(pos.x, pos.y - 10, pos.z),
                    transform.rotation);
                hexagon.transform.DOLocalMoveY(pos.y, 0.5f);


                if (rotation != null)
                    hexagon.transform.eulerAngles = new Vector3(hexagon.transform.eulerAngles.x + rotation.Value.x,
                        hexagon.transform.eulerAngles.y + rotation.Value.y,
                        hexagon.transform.eulerAngles.z + rotation.Value.z);

                hexagon.transform.SetParent(transform, true);

        }

        public void SetPrefab(int prefabIndex, Vector3? rotation = null)
        {
            //TODO: Löschen altes Prefab?
            if (transform.childCount > 1)
            {
                GameObject.Destroy(transform.GetChild(1).gameObject);
            }

           GameObject hexagon = Instantiate(hexPrefabs[prefabIndex], transform.position, transform.rotation);


            if (rotation != null)
                hexagon.transform.eulerAngles = new Vector3(hexagon.transform.eulerAngles.x + rotation.Value.x,
                    hexagon.transform.eulerAngles.y + rotation.Value.y,
                    hexagon.transform.eulerAngles.z + rotation.Value.z);

            hexagon.transform.SetParent(transform, true);
            if (acquiredField != null)
            {
                var acquire = hexagon.gameObject.transform.Find("Cylinder");
                acquiredField = acquire.gameObject;
                acquiredField.SetActive(true);
            }

            StartCoroutine(CheckNeighb());
        }

        public void SetPrefab(CellType cellType, Resource.ResourceType ressource, Vector3? rotation = null,
            int path = 0)
        {
            Debug.Log(cellType);
            int index = (int) cellType + (int) ressource + path;

            //TODO: Altes Prefab löschen
            
            if (transform.childCount > 0)
            {
                GameObject.Destroy(transform.GetChild(0).gameObject);
            }
            
            GameObject hexagon;
            Vector3 pos = transform.position;

            //TODO: Tweening der Zellen anpassen, die unaquired sind. Leider wird das Acquired vor Aufruf dieser Zeile gesetzt
            //Tweening nur für Zellen die noch nicht vorhanden sind
            if (cellType != CellType.Acquired || recentlyBuild)
            {

                hexagon = Instantiate(hexPrefabs[index], new Vector3(pos.x, pos.y - 10, pos.z), transform.rotation);
                hexagon.transform.DOLocalMoveY(pos.y, 0.5f);
                recentlyBuild = false;
            }
            else
            {
                hexagon = Instantiate(hexPrefabs[index], new Vector3(pos.x, pos.y, pos.z), transform.rotation);
            }

            if (rotation != null)
                hexagon.transform.eulerAngles = new Vector3(hexagon.transform.eulerAngles.x + rotation.Value.x,
                    hexagon.transform.eulerAngles.y + rotation.Value.y,
                    hexagon.transform.eulerAngles.z + rotation.Value.z);

            hexagon.transform.SetParent(transform, true);

            //Ressourcenfelder ohne Wege werden random gedreht, bei Grasfelder zusätzlich das Material random angepasst
            if (path == 0)
            {
                if (ressource == Resource.ResourceType.Neutral)
                    SetRandomField(hexagon, true);
                else
                {
                    SetRandomField(hexagon, false);
                }
            }

            //Setzt "Eingenommen Shader" und aktiviert ihn
            //TODO Material je nach Spieler anpassen
            var acquire = hexagon.gameObject.transform.Find("Cylinder");
            acquiredField = acquire.gameObject;
            acquiredField.SetActive(true);
            if (type == CellType.Hidden)
            {
                hexagon.SetActive(false);
            }

            StartCoroutine(CheckNeighb());
        }

        //Sucht Nachbarfelder nach "Hidden" Feldern ab und setzt Prefab.
        //Ist ausgewähltes Feld Ressourcenfeld, werden Nachbarfelder Neutral gesetzt
        public IEnumerator CheckNeighb()
        {

            yield return new WaitForSeconds(0.2f);
            foreach (var cell in neighb)
            {
                if (cell.Celltype == CellType.Hidden)
                {
                    cell.SetCellType(CellType.CanBeAcquired);
                    cell.SetUnacquiredPrefab(cell, cell.Celltype,   cell.resource.GetResource());
                }
            }
        }
        
        //Für Rotation und Material an den Grasfeldern
        void SetRandomField(GameObject field, bool neutral)
        {
            //Grasfeld Material ändern
            if (neutral)
            {
                int randomMaterial = Random.Range(0, 10);

                if (randomMaterial < 4)
                    field.GetComponent<MeshRenderer>().material =
                        buildManager.Grid.FieldMaterial[Random.Range(0, buildManager.Grid.FieldMaterial.Length)];
            }

            //Rotation ändern
            field.transform.Rotate(0, 60*Random.Range(0, 6), 0);
        }
        
        
    }
}