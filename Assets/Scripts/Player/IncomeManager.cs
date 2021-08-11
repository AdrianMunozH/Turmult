using Field;
using MLAPI;
using MLAPI.Messaging;


// muss in shared
namespace Player
{
    public class IncomeManager : NetworkBehaviour
    {
        private static IncomeManager _instance;

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

        }

        public bool PurchaseTurret(int turretCost, Ressource.RessourceType ressourceEnum)
        {
            if (turretCost < 0) return false;
            if (ressourceEnum == Ressource.RessourceType.Sumpf && turretCost < _playerStats.Swamp)
            {
                _playerStats.Swamp -= turretCost;
                return true;
            }

            if (ressourceEnum ==  Ressource.RessourceType.Berg && turretCost < _playerStats.Mountain)
            {
                _playerStats.Mountain -= turretCost;
                return true;
            }

            if (ressourceEnum ==  Ressource.RessourceType.Wald && turretCost < _playerStats.Forest)
            {
                _playerStats.Forest -= turretCost;
                return true;
            }

            return false;
        }

        public bool GoldPurchase(int gold)
        {
            if (gold < 0) return false;
            if (gold > _playerStats.Gold)
                return false;

            _playerStats.Gold -= gold;
            return true;
        }



        // zinsen
        public void Interest()
        {
            float interest = _playerStats.Gold / 10;
            _playerStats.Gold += (int) interest;
        }
    
    
    
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


    }
}

