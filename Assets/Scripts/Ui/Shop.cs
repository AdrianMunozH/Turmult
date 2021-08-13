using System;
using System.Collections;
using System.Collections.Generic;
using Field;
using Player;
using UnityEngine;

namespace Control
{
    public class Shop : MonoBehaviour
    {
        private BuildManager buildManager;
        private UI _ui;

        private void Start()
        {
            buildManager = BuildManager.instance;
            _ui = GetComponent<UI>();
        }

        //Mountain Turrets

        private void UnlockTurret(int turretCost, int ressourceCost,Ressource.RessourceType ressource , int turrentIndex)
        {
            if (IncomeManager.Instance.IsTurretUnlocked(ressource, turrentIndex))
            {
                if(IncomeManager.Instance.PurchaseTurret(turretCost, ressource, turrentIndex))
                    SetTurretToBuild(ressource ,turrentIndex);
            }
               
            else
            {     //nicht unlocked -- wenn unlock möglich ist wird UI geändert
                if(IncomeManager.Instance.UnlockTurret(ressourceCost, ressource, turrentIndex))
                    _ui.UnlockTurretImage(ressource,turrentIndex);
            }
        }

        private void SetTurretToBuild(Ressource.RessourceType ressourceType, int turretIndex)
        {
            switch (ressourceType)
            {
                case Ressource.RessourceType.Berg:
                    buildManager.SetTurretToBuild(buildManager.mountainTurrets[turretIndex]);
                    buildManager.SetTurretToBuildPreview(buildManager.mountainPreviewTurrets[turretIndex]);
                    break;
                case Ressource.RessourceType.Sumpf:
                    buildManager.SetTurretToBuild(buildManager.swampTurrets[turretIndex]);
                    buildManager.SetTurretToBuildPreview(buildManager.swampPreviewTurrets[turretIndex]);
                    break;
                case Ressource.RessourceType.Wald:
                    buildManager.SetTurretToBuild(buildManager.forestTurrets[turretIndex]);
                    buildManager.SetTurretToBuildPreview(buildManager.forestPreviewTurrets[turretIndex]);
                    break;
            }
        }
        
        public void PurchaseMountainTurretOne()
        {
            Debug.Log("Mountain Turret 1 Purchased");
            
            UnlockTurret(20,1,Ressource.RessourceType.Berg,0);
        }
        public void PurchaseMountainTurretTwo()
        {
            Debug.Log("Mountain Turret 2 Purchased");
            UnlockTurret(30,2,Ressource.RessourceType.Berg,1);
        }
        public void PurchaseMountainTurretThree()
        {
            Debug.Log("Mountain Turret 3 Purchased");
            
            UnlockTurret(40,3,Ressource.RessourceType.Berg,2);
        }

        //Forest Turrets
        public void PurchaseForestTurretOne()
        {
            Debug.Log("Forest Turret 1 Purchased");
            UnlockTurret(20,1,Ressource.RessourceType.Wald,0);
        }
        public void PurchaseForestTurretTwo()
        {
            Debug.Log("Forest Turret 2 Purchased");
            UnlockTurret(30,2,Ressource.RessourceType.Wald,1);
            
        }
        public void PurchaseForestTurretThree()
        {
            Debug.Log("Forest Turret 3 Purchased");
            
            UnlockTurret(40,3,Ressource.RessourceType.Wald,2);
        }
        
        //Swamp Turrets
        public void PurchaseSwampTurretOne()
        {
            Debug.Log("Swamp Turret 1 Purchased");
            
            UnlockTurret(20,1,Ressource.RessourceType.Sumpf,0);
        }
        public void PurchaseSwampTurretTwo()
        {
            Debug.Log("Swamp Turret 2 Purchased");
            
            UnlockTurret(30,2,Ressource.RessourceType.Sumpf,1);
        }
        public void PurchaseSwampTurretThree()
        {
            Debug.Log("Swamp Turret 3 Purchased");
            
            UnlockTurret(40,3,Ressource.RessourceType.Sumpf,2);
        }
    }
}

