using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HCell : MonoBehaviour
{
    public HexCoordinates coordinates;
    int distance;

    public int index;
    public bool isAcquired;
    public int spindex;
    public Color hoverColor;
    private Color startColor;
    private GameObject turret;
    private GameObject previewTurret;


    //Optimization: Cachen des Renderes auf dem Objekt
    private Renderer rend;
    //bool ob es schon bebaut wurde 

    private void OnMouseEnter()
    {
        //Nur wenn der Buildmode eingeschaltet ist, werden previews angezeigt
        if (BuildManager.instance.IsBuildModeOn())
        {
            GetComponent<Renderer>().material.color = hoverColor;
            GameObject turretToBuild = BuildManager.instance.getTurretToBuildPreview();
            if (turretToBuild != null)
            {
                previewTurret = (GameObject) Instantiate(turretToBuild, transform.position, transform.rotation);
            }
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
                Debug.Log("Hier steht schon was Brudi");
            }

            //Bauen des Turms
            GameObject turretToBuild = BuildManager.instance.GetTurretToBuild();
            turret = (GameObject) Instantiate(turretToBuild, transform.position, transform.rotation);
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        rend = GetComponent<Renderer>();
        startColor = rend.material.color;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
