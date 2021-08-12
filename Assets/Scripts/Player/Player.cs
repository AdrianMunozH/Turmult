using DapperDino.UMT.Lobby.Networking;
using MLAPI;
using MLAPI.Connection;
using MLAPI.Messaging;
using MLAPI.NetworkVariable.Collections;
using TMPro;
using Ui;
using UnityEngine;
using UnityEngine.UI;

namespace Player
{
    public class Player : NetworkBehaviour
    {
        [Header("Spieler Guthaben")]
        private int gold;
        public int startGold = 500;
        public int interestRateInPercentPerRound = 5;
        private int forest;

        public int Gold
        {
            get => gold;
            set => gold = value;
        }

        public int Forest
        {
            get => forest;
            set => forest = value;
        }

        public int Mountain
        {
            get => mountain;
            set => mountain = value;
        }

        public int Swamp
        {
            get => swamp;
            set => swamp = value;
        }

        public int startForest = 1;
        private int mountain;
        public int startMountain = 1;
        private int swamp;
        public int startSwamp = 1;

        [Header("Lifes")] 
        private int lifes;
        public int startLifes = 20;
        private float hpBarValue;
        
        [Header("Lifebar Color")] 
        public Color full = Color.green;
        public Color nearlyFull = Color.cyan;
        public Color medium = Color.yellow;
        public Color low = new Color(0.6f,0.4f,0.2f);
        public Color superlow = Color.red;
        
        [Header("Textfelder/Gameobjekte")] 
        public TextMeshProUGUI goldTMP;
        public TextMeshProUGUI mountainTMP;
        public TextMeshProUGUI forestTMP;
        public TextMeshProUGUI swampTMP;
        public Image hpBar;
        public TextMeshProUGUI gameOVER;
        
        private NetworkList<GamePlayerState> connectedPlayers = new NetworkList<GamePlayerState>();
        
    
        // Start is called before the first frame update
        void Start()
        {
            if (IsServer) return;

            gold = startGold;
            forest = startForest;
            mountain = startMountain;
            swamp = startSwamp;
            lifes = startLifes;
            //Berechnung des Fillamounts
            hpBarValue = (float) lifes / startLifes;
            hpBar.fillAmount = hpBarValue;
            
            //TODO Eventsubscription für Interestberechnung
        }
        

        private void FixedUpdate()
        {
            if (IsServer) return;
            goldTMP.text = gold.ToString();
            mountainTMP.text = mountain.ToString();
            forestTMP.text = forest.ToString();
            swampTMP.text = swamp.ToString();
            
            //Berechnung des Fillamounts
            hpBarValue = (float) lifes / startLifes;
            hpBar.fillAmount = hpBarValue;
            hpBar.color = getHpBarColor();
        }

        private Color getHpBarColor()
        {
            if (hpBarValue > 0.95)
            {
                return full; 
            }
            else if (hpBarValue > 0.8)
            {
                return nearlyFull;
            }else if (hpBarValue > 0.6)
            {
                return medium;
            }else if (hpBarValue > 0.4)
            {
                return low;
            }
            return superlow;
        }

        public void LoseLife()
        {
            LoseLifeServerRpc();
        }

        [ContextMenu("LoseLife")]
        [ServerRpc(RequireOwnership = false)]
        public void LoseLifeServerRpc(ServerRpcParams serverRpcParams = default)
        {
            for (int i = 0; i < connectedPlayers.Count; i++)
            {
                Debug.Log(connectedPlayers[i].ClientId +" "+ serverRpcParams.Receive.SenderClientId);
                
                if (connectedPlayers[i].ClientId == serverRpcParams.Receive.SenderClientId)
                {
                    connectedPlayers[i] = new GamePlayerState(
                        connectedPlayers[i].ClientId,
                        connectedPlayers[i].PlayerName,
                        connectedPlayers[i].Lifes-1
                    );
                    Debug.Log(connectedPlayers[i].PlayerName);
                }
            }
        }

        
        public override void NetworkStart()
        {
            if (IsClient)
            {
                connectedPlayers.OnListChanged += HandlePlayersStateChanged;
            }

            if (IsServer)
            {
                NetworkManager.Singleton.OnClientConnectedCallback += HandleClientConnected;
                NetworkManager.Singleton.OnClientDisconnectCallback += HandleClientDisconnect;

                foreach (NetworkClient client in NetworkManager.Singleton.ConnectedClientsList)
                {
                    //Füge alle connected Players der Liste hinzu
                    HandleClientConnected(client.ClientId);
                }
            }
        }

        //Aufgerufen sobald sich das Leben ändert
        private void HandlePlayersStateChanged(NetworkListEvent<GamePlayerState> playerState)
        {
            if (playerState.Value.ClientId == NetworkManager.Singleton.LocalClientId)
            {
                lifes = playerState.Value.Lifes;
                Debug.Log(lifes);
                if (lifes <= 0)
                {
                    gameOVER.gameObject.SetActive(true);
                }
            }
        }

        private void HandleClientConnected(ulong clientId)
        {
            var playerData = ServerGameNetPortal.Instance.GetPlayerData(clientId);
            if (!playerData.HasValue)
            {
                return;
            }
            
            connectedPlayers.Add(new GamePlayerState(
                clientId,
                playerData.Value.PlayerName,
                startLifes
            ));
        }
        

        private void HandleClientDisconnect(ulong clientId)
        {
            for (int i = 0; i < connectedPlayers.Count; i++)
            {
                if (connectedPlayers[i].ClientId == clientId)
                {
                    connectedPlayers.RemoveAt(i);
                    break;
                }
            }
        }


    }
}
