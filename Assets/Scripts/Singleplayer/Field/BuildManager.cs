using MLAPI;
using UnityEngine;

namespace Singleplayer.Field
{
    /**
 * Buildmanger nach Singleton-Pattern
 */
    public class BuildManager : MonoBehaviour
    {
        public static BuildManager instance;
        private GameObject turretToBuild;
        private GameObject turretToBuildPreview;
        public GameObject[] mountainTurrets;
        public GameObject[] mountainPreviewTurrets;
        public GameObject[] forestTurrets;
        public GameObject[] forestPreviewTurrets;
        public GameObject[] swampTurrets;
        public GameObject[] swampPreviewTurrets;


        [SerializeField] private HGrid grid;

        public HGrid Grid => grid;
        

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
            //TODO Müssen dann noch PreviewPrefabs für alle Türme machen und erst bei der Shop Auswahl setzen
            // turretToBuildPreview = mountainPreviewTurrets[0];
        }

        public GameObject GetTurretToBuild()
        {
            return turretToBuild;
        }
        //Graphics.DrawMesh vllt ist schöner
        public GameObject GetTurretToBuildPreview()
        {
            return turretToBuildPreview;
        }
        

        public void SetTurretToBuild(GameObject turret)
        {
            turretToBuild = turret;
        }
        public void SetTurretToBuildPreview(GameObject preview)
        {
            turretToBuildPreview = preview;
            
        }
    }
}