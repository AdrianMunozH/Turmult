
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
        

        public static IncomeManager Instance => _instance;

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
            if(ResourcePurchase(ressourceCost,ressource))
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
                Debug.Log("turret is unlocked");
                return GoldPurchase(turretCost);
            }

            return false;
        }

        public bool ResourcePurchase(int resourceCost, Resource.ResourceType ressource)
        {
            if (resourceCost < 0) return false;
            
            
            if (ressource == Resource.ResourceType.Berg && resourceCost <= _playerStats.Mountain)
            {
                _playerStats.Mountain -= resourceCost;
                return true;
            }if (ressource == Resource.ResourceType.Sumpf && resourceCost <= _playerStats.Swamp)
            {
                _playerStats.Swamp -= resourceCost;
                return true;
            }if (ressource == Resource.ResourceType.Wald && resourceCost <= _playerStats.Forest)
            {
                _playerStats.Forest -= resourceCost;
                return true;
            }
            
            return false;
        }
        
        
        
        public bool GoldPurchase(int gold)
        {
            if (gold < 0) return false;
            if (gold >= _playerStats.Gold)
                return false;

            _playerStats.Gold -= gold;
            return true;
        }

        public bool GetResource(int resource, Resource.ResourceType resourceType)
        {
            if (resource < 0) return false;

            switch (resourceType)
            {
                case Resource.ResourceType.Berg:
                    _playerStats.Mountain += resource;
                    break;
                case Resource.ResourceType.Wald:
                    _playerStats.Forest += resource;
                    break;
                case Resource.ResourceType.Sumpf:
                    _playerStats.Swamp += resource;
                    break;
            }

            return true;
        }



        // zinsen
        public void Interest()
        {
            // possible loss of fraction ist gewollt
            float interest = _playerStats.Gold / 10;
            _playerStats.Gold += (int) interest;
        }

        public void MinionGold(int gold)
        {
            if(gold < 0) return;
            _playerStats.Gold += gold;
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

