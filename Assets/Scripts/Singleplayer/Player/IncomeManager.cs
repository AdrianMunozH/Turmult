using Singleplayer.Field;
using UnityEngine;


// muss in shared
namespace Singleplayer.Player
{
    public class IncomeManager : MonoBehaviour
    {
        private static IncomeManager _instance;
        private bool[] forestTurrets;
        private bool[] mountainTurrets;
        private bool[] swampTurrets;
        

        public static IncomeManager Instance
        {
            get { return _instance; }
        }

        private Player _playerStats;



        // Start is called before the first frame update
        void Start()
        {
            if (_instance != null && _instance != this)
            {
                Destroy(gameObject);
            }
            else
            {
                _instance = this;
            }

            forestTurrets = new bool[3];
            mountainTurrets = new bool[3];
            swampTurrets = new bool[3];

            _playerStats = GetComponent<Player>();

        }

        public bool IsTurretUnlocked(Resource.ResourceType ressource, int turretIndex)
        {
            if (turretIndex > 4) return false;
            if (ressource == Resource.ResourceType.Wald)
            {
                return forestTurrets[turretIndex];
            }
            else if (ressource == Resource.ResourceType.Berg)
            {
                return mountainTurrets[turretIndex];
            }
            else if (ressource == Resource.ResourceType.Sumpf)
            {
                return swampTurrets[turretIndex];
            }

            return false;

        }

        public bool UnlockTurret(int ressourceCost, Resource.ResourceType ressource, int turretIndex)
        {
            if (IsTurretUnlocked(ressource, turretIndex)) return true;
            if(RessourcePurchase(ressourceCost,ressource))
            {
                switch (ressource)
                {
                    case Resource.ResourceType.Berg:
                        mountainTurrets[turretIndex] = true;
                        return true;
                    case Resource.ResourceType.Wald:
                        forestTurrets[turretIndex] = true;
                        return true;
                    case Resource.ResourceType.Sumpf:
                        swampTurrets[turretIndex] = true;
                        return true;
                }
            }

            return false;
        }

        public bool PurchaseTurret(int turretCost, Resource.ResourceType ressourceEnum, int turretIndex) 
        {
            if (IsTurretUnlocked(ressourceEnum, turretIndex))
            {
                return GoldPurchase(turretCost);
            }

            return false;
        }

        public bool RessourcePurchase(int ressourceCost, Resource.ResourceType ressource)
        {
            if (ressourceCost < 0) return false;
            
            
            if (ressource == Resource.ResourceType.Berg && ressourceCost <= _playerStats.Mountain)
            {
                _playerStats.Mountain -= ressourceCost;
                return true;
            }if (ressource == Resource.ResourceType.Sumpf && ressourceCost <= _playerStats.Swamp)
            {
                _playerStats.Swamp -= ressourceCost;
                return true;
            }if (ressource == Resource.ResourceType.Wald && ressourceCost <= _playerStats.Forest)
            {
                _playerStats.Forest -= ressourceCost;
                return true;
            }
            
            return false;
        }
        
        
        
        public bool GoldPurchase(int gold)
        {
            if (gold < 0) return false;
            if (gold <= _playerStats.Gold)
                return false;

            _playerStats.Gold -= gold;
            return true;
        }



        // zinsen
        public void Interest()
        {
            // possible loss of fraction ist gewollt
            float interest = _playerStats.Gold / 10;
            _playerStats.Gold += (int) interest;
        }
    
    
        //TODO
    /*
        //runden geld
        [ServerRpc]
        public void RoundIncomeServerRpc()
        {
            RoundIncomeClientRpc();
        }
    
        [ClientRpc]
        public void RoundIncomeClientRpc()
        {
            int someValue = 5;
            _playerStats.Gold += someValue;
        
            _playerStats.Mountain += someValue;
            _playerStats.Forest += someValue;
            _playerStats.Swamp += someValue;
        
            //hier aufrufen oder im gamemanager ?
            Interest();
        }

        [ServerRpc]
        private void MinionGoldServerRpc(ulong playerID, int minionGold)
        {
            if(!IsServer) return;
        
            foreach (var client in NetworkManager.Singleton.ConnectedClientsList)
            {
                if(client.ClientId == playerID)
                    MinionGoldClientRpc(minionGold);
            }
        }

        [ClientRpc]
        private void MinionGoldClientRpc(int minionGold)
        {
            _playerStats.Gold += minionGold;
        }
*/

    }
}
