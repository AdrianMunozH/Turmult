using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class HCell : MonoBehaviour
{
    public HexCoordinates coordinates;
    int distance;

    public int index;
    public bool isAcquired;
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


    //Optimization: Cachen des Renderes auf dem Objekt
    private Renderer rend;
    //bool ob es schon bebaut wurde 

    private void OnMouseEnter()
    {
        //Hier Turrets
        //Nur wenn der Buildmode eingeschaltet ist, werden previews angezeigt
        if (BuildManager.instance.IsBuildModeOn() && isAcquired)
        {
            rend.material.color = hoverColorBuildMode;
            GameObject turretToBuild = BuildManager.instance.getTurretToBuildPreview();
            if (turretToBuild != null)
            {
                previewTurret = (GameObject) Instantiate(turretToBuild, transform.position, transform.rotation);
            }
        //Hier Land
        }else if (BuildManager.instance.istAcquireModeOn())
        {
            rend.material.color = isAcquired?GetAcquiredColor():hoverColorAcquireMode;
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
            }else if (!isAcquired)
            {
                Debug.Log("Bratan, Feld einnehmen!");
                return;
            }

            //Bauen des Turms
            GameObject turretToBuild = BuildManager.instance.GetTurretToBuild();
            turret = (GameObject) Instantiate(turretToBuild, transform.position, transform.rotation);
            hasBuilding = true;
        }else if (BuildManager.instance.istAcquireModeOn())
        {
            if (isAcquired)
            {
                Debug.Log("Das Feld wurde doch schon eingenommen ...");
            }
            else
            {
                isAcquired = true;
                SetInitialColor();
            }
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        _ressource = new Ressource();
        rend = GetComponent<Renderer>();
        SetInitialColor();
    }

    //Setzt das Initiale Material für die Zelle
    private void SetInitialColor()
    {
        switch (_ressource.GetRessourceType())
            {
                case Ressource.RessourceType.Berg:
                    startColor = isAcquired?colorAcquiredMountain:colorUnacquiredMountain;
                    break;
                case Ressource.RessourceType.Sumpf:
                    startColor = isAcquired?colorAcquiredSwamp:colorUnacquiredSwamp;
                    break;
                case Ressource.RessourceType.Wald:
                    startColor = isAcquired?colorAcquiredForest:colorUnacquiredForest;
                    break;
                case Ressource.RessourceType.Neutral:
                    startColor = isAcquired?colorAcquiredNeutral:colorUnacquiredNeutral;
                    break;
            }

        rend.material.color = startColor;
    }

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