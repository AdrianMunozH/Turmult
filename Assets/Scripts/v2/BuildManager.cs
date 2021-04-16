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
    private bool _acquireModeOn;

    void Awake()
    {
        if (instance != null)
        {
            Debug.LogError("Es gibt mehr als einen Buildmanager in der Szene: Bitte nur eine pro Szene!");
            return;
        }

        instance = this;
        _buildModeOn = false;
        _acquireModeOn = false;
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

    public bool istAcquireModeOn()
    {
        return _acquireModeOn;
    }

    public void SetBuildMode()
    {
        _buildModeOn = !_buildModeOn;
        if (_acquireModeOn && _buildModeOn)
        {
            _acquireModeOn = false;
        }
    }

    public void SetAcquireMode()
    {
        _acquireModeOn = !_acquireModeOn;
        if (_acquireModeOn && _buildModeOn)
        {
            _buildModeOn = false;
        }
    }
}