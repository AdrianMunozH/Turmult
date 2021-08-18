using Singleplayer.Field;
using Singleplayer.Player;
using Singleplayer.Ui.Input;
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

        private void UnlockTurret(int ressourceCost,Resource.ResourceType resource , int turrentIndex)
        {
            // schon unlocked kann gekauft werden
            if (IncomeManager.Instance.IsTurretUnlocked(resource, turrentIndex))
            {
                // muss gar nicht abgefragt werden ob es genug geld gibt
                SetTurretToBuild(resource ,turrentIndex);
                PlayerInputManager.Instance.GetState().CurrentTurretIndex = turrentIndex;

            }
               
            else
            {     //nicht unlocked -- wenn unlock möglich ist wird UI geändert
                if (IncomeManager.Instance.UnlockTurret(ressourceCost, resource, turrentIndex))
                {
                    _ui.UnlockTurretImage(resource,turrentIndex);
                    SetTurretToBuild(resource ,turrentIndex);
                }
                    
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

            UnlockTurret(1,Resource.ResourceType.Berg,0);
        }
        public void PurchaseMountainTurretTwo()
        {
            
            UnlockTurret(2,Resource.ResourceType.Berg,1);
        }
        public void PurchaseMountainTurretThree()
        {
            
            
            UnlockTurret(3,Resource.ResourceType.Berg,2);
        }

        //Forest Turrets
        public void PurchaseForestTurretOne()
        {
            
            UnlockTurret(1,Resource.ResourceType.Wald,0);
        }
        public void PurchaseForestTurretTwo()
        {
            
            UnlockTurret(2,Resource.ResourceType.Wald,1);
            
        }
        public void PurchaseForestTurretThree()
        {
           
            
            UnlockTurret(3,Resource.ResourceType.Wald,2);
        }
        
        //Swamp Turrets
        public void PurchaseSwampTurretOne()
        {

            UnlockTurret(1,Resource.ResourceType.Sumpf,0);
        }
        public void PurchaseSwampTurretTwo()
        {

            UnlockTurret(2,Resource.ResourceType.Sumpf,1);
        }
        public void PurchaseSwampTurretThree()
        {

            UnlockTurret(3,Resource.ResourceType.Sumpf,2);
        }
    }
}

