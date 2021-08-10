using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using MLAPI;
using MLAPI.SceneManagement;
using MLAPI.Spawning;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Networking
{
    public class ServerGameNetPortal : MonoBehaviour
    {
        [Header("Settings")]
        [SerializeField] private int maxPlayers = 4;

        public static ServerGameNetPortal Instance => _instance;
        private static ServerGameNetPortal _instance;

        private Dictionary<string, PlayerData> _clientData;
        private Dictionary<ulong, string> _clientIdToGuid;
        private Dictionary<ulong, int> _clientSceneMap;
        private bool _gameInProgress;

        private const int MaxConnectionPayload = 1024;

        private GameNetPortal _gameNetPortal;

        private void Awake()
        {
            if (_instance != null && _instance != this)
            {
                Destroy(gameObject);
                return;
            }

            _instance = this;
            DontDestroyOnLoad(gameObject);
        }

        private void Start()
        {
            _gameNetPortal = GetComponent<GameNetPortal>();
            _gameNetPortal.OnNetworkReadied += HandleNetworkReadied;

            NetworkManager.Singleton.ConnectionApprovalCallback += ApprovalCheck;
            NetworkManager.Singleton.OnServerStarted += HandleServerStarted;

            _clientData = new Dictionary<string, PlayerData>();
            _clientIdToGuid = new Dictionary<ulong, string>();
            _clientSceneMap = new Dictionary<ulong, int>();
        }

        private void OnDestroy()
        {
            if (_gameNetPortal == null) { return; }

            _gameNetPortal.OnNetworkReadied -= HandleNetworkReadied;

            if (NetworkManager.Singleton == null) { return; }

            NetworkManager.Singleton.ConnectionApprovalCallback -= ApprovalCheck;
            NetworkManager.Singleton.OnServerStarted -= HandleServerStarted;
        }

        public PlayerData? GetPlayerData(ulong clientId)
        {
            if (_clientIdToGuid.TryGetValue(clientId, out string clientGuid))
            {
                if (_clientData.TryGetValue(clientGuid, out PlayerData playerData))
                {
                    return playerData;
                }
                else
                {
                    Debug.LogWarning($"No player data found for client id: {clientId}");
                }
            }
            else
            {
                Debug.LogWarning($"No client guid found for client id: {clientId}");
            }

            return null;
        }

        public void StartGame()
        {
            _gameInProgress = true;

            NetworkSceneManager.SwitchScene(SceneData.Instance.sceneGame);
        }

        public void EndRound()
        {
            _gameInProgress = false;

            NetworkSceneManager.SwitchScene(SceneData.Instance.sceneLobby);
        }

        private void HandleNetworkReadied()
        {
            if (!NetworkManager.Singleton.IsServer) { return; }

            _gameNetPortal.OnUserDisconnectRequested += HandleUserDisconnectRequested;
            NetworkManager.Singleton.OnClientDisconnectCallback += HandleClientDisconnect;
            _gameNetPortal.OnClientSceneChanged += HandleClientSceneChanged;

            NetworkSceneManager.SwitchScene(SceneData.Instance.sceneLobby);

            if (NetworkManager.Singleton.IsHost)
            {
                _clientSceneMap[NetworkManager.Singleton.LocalClientId] = SceneManager.GetActiveScene().buildIndex;
            }
        }

        private void HandleClientDisconnect(ulong clientId)
        {
            _clientSceneMap.Remove(clientId);

            if (_clientIdToGuid.TryGetValue(clientId, out string guid))
            {
                _clientIdToGuid.Remove(clientId);

                if (_clientData[guid].ClientId == clientId)
                {
                    _clientData.Remove(guid);
                }
            }

            if (clientId == NetworkManager.Singleton.LocalClientId)
            {
                _gameNetPortal.OnUserDisconnectRequested -= HandleUserDisconnectRequested;
                NetworkManager.Singleton.OnClientDisconnectCallback -= HandleClientDisconnect;
                _gameNetPortal.OnClientSceneChanged -= HandleClientSceneChanged;
            }
        }

        private void HandleClientSceneChanged(ulong clientId, int sceneIndex)
        {
            _clientSceneMap[clientId] = sceneIndex;
        }

        private void HandleUserDisconnectRequested()
        {
            HandleClientDisconnect(NetworkManager.Singleton.LocalClientId);

            NetworkManager.Singleton.StopHost();

            ClearData();

            SceneManager.LoadScene(SceneData.Instance.sceneMainMenu);
        }

        private void HandleServerStarted()
        {
            if (!NetworkManager.Singleton.IsHost) { return; }

            string clientGuid = Guid.NewGuid().ToString();
            string playerName = PlayerPrefs.GetString("PlayerName", "Missing Name");

            _clientData.Add(clientGuid, new PlayerData(playerName, NetworkManager.Singleton.LocalClientId));
            _clientIdToGuid.Add(NetworkManager.Singleton.LocalClientId, clientGuid);
        }

        private void ClearData()
        {
            _clientData.Clear();
            _clientIdToGuid.Clear();
            _clientSceneMap.Clear();

            _gameInProgress = false;
        }

        private void ApprovalCheck(byte[] connectionData, ulong clientId, NetworkManager.ConnectionApprovedDelegate callback)
        {
            if (connectionData.Length > MaxConnectionPayload)
            {
                callback(false, 0, false, null, null);
                return;
            }

            string payload = Encoding.UTF8.GetString(connectionData);
            var connectionPayload = JsonUtility.FromJson<ConnectionPayload>(payload);

            ConnectStatus gameReturnStatus = ConnectStatus.Success;

            // This stops us from running multiple standalone builds since 
            // they disconnect eachother when trying to join
            //
            // if (clientData.ContainsKey(connectionPayload.clientGUID))
            // {
            //     ulong oldClientId = clientData[connectionPayload.clientGUID].ClientId;
            //     StartCoroutine(WaitToDisconnectClient(oldClientId, ConnectStatus.LoggedInAgain));
            // }

            if (_gameInProgress)
            {
                gameReturnStatus = ConnectStatus.GameInProgress;
            }
            else if (_clientData.Count >= maxPlayers)
            {
                gameReturnStatus = ConnectStatus.ServerFull;
            }

            if (gameReturnStatus == ConnectStatus.Success)
            {
                _clientSceneMap[clientId] = connectionPayload.clientScene;
                _clientIdToGuid[clientId] = connectionPayload.clientGUID;
                _clientData[connectionPayload.clientGUID] = new PlayerData(connectionPayload.playerName, clientId);
            }

            callback(false, 0, true, null, null);

            _gameNetPortal.ServerToClientConnectResult(clientId, gameReturnStatus);

            if (gameReturnStatus != ConnectStatus.Success)
            {
                StartCoroutine(WaitToDisconnectClient(clientId, gameReturnStatus));
            }
        }

        private IEnumerator WaitToDisconnectClient(ulong clientId, ConnectStatus reason)
        {
            _gameNetPortal.ServerToClientSetDisconnectReason(clientId, reason);

            yield return new WaitForSeconds(0);

            KickClient(clientId);
        }

        private void KickClient(ulong clientId)
        {
            NetworkObject networkObject = NetworkSpawnManager.GetPlayerNetworkObject(clientId);
            if (networkObject != null)
            {
                networkObject.Despawn(true);
            }

            NetworkManager.Singleton.DisconnectClient(clientId);
        }
    }
}
