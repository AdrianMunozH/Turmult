using System;
using System.Collections;
using System.Collections.Generic;
using Field;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.UI;

namespace Control
{
    public class UI : MonoBehaviour
    {
        [SerializeField] private Button towerBuildMode;
        [SerializeField] private Button minionBuildMode;
        
        [SerializeField] private GameObject[] ressourceType;

        [SerializeField] private GameObject scrollViewTower;
        [SerializeField] private GameObject scrollViewMinion;

        [SerializeField] private GameObject[] towerTypes;
        [SerializeField] private GameObject[] minionTypes;

        [SerializeField] private BuildManager buildManager;

        //wenn towerMode false, dann ist man im minionMode
        private bool towerMode;

        private void Start()
        {
            towerMode = true;
            scrollViewTower.SetActive(true);
            towerTypes[0].SetActive(true);
            ressourceType[0].SetActive(true);
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
        }

        [UsedImplicitly]
        public void SelectType(Button type)
        {
            if (towerMode)
            {
                
                DeselectType();
                switch (type.name)
                {
                    case "Mountain":
                        towerTypes[0].SetActive(true);
                        ressourceType[0].SetActive(true);
                        break;
                    case "Forest":
                        towerTypes[1].SetActive(true);
                        ressourceType[1].SetActive(true);
                        break;
                    case "Swamp":
                        towerTypes[2].SetActive(true);
                        ressourceType[2].SetActive(true);
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
                        break;
                    case "Forest":
                        minionTypes[1].SetActive(true);
                        ressourceType[1].SetActive(true);
                        break;
                    case "Swamp":
                        minionTypes[2].SetActive(true);
                        ressourceType[2].SetActive(true);
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

            buildManager._acquireModeOn = false;
        }
    
    }
}

