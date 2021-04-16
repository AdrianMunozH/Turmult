using UnityEngine;
using UnityEngine.PlayerLoop;

/**
 * Buildmanger nach Singleton-Pattern
 */
public class BuildManager : MonoBehaviour
{
    public static BuildManager instance;
    private GameObject turretToBuild;
    private GameObject turretToBuildPreview;
    public GameObject standardTurretPrefab;
    public GameObject standardTurretPreviewPrefab;
    private bool _buildModeOn;


    void Awake()
    {
        if (instance != null)
        {
            Debug.LogError("Es gibt mehr als einen Buildmanager in der Szene: Bitte nur eine pro Szene!");
            return;
        }
        instance = this;
        _buildModeOn = false;
    }

    void Start()
    {
        //Setzt den Standard Turret
        turretToBuild = standardTurretPrefab;
        turretToBuildPreview = standardTurretPreviewPrefab;
    }

    public GameObject GetTurretToBuild()
    {
        return turretToBuild;
    }

    public GameObject getTurretToBuildPreview()
    {
        return turretToBuildPreview;
    }

    public bool IsBuildModeOn()
    {
        return _buildModeOn;
    }

    public void SetBuildMode()
    {
        _buildModeOn = !_buildModeOn;
    }
}