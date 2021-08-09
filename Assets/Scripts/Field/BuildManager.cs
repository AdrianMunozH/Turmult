using System;
using DG.Tweening;
using UnityEngine;

namespace Field
{
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
        public GameObject[] mountainTurrets;
        public GameObject[] mountainPreviewTurrets;
        public GameObject[] forestTurrets;
        public GameObject[] forestPreviewTurrets;
        public GameObject[] swampTurrets;
        public GameObject[] swampPreviewTurrets;
        public bool _buildModeOn;
        public bool _acquireModeOn;

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
            _buildModeOn = false;
            _acquireModeOn = false;

            
        }
      
        void Start()
        {
            //Setzt den Standard Turret
            //TODO Müssen dann noch PreviewPrefabs für alle Türme machen und erst bei der Shop Auswahl setzen
            turretToBuildPreview = standardTurretPreviewPrefab;
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

        public bool IsBuildModeOn()
        {
            return _buildModeOn;
        }

        public bool isAcquireModeOn()
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