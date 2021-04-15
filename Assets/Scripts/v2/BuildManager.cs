using UnityEngine;

/**
 * Buildmanger nach Singleton-Pattern
 */
public class BuildManager : MonoBehaviour
{
    public static BuildManager instance;
    private GameObject turretToBuild;
    public GameObject standardTurretPrefab;

    void Awake()
    {
        if (instance != null)
        {
            Debug.LogError("Es gibt mehr als einen Buildmanager in der Szene: Bitte nur eine pro Szene!");
            return;
        }

        instance = this;
    }

    void Start()
    {
        //Setzt den Standard Turret
        turretToBuild = standardTurretPrefab;
    }

    public GameObject GetTurretToBuild()
    {
        return turretToBuild;
    }
}