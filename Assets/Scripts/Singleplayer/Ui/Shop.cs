using Singleplayer.Field;
using Singleplayer.Player;
using UnityEngine;

namespace Singleplayer.Ui
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

        private void UnlockTurret(int turretCost, int ressourceCost,Resource.ResourceType resource , int turrentIndex)
        {
            if (IncomeManager.Instance.IsTurretUnlocked(resource, turrentIndex))
            {
                if(IncomeManager.Instance.PurchaseTurret(turretCost, resource, turrentIndex))
                    SetTurretToBuild(resource ,turrentIndex);
            }
               
            else
            {     //nicht unlocked -- wenn unlock möglich ist wird UI geändert
                if(IncomeManager.Instance.UnlockTurret(ressourceCost, resource, turrentIndex))
                    _ui.UnlockTurretImage(resource,turrentIndex);
            }
        }

        private void SetTurretToBuild(Resource.ResourceType ressourceType, int turretIndex)
        {
            switch (ressourceType)
            {
                case Resource.ResourceType.Berg:
                    buildManager.SetTurretToBuild(buildManager.mountainTurrets[turretIndex]);
                    buildManager.SetTurretToBuildPreview(buildManager.mountainPreviewTurrets[turretIndex]);
                    break;
                case Resource.ResourceType.Sumpf:
                    buildManager.SetTurretToBuild(buildManager.swampTurrets[turretIndex]);
                    buildManager.SetTurretToBuildPreview(buildManager.swampPreviewTurrets[turretIndex]);
                    break;
                case Resource.ResourceType.Wald:
                    buildManager.SetTurretToBuild(buildManager.forestTurrets[turretIndex]);
                    buildManager.SetTurretToBuildPreview(buildManager.forestPreviewTurrets[turretIndex]);
                    break;
            }
        }
        
        public void PurchaseMountainTurretOne()
        {
            Debug.Log("Mountain Turret 1 Purchased");
            
            UnlockTurret(20,1,Resource.ResourceType.Berg,0);
        }
        public void PurchaseMountainTurretTwo()
        {
            Debug.Log("Mountain Turret 2 Purchased");
            UnlockTurret(30,2,Resource.ResourceType.Berg,1);
        }
        public void PurchaseMountainTurretThree()
        {
            Debug.Log("Mountain Turret 3 Purchased");
            
            UnlockTurret(40,3,Resource.ResourceType.Berg,2);
        }

        //Forest Turrets
        public void PurchaseForestTurretOne()
        {
            Debug.Log("Forest Turret 1 Purchased");
            UnlockTurret(20,1,Resource.ResourceType.Wald,0);
        }
        public void PurchaseForestTurretTwo()
        {
            Debug.Log("Forest Turret 2 Purchased");
            UnlockTurret(30,2,Resource.ResourceType.Wald,1);
            
        }
        public void PurchaseForestTurretThree()
        {
            Debug.Log("Forest Turret 3 Purchased");
            
            UnlockTurret(40,3,Resource.ResourceType.Wald,2);
        }
        
        //Swamp Turrets
        public void PurchaseSwampTurretOne()
        {
            Debug.Log("Swamp Turret 1 Purchased");
            
            UnlockTurret(20,1,Resource.ResourceType.Sumpf,0);
        }
        public void PurchaseSwampTurretTwo()
        {
            Debug.Log("Swamp Turret 2 Purchased");
            
            UnlockTurret(30,2,Resource.ResourceType.Sumpf,1);
        }
        public void PurchaseSwampTurretThree()
        {
            Debug.Log("Swamp Turret 3 Purchased");
            
            UnlockTurret(40,3,Resource.ResourceType.Sumpf,2);
        }
    }
}

