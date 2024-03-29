using DG.Tweening;
using Singleplayer.Field;
using JetBrains.Annotations;
using Singleplayer.Ui.Input;
using UnityEngine;
using UnityEngine.UI;

namespace Singleplayer.Ui
{
    public class UI : MonoBehaviour
    {
        [SerializeField] private Image bgImage;

        [SerializeField] private Sprite[] bgSprites;
        
        [SerializeField] private GameObject[] ressourceType;
        [SerializeField] private GameObject acquireMode;

        [SerializeField] private GameObject[] forestLockedImage;
        [SerializeField] private GameObject[] mountainLockedImage;
        [SerializeField] private GameObject[] swampLockedImage;
        
        [SerializeField] private GameObject buildMode;

        [SerializeField] private GameObject scrollViewTower;
        [SerializeField] private GameObject scrollViewMinion;

        [SerializeField] private GameObject[] towerTypes;
        [SerializeField] private GameObject[] minionTypes;

        private BuildManager buildManager;

        private Vector3 menuPos;


        //wenn towerMode false, dann ist man im minionMode
        public bool towerMode;
        
        
        private int currentRessource;

        public int CurrentRessource => currentRessource;
        // 0 == berg
        // 1 == wald
        // 2 == sumpf

        private void Start()
        {
            buildManager = BuildManager.instance;
            towerMode = false;
            bgImage.sprite = bgSprites[1];
            SetAcquireMode();
            currentRessource = 0;
            menuPos = bgImage.transform.position;


        }

        public void UnlockTurretImage(Resource.ResourceType ressource , int turretIndex)
        {
            switch (ressource)
            {
                case Resource.ResourceType.Berg:
                    mountainLockedImage[turretIndex].SetActive(false);
                    break;
                case Resource.ResourceType.Sumpf:
                    swampLockedImage[turretIndex].SetActive(false);
                    break;
                    
                case Resource.ResourceType.Wald:
                    forestLockedImage[turretIndex].SetActive(false);
                    break;
            }
        }

        [UsedImplicitly]
        public void OpenTowerBuildMode(int ressource = 0)
        {
            towerMode = true;
            scrollViewTower.SetActive(true);
            scrollViewMinion.SetActive(false);
            DeselectType();
            towerTypes[ressource].SetActive(true);
            ressourceType[ressource].SetActive(true);
            bgImage.sprite = bgSprites[1];
            Camera.main.cullingMask = -1;
            
            //TurretToBuild wird erst beim Purchase gesetzt
            buildManager.SetTurretToBuild(null);
        }
        
        [UsedImplicitly]
        public void OpenMinionBuildMode()
        {
            towerMode = false;
            scrollViewMinion.SetActive(true);
            scrollViewTower.SetActive(false);
            DeselectType();
            minionTypes[currentRessource].SetActive(true);
            ressourceType[currentRessource].SetActive(true);
            bgImage.sprite = bgSprites[currentRessource+1]; // später wird das acquired bg gelösct und dann brauchen wir kein +1
            Camera.main.cullingMask &=  ~(1 << LayerMask.NameToLayer("Shader"));
        }
        [UsedImplicitly]
        public void SetAcquireMode()
        {
            DeselectType();
            // setzt den state
            PlayerInputManager.Instance.AcquireModeOn();
            
            acquireMode.SetActive(true);
            scrollViewTower.SetActive(false);
            scrollViewMinion.SetActive(false);
            buildMode.SetActive(false);
            bgImage.gameObject.SetActive(false);
            bgImage.sprite = bgSprites[0];
            
            Camera.main.cullingMask = -1;
            
        }

        [UsedImplicitly]
        public void SelectType(Button type)
        {
            
            if (PlayerInputManager.Instance.GetState().name.Equals("Acquire"))
            {
                towerMode = true;
                scrollViewTower.SetActive(true);
                
            }
            if (towerMode)
            {
                PlayerInputManager.Instance.BuildStateOn();
                DeselectType();
                buildMode.SetActive(true);
                bgImage.transform.position = new Vector3(menuPos.x, menuPos.y - 100, menuPos.z);
                bgImage.gameObject.SetActive(true);
                bgImage.transform.DOMoveY(menuPos.y, 0.2f);
                switch (type.name)
                {
                    case "Mountain":
                        towerTypes[0].SetActive(true);
                        ressourceType[0].SetActive(true);
                        bgImage.sprite = bgSprites[1];
                        currentRessource = 0;
                        PlayerInputManager.Instance.GetState().CurrentResource = Resource.ResourceType.Berg;
                        break;
                    case "Forest":
                        towerTypes[1].SetActive(true);
                        ressourceType[1].SetActive(true);
                        bgImage.sprite = bgSprites[2];
                        currentRessource = 1;
                        PlayerInputManager.Instance.GetState().CurrentResource = Resource.ResourceType.Wald;
                        break;
                    case "Swamp":
                        towerTypes[2].SetActive(true);
                        ressourceType[2].SetActive(true);
                        bgImage.sprite = bgSprites[3];
                        currentRessource = 2;
                        PlayerInputManager.Instance.GetState().CurrentResource = Resource.ResourceType.Sumpf;
                        break;
                }
            }
            else
            {
               DeselectType();
               bgImage.transform.position = new Vector3(menuPos.x, menuPos.y - 100, menuPos.z);
               bgImage.gameObject.SetActive(true);
               bgImage.transform.DOMoveY(menuPos.y, 0.2f);
                switch (type.name)
                {
                    case "Mountain":
                        minionTypes[0].SetActive(true);
                        ressourceType[0].SetActive(true);
                        bgImage.sprite = bgSprites[1];
                        currentRessource = 0;
                        break;
                    case "Forest":
                        minionTypes[1].SetActive(true);
                        ressourceType[1].SetActive(true);
                        bgImage.sprite = bgSprites[2];
                        currentRessource = 1;
                        break;
                    case "Swamp":
                        minionTypes[2].SetActive(true);
                        ressourceType[2].SetActive(true);
                        bgImage.sprite = bgSprites[3];
                        currentRessource = 2;
                        break;
                }
            }
        }
        
        
        public void SelectType(string name)
        {
            
            if (PlayerInputManager.Instance.GetState().name.Equals("Acquire"))
            {
                towerMode = true;
                scrollViewTower.SetActive(true);
            }
            if (towerMode)
            {
                PlayerInputManager.Instance.BuildStateOn();
                DeselectType();
                buildMode.SetActive(true);
                bgImage.gameObject.SetActive(true);
                switch (name)
                {
                    case "Mountain":
                        towerTypes[0].SetActive(true);
                        ressourceType[0].SetActive(true);
                        bgImage.sprite = bgSprites[1];
                        currentRessource = 0;
                        PlayerInputManager.Instance.GetState().CurrentResource = Resource.ResourceType.Berg;
                        break;
                    case "Forest":
                        towerTypes[1].SetActive(true);
                        ressourceType[1].SetActive(true);
                        bgImage.sprite = bgSprites[2];
                        currentRessource = 1;
                        PlayerInputManager.Instance.GetState().CurrentResource = Resource.ResourceType.Wald;
                        break;
                    case "Swamp":
                        towerTypes[2].SetActive(true);
                        ressourceType[2].SetActive(true);
                        bgImage.sprite = bgSprites[3];
                        currentRessource = 2;
                        PlayerInputManager.Instance.GetState().CurrentResource = Resource.ResourceType.Sumpf;
                        break;
                }
            }
            else
            {
               DeselectType();
                switch (name)
                {
                    case "Mountain":
                        minionTypes[0].SetActive(true);
                        ressourceType[0].SetActive(true);
                        bgImage.sprite = bgSprites[1];
                        currentRessource = 0;
                        break;
                    case "Forest":
                        minionTypes[1].SetActive(true);
                        ressourceType[1].SetActive(true);
                        bgImage.sprite = bgSprites[2];
                        currentRessource = 1;
                        break;
                    case "Swamp":
                        minionTypes[2].SetActive(true);
                        ressourceType[2].SetActive(true);
                        bgImage.sprite = bgSprites[3];
                        currentRessource = 2;
                        break;
                }
            }
        }

        void DeselectType()
        {
            acquireMode.SetActive(false);
            foreach (var tower in towerTypes)
            {
                tower.SetActive(false);
            }
            foreach (var minion in minionTypes)
            {
                minion.SetActive(false);
            }

            foreach (var ressource in ressourceType)
            {
                ressource.SetActive(false);
            }
           
            
        }
    
    }
}

