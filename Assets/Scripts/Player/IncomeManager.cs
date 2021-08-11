using Field;
using MLAPI;
using MLAPI.Messaging;
using Player;


// muss in shared
public class IncomeManager : NetworkBehaviour
{
    private static IncomeManager _instance;

    public static IncomeManager Instance
    {
        get { return _instance; }
    }

    private PlayerStats _playerStats;



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
        if (ressourceEnum == Ressource.RessourceType.Sumpf && turretCost < _playerStats.Sumpf)
        {
            _playerStats.Sumpf -= turretCost;
            return true;
        }

        if (ressourceEnum ==  Ressource.RessourceType.Berg && turretCost < _playerStats.Berg)
        {
            _playerStats.Berg -= turretCost;
            return true;
        }

        if (ressourceEnum ==  Ressource.RessourceType.Wald && turretCost < _playerStats.Wald)
        {
            _playerStats.Wald -= turretCost;
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
        
        _playerStats.Berg += someValue;
        _playerStats.Wald += someValue;
        _playerStats.Sumpf += someValue;
        
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

