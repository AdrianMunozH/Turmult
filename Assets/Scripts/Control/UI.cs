using System;
using System.Collections;
using System.Collections.Generic;
using Field;
using JetBrains.Annotations;
using Ui.Input;
using UnityEngine;
using UnityEngine.UI;

namespace Control
{
    public class UI : MonoBehaviour
    {
        [SerializeField] private Image bgImage;

        [SerializeField] private Sprite[] bgSprites;
        
        [SerializeField] private GameObject[] ressourceType;
        [SerializeField] private GameObject acquireMode;

        [SerializeField] private GameObject scrollViewTower;
        [SerializeField] private GameObject scrollViewMinion;

        [SerializeField] private GameObject[] towerTypes;
        [SerializeField] private GameObject[] minionTypes;

        private BuildManager buildManager;

        //wenn towerMode false, dann ist man im minionMode
        private bool towerMode;

        private void Start()
        {
            buildManager = BuildManager.instance;
            towerMode = true;
            scrollViewTower.SetActive(true);
            towerTypes[0].SetActive(true);
            ressourceType[0].SetActive(true);
            bgImage.sprite = bgSprites[1];

        }

        [UsedImplicitly]
        public void OpenTowerBuildMode()
        {
            towerMode = true;
            scrollViewTower.SetActive(true);
            scrollViewMinion.SetActive(false);
            DeselectType();
            towerTypes[0].SetActive(true);
            ressourceType[0].SetActive(true);
            bgImage.sprite = bgSprites[1];
            Camera.main.cullingMask = -1;
        }
        
        [UsedImplicitly]
        public void OpenMinionBuildMode()
        {
            towerMode = false;
            scrollViewMinion.SetActive(true);
            scrollViewTower.SetActive(false);
            DeselectType();
            minionTypes[0].SetActive(true);
            ressourceType[0].SetActive(true);
            bgImage.sprite = bgSprites[1];
            Camera.main.cullingMask &=  ~(1 << LayerMask.NameToLayer("Shader"));
        }
        [UsedImplicitly]
        public void SetAcquireMode()
        {
            DeselectType();
            PlayerInputManager.Instance.SetState(new AcquireState());
            
            acquireMode.SetActive(true);
            scrollViewTower.SetActive(false);
            scrollViewMinion.SetActive(false);
            bgImage.sprite = bgSprites[0];
            buildManager._acquireModeOn = !buildManager._acquireModeOn;
            if (buildManager._acquireModeOn && buildManager._buildModeOn)
            {
                buildManager._buildModeOn = false;
            }
            Camera.main.cullingMask = -1;
            
        }

        [UsedImplicitly]
        public void SelectType(Button type)
        {
            
            if (buildManager._acquireModeOn)
            {
                towerMode = true;
                scrollViewTower.SetActive(true);
            }
            if (towerMode)
            {
                PlayerInputManager.Instance.SetState(new BuildState());
                DeselectType();
                switch (type.name)
                {
                    case "Mountain":
                        towerTypes[0].SetActive(true);
                        ressourceType[0].SetActive(true);
                        bgImage.sprite = bgSprites[1];
                        break;
                    case "Forest":
                        towerTypes[1].SetActive(true);
                        ressourceType[1].SetActive(true);
                        bgImage.sprite = bgSprites[2];
                        break;
                    case "Swamp":
                        towerTypes[2].SetActive(true);
                        ressourceType[2].SetActive(true);
                        bgImage.sprite = bgSprites[3];
                        break;
                }
            }
            else
            {
               DeselectType();
                switch (type.name)
                {
                    case "Mountain":
                        minionTypes[0].SetActive(true);
                        ressourceType[0].SetActive(true);
                        bgImage.sprite = bgSprites[1];
                        break;
                    case "Forest":
                        minionTypes[1].SetActive(true);
                        ressourceType[1].SetActive(true);
                        bgImage.sprite = bgSprites[2];
                        break;
                    case "Swamp":
                        minionTypes[2].SetActive(true);
                        ressourceType[2].SetActive(true);
                        bgImage.sprite = bgSprites[3];
                        break;
                }
            }
        }

        void DeselectType()
        {
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

            acquireMode.SetActive(false);
            buildManager._acquireModeOn = false;
        }
    
    }
}

