using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using MLAPI;
using MLAPI.Messaging;
using Turrets;
using UnityEngine;
using UnityEngine.UI;

namespace Field
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

    
        public HexCoordinates coordinates;
        int distance;

        public int index;
        public CellType type;

        public CellType Celltype
        {
            get => type;
            set => type = value;
        }

        public int spindex;
        private Color startColor;
        private GameObject turret;
        public GameObject previewTurret;
        private Ressource _ressource;

        public GameObject acquiredField;

        public HCell[] neighb;

        
        private BuildManager buildManager;

        public Ressource Ressource
        {
            get => _ressource;
            set => _ressource = value;
        }


        public bool hasBuilding;

        public bool HasBuilding
        {
            get => hasBuilding;
            set => hasBuilding = value;
        }

        // maps gehen nciht wegen unity :((
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
                
        /*
        [Header("Color Highlighting")]
        public Color hoverColorBuildMode = new Color(214.0f/255f, 200.0f/255f, 178.0f/255f,100.0f);
        public Color hoverColorAcquireMode = new Color(214.0f/255f, 200.0f/255f, 178.0f/255f,100.0f);

        [Header("Cell Color Setup - Acquired")]
        public Color colorAcquiredNeutral = new Color(214.0f/255f, 200.0f/255f, 178.0f/255f,100.0f);
        public Color colorAcquiredMountain = new Color(64.0f/255f, 60.0f/255f, 47.0f/255f,100.0f);
        public Color colorAcquiredSwamp = new Color(81.0f/255f, 143.0f/255f, 116.0f/255f,100.0f);
        public Color colorAcquiredForest = new Color(6.0f/255f, 84.0f/255f, 15.0f/255f,100.0f);

        [Header("Cell Color Setup - Not Acquired")]
        public Color colorUnacquiredNeutral = new Color(189.0f/255f, 186.0f/255f, 183.0f/255f,100.0f);
        public Color colorUnacquiredMountain = new Color(158f/255f, 153f/255f, 138f/255f,100f);
        public Color colorUnacquiredSwamp = new Color(102f/255f, 110f/255f, 106f/255f,100f);
        public Color colorUnacquiredForest = new Color(160f/255f, 176f/255f, 162f/255f,100f);
        
    
        [Header("Cell Color Setup - Not Acquired")]
        public Color colorNeutralField = new Color(56.0f/255f, 56.0f/255f, 56.0f/255f,100.0f);
        */
        //Optimization: Cachen des Renderes auf dem Objekt
        private Renderer rend;


        public Image gridImage;
        
        //bool ob es schon bebaut wurde 
        
        public CellType GetCellType()
        {
            return type;
        }

        public void SetCellType(CellType celltype)
        {
            type = celltype;
        }

        public Color SetColor(float r,float g,float b, float a)
        {
            var tempColor = new Color(r,g,b);
            tempColor.a = a;
            return tempColor;
        }
        
        
        
        public void BuildTurret()
        {
            Debug.Log("buildTurret    ");
            GameObject turretToBuild = buildManager.GetTurretToBuild();
            Vector3 turPos = transform.position;
            turret = (GameObject) Instantiate(turretToBuild, new Vector3(turPos.x, turPos.y - 10, turPos.z) , transform.rotation);
            Turret t = turret.GetComponent<Turret>();
            
            // Animation
            var sequence = DOTween.Sequence();
            sequence.Append(turret.transform.DOLocalMoveY(turPos.y, 0.5f));
            sequence.Append(turret.transform.DOShakeScale( 1f, new Vector3(0f, 0.01f, 0f), 5, 0, fadeOut:true));
            //
            
            
            SetPrefab((int) t.ressourceType + 2);
            hasBuilding = true;
            
        }

        private void Awake()
        {
            type = CellType.Hidden;
            
        }

        public GameObject InstantiateTurretPreview(GameObject turretToBuild)
        {
            return Instantiate(turretToBuild, transform.position, transform.rotation); 
        }

        public void DestroyPreviewTurret()
        {
            Destroy(previewTurret);
        }        
        
        // Start is called before the first frame update
        void Start()
        {
            buildManager = BuildManager.instance;
            
            
            _ressource = new Ressource();
            rend = GetComponent<Renderer>();
            rend.enabled = false;
            
            SetNeighb();
            

            gridImage.color = SetColor(0f, 0f, 0f, 0f);
            //SetCellColor();
            if (type != CellType.Base) ;
                // SetPrefab(type,_ressource.GetRessourceType());
        }
        
        private void SetNeighb()
        {
            //Debug.Log(h.coordinates.ToString() + " ausgewählt");
            neighb = new HCell[6];
            int i = 0;
            foreach (HCell cell in HGrid.Instance.cells)
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

        public void SetUnacquiredPrefab(HCell cell, CellType cellType, Ressource.RessourceType ressource, Vector3? rotation = null,
            int path = 0)
        {

            if (cell.transform.childCount == 0)
            {
                
                cell.SetCellType(CellType.CanBeAcquired);
                int index = (int) cellType + (int) ressource + path;
            
                Vector3 pos = transform.position;
                GameObject hexagon = Instantiate(hexPrefabs[index], new Vector3(pos.x,pos.y -10, pos.z), transform.rotation);
                hexagon.transform.DOLocalMoveY(pos.y, 0.5f);
                

                if(rotation != null)
                    hexagon.transform.eulerAngles = new Vector3(hexagon.transform.eulerAngles.x + rotation.Value.x,
                        hexagon.transform.eulerAngles.y + rotation.Value.y,
                        hexagon.transform.eulerAngles.z + rotation.Value.z);
            
                hexagon.transform.SetParent(transform, true);
            }
        }
/*
        [ServerRpc(RequireOwnership = false)]
        public void AcquiredThisCellServerRpc()
        {
            // sagt allen clients bescheid das das feld eingenommen wurde
            AcquiredThisCellClientRpc();
        }
        [ClientRpc]
        public void AcquiredThisCellClientRpc()
        {
            SetPrefab(type,_ressource.GetRessourceType());
        }
        */

        public void SetPrefab(int prefabIndex, Vector3? rotation = null)
        {
            
            if(transform.childCount > 0)
                GameObject.Destroy(transform.GetChild(0).gameObject);
            
            GameObject hexagon = Instantiate(hexPrefabs[prefabIndex], transform.position, transform.rotation);
            
            
            if(rotation != null)
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
        
        public void SetPrefab(CellType cellType, Ressource.RessourceType ressource, Vector3? rotation = null,int path = 0)
        {
            int index = (int) cellType + (int) ressource + path;
            
            if(transform.childCount > 0)
                GameObject.Destroy(transform.GetChild(0).gameObject);
            
            Vector3 pos = transform.position;
            GameObject hexagon = Instantiate(hexPrefabs[index], new Vector3(pos.x,pos.y -10, pos.z), transform.rotation);
            hexagon.transform.DOLocalMoveY(pos.y, 0.5f);

            if(rotation != null)
                hexagon.transform.eulerAngles = new Vector3(hexagon.transform.eulerAngles.x + rotation.Value.x,
                    hexagon.transform.eulerAngles.y + rotation.Value.y,
                    hexagon.transform.eulerAngles.z + rotation.Value.z);
            
            hexagon.transform.SetParent(transform, true);
            
            //Ressourcenfelder ohne Wege werden random gedreht, bei Grasfelder zusätzlich das Material random angepasst
            if (path == 0)
            {
                if (ressource == Ressource.RessourceType.Neutral)
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
                if (cell.GetCellType() == CellType.Hidden)
                {
                    cell.SetCellType(CellType.CanBeAcquired);
                    if (_ressource.GetRessourceType()  != Ressource.RessourceType.Neutral)
                    {
                        cell._ressource.SetSpecificType(Ressource.RessourceType.Neutral);
                    }
                    cell.SetUnacquiredPrefab(cell, cell.GetCellType(),cell._ressource.GetRessourceType());
                }
            }
        }

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
            int randomDirection = Random.Range(0, 6);

            switch (randomDirection)
            {
                case 0:
                    break;
                case 1:
                    field.transform.Rotate(0,60,0);
                    break;
                case 2:
                    field.transform.Rotate(0,120,0);
                    break;
                case 3:
                    field.transform.Rotate(0,180,0);
                    break;
                case 4:
                    field.transform.Rotate(0,240,0);
                    break;
                case 5:
                    field.transform.Rotate(0,300,0);
                    break;
            }
        }

       
    }
}