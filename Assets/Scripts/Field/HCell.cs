using UnityEngine;
using UnityEngine.UI;

namespace Field
{
    public class HCell : MonoBehaviour
    {
        public enum CellType
        {
            CanBeAcquired,
            Acquired,
            Neutral
        };

    
        public HexCoordinates coordinates;
        int distance;

        public int index;
        private CellType type;
        public int spindex;
        private Color startColor;
        private GameObject turret;
        private GameObject previewTurret;
        private Ressource _ressource;
        public bool hasBuilding;

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
        [SerializeField] private GameObject[] unacquiredHex;
    
        [Header("Cell Color Setup - Not Acquired")]
        public Color colorNeutralField = new Color(56.0f/255f, 56.0f/255f, 56.0f/255f,100.0f);

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
            if (BuildManager.instance.IsBuildModeOn() && type == CellType.Acquired)
            {
                gridImage.color = SetColor(214.0f/255f, 200.0f/255f, 178.0f/255f,30f);
                GameObject turretToBuild = BuildManager.instance.getTurretToBuildPreview();
                if (turretToBuild != null)
                {
                    previewTurret = (GameObject) Instantiate(turretToBuild, transform.position, transform.rotation);
                }
                //Hier Land
            }else if (BuildManager.instance.isAcquireModeOn() && type != CellType.Neutral && type != CellType.Acquired)
            {
                gridImage.color = SetColor(225f,225f,225f,30f);
                rend.material.color = type == CellType.Acquired?GetAcquiredColor():hoverColorAcquireMode;
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
            gridImage.color = SetColor(5f/255f, 55f/255f, 18f/255f,40f);
        }

        private void OnMouseDown()
        {
            if (BuildManager.instance.IsBuildModeOn())
            {
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
                GameObject turretToBuild = BuildManager.instance.GetTurretToBuild();
                turret = (GameObject) Instantiate(turretToBuild, transform.position, transform.rotation);
                hasBuilding = true;
                Debug.Log("reroute turretmode");
                HGameManager.Instance.rerouteEnemys(this);
            }else if (BuildManager.instance.isAcquireModeOn())
            {
                if (type == CellType.Acquired)
                {
                    Debug.Log("Das Feld wurde doch schon eingenommen ...");
                }else if (type == CellType.Neutral)
                {
                    Debug.Log("Das Feld ist die Schweiz, lass es in Ruhe");
               
                }
                else
                {
                    type = CellType.Acquired;
                    HGameManager.Instance.rerouteEnemys(this);
                    Debug.Log("reroute acquiremode");
                    SetCellColor();
                }
            }
        }

        private void Awake()
        {
            type = CellType.CanBeAcquired;
        }

        // Start is called before the first frame update
        void Start()
        {
            _ressource = new Ressource();
            rend = GetComponent<Renderer>();
            SetCellColor();
        }

        //Setzt und aktualisiert die Materialen bzw. Farben einer Zelle
        private void SetCellColor()
        {
            //TODO SET COLOR VON HCELL ANSCHAUEN
            if (type != CellType.Neutral)
            {
                startColor = colorUnacquiredNeutral;
                // UND DAS MINIONSSPAWNPROBLEM FIXEN
                switch (_ressource.GetRessourceType())
                {
                    case Ressource.RessourceType.Berg:
                        //Wenn  nocht nicht eignenommen
                        if (type == CellType.CanBeAcquired)
                        {
                            startColor = colorUnacquiredMountain;
                            GameObject hexagon = Instantiate(unacquiredHex[1], transform.position, transform.rotation);
                            hexagon.transform.SetParent(transform, true);
                            //Wenn eingenommen
                        }
                        else if (type == CellType.Acquired)
                        {
                            startColor = colorAcquiredMountain;
                        }

                        break;
                    case Ressource.RessourceType.Sumpf:
                        if (type == CellType.CanBeAcquired)
                        {
                            startColor = colorUnacquiredSwamp;
                            GameObject hexagon = Instantiate(unacquiredHex[2], transform.position, transform.rotation);
                            hexagon.transform.SetParent(transform, true);
                        }
                        else if (type == CellType.Acquired)
                        {
                            startColor = colorAcquiredSwamp;
                        }

                        break;
                    case Ressource.RessourceType.Wald:
                        if (type == CellType.CanBeAcquired)
                        {
                            startColor = colorUnacquiredForest;
                            GameObject hexagon = Instantiate(unacquiredHex[3], transform.position, transform.rotation);
                            hexagon.transform.SetParent(transform, true);
                        }
                        else if (type == CellType.Acquired)
                        {
                            startColor = colorAcquiredForest;
                        }

                        break;
                    default:
                        if (type == CellType.CanBeAcquired)
                        {
                            startColor = colorUnacquiredNeutral;
                            GameObject hexagon = Instantiate(unacquiredHex[0], transform.position, transform.rotation);
                            hexagon.transform.SetParent(transform, true);
                        }
                        else if (type == CellType.Acquired)
                        {
                            startColor = colorAcquiredNeutral;
                        }

                        break;
                }
            }
            else
            {
                //Neutrale Felder
                startColor = colorNeutralField;
                GameObject hexagon = Instantiate(unacquiredHex[0], transform.position, transform.rotation);
                hexagon.transform.SetParent(transform, true);
            }

            rend.material.color = startColor;
        }

        /**
     * Gibt die Farbe des eingenommenen Zustands zurück
     */
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

        // Update is called once per frame
        void Update()
        {
        }
    }
}