using MLAPI;
using MLAPI.Connection;
using MLAPI.NetworkVariable.Collections;

namespace Ui
{
    public class GameUI : NetworkBehaviour
    {
        private NetworkList<GamePlayerState> players = new NetworkList<GamePlayerState>();
        
        public override void NetworkStart()
        {
            if (IsClient)
            {
                players.OnListChanged += HandleGamePlayersStateChanged;
            }

            if (IsServer)
            {

                NetworkManager.Singleton.OnClientConnectedCallback += HandleClientConnected;
                NetworkManager.Singleton.OnClientDisconnectCallback += HandleClientDisconnect;

                foreach (NetworkClient client in NetworkManager.Singleton.ConnectedClientsList)
                {
                    HandleClientConnected(client.ClientId);
                }
            }
        }

        private void HandleClientDisconnect(ulong obj)
        {
            throw new System.NotImplementedException("Client disconnected ..");
        }

        private void HandleClientConnected(ulong obj)
        {
            throw new System.NotImplementedException("Client connected.. ");
        }

        private void HandleGamePlayersStateChanged(NetworkListEvent<GamePlayerState> changeevent)
        {
            throw new System.NotImplementedException();
        }
    }
}