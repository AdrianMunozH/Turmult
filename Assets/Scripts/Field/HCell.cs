using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
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
        public int spindex;
        private Color startColor;
        private GameObject turret;
        private GameObject previewTurret;
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
        public LineRenderer lineRenderer;


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

        private Color SetColor(float r,float g,float b, float a)
        {
            var tempColor = new Color(r,g,b);
            tempColor.a = a;
            return tempColor;
        }

        private void OnMouseEnter()
        {
            //Hier Turrets
            //Nur wenn der Buildmode eingeschaltet ist, werden previews angezeigt
            if (buildManager.IsBuildModeOn() && type == CellType.Acquired && buildManager.GetTurretToBuild() != null)
            {
                gridImage.color = SetColor(214.0f/255f, 200.0f/255f, 178.0f/255f,70f/255f);
                GameObject turretToBuild = buildManager.GetTurretToBuildPreview();
                if (turretToBuild != null)
                {
                    previewTurret = (GameObject) Instantiate(turretToBuild, transform.position, transform.rotation);
                }
                //Hier Land
            }else if (BuildManager.instance.isAcquireModeOn() && type != CellType.Neutral && type != CellType.Acquired && type == CellType.CanBeAcquired)
            {
                gridImage.color = SetColor(225f,225f,225f,70f/255f);
                //rend.material.color = type == CellType.Acquired?GetAcquiredColor():hoverColorAcquireMode;
            }
        }
        
        private void OnMouseExit()
        {
            rend.material.color = startColor;
            if (BuildManager.instance.IsBuildModeOn())
            {
               
                //Zerstören des Preview Turrets
                if (previewTurret != null)
                {
                    Destroy(previewTurret);
                }

                
            }
            if(type == CellType.Acquired)
                gridImage.color = SetColor(5f/255f, 55f/255f, 18f/255f,40f/255f);
            gridImage.color = SetColor(0f, 0f, 0f, 0f);
        }

        private void OnMouseDown()
        {
            if (buildManager.IsBuildModeOn() && _ressource.GetRessourceType() == Ressource.RessourceType.Neutral)
            {
                if (buildManager.GetTurretToBuild() == null)
                    return;
                //Wenn Feld noch nicht bebaut ist
                if (turret != null)
                {
                    //TODO: Fehlerhandling anpassen
                    Debug.Log("Hier steht schon was Brudi!");
                    return;
                }else if (type != CellType.Acquired)
                {
                    Debug.Log("Bratan, Feld einnehmen!");
                    return;
                }
                //Bauen des Turms
                GameObject turretToBuild = buildManager.GetTurretToBuild();
                Vector3 turPos = transform.position;
                turret = (GameObject) Instantiate(turretToBuild, new Vector3(turPos.x, turPos.y - 10, turPos.z) , transform.rotation);
                Turret t = turret.GetComponent<Turret>();
                var sequence = DOTween.Sequence();
                sequence.Append(turret.transform.DOLocalMoveY(turPos.y, 0.5f));
                sequence.Append(turret.transform.DOShakeScale( 1f, new Vector3(0f, 0.01f, 0f), 5, 0, fadeOut:true));
                
                
                
                
                SetPrefab((int) t.ressourceType + 2);
                hasBuilding = true;
                Debug.Log("reroute turretmode");
                HGameManager.instance.rerouteEnemys(this);
                
            }else if (buildManager.isAcquireModeOn())
            {
                if (type == CellType.Acquired)
                {
                    Debug.Log("Das Feld wurde doch schon eingenommen ...");
                }else if (type == CellType.Neutral || type == CellType.Base)
                {
                    Debug.Log("Das Feld ist die Schweiz, lass es in Ruhe");
               
                }else if (type == CellType.Hidden)
                {
                    Debug.Log("Hidden");
                }
                else
                {
                    type = CellType.Acquired;
                    acquiredField = GameObject.Find("Cylinder");
                    HGameManager.instance.rerouteEnemys(this);
                    Debug.Log("reroute acquiremode");
                    //SetCellColor();
                    SetPrefab(type,_ressource.GetRessourceType());
                    StartCoroutine(CheckNeighb());
                    
                }
            }
        }

        private void Awake()
        {
            type = CellType.Hidden;
            
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
            foreach (HCell cell in buildManager.Grid.cells)
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
        IEnumerator CheckNeighb()
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

        //Setzt und aktualisiert die Materialen bzw. Farben einer Zelle
        private void SetCellColor()
        {
            //TODO SET COLOR VON HCELL ANSCHAUEN
            if (type != CellType.Neutral)
            {
                //startColor = colorUnacquiredNeutral;
                // UND DAS MINIONSSPAWNPROBLEM FIXEN
                switch (_ressource.GetRessourceType())
                {
                    case Ressource.RessourceType.Berg:
                        //Wenn  nocht nicht eignenommen
                        if (type == CellType.CanBeAcquired)
                        {
                            //startColor = colorUnacquiredMountain;
                            SetPrefab(0);
                            //Wenn eingenommen
                        }
                        else if (type == CellType.Acquired)
                        {
                            SetPrefab(1);
                            //startColor = colorAcquiredMountain;
                        }

                        break;
                    case Ressource.RessourceType.Sumpf:
                        if (type == CellType.CanBeAcquired)
                        {
                            //startColor = colorUnacquiredSwamp;
                            SetPrefab(2);
                        }
                        else if (type == CellType.Acquired)
                        {
                            SetPrefab(3);
                            //startColor = colorAcquiredSwamp;
                        }

                        break;
                    case Ressource.RessourceType.Wald:
                        if (type == CellType.CanBeAcquired)
                        {
                            //startColor = colorUnacquiredForest;
                            SetPrefab(4);
                        }
                        else if (type == CellType.Acquired)
                        {
                            SetPrefab(5);
                            //startColor = colorAcquiredForest;
                        }

                        break;
                    default:
                        if (type == CellType.CanBeAcquired)
                        {
                            //startColor = colorUnacquiredNeutral;
                            SetPrefab(7);
                        }
                        else if (type == CellType.Acquired)
                        {
                            SetPrefab(6);
                        }

                        break;
                }
            }
            else
            {
                //Neutrale Felder
                //startColor = colorNeutralField;
                SetPrefab(8);
            }

            rend.material.color = startColor;
        }
        

        /**
     * Gibt die Farbe des eingenommenen Zustands zurück
     
        private Color GetAcquiredColor()
        {
            switch (_ressource.GetRessourceType())
            {
                case Ressource.RessourceType.Berg:
                    return colorAcquiredMountain;
                case Ressource.RessourceType.Sumpf:
                    return colorAcquiredSwamp;
                case Ressource.RessourceType.Wald:
                    return colorAcquiredForest;
                default:
                    return colorAcquiredNeutral;
            }
        }
        */

        // Update is called once per frame
        void Update()
        {
            
        }
    }
}