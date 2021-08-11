using DapperDino.UMT.Lobby.Networking;
using MLAPI;
using UnityEngine;

namespace Networking
{
    public class NetworkGuiManager : MonoBehaviour
    {
        void OnGUI()
        {
            GUILayout.BeginArea(new Rect(5, 20, 100, 200));
            if (!NetworkManager.Singleton.IsClient && !NetworkManager.Singleton.IsServer)
            {
                StartButtons();
            }
            else
            {
                StatusLabels();
            }

            GUILayout.EndArea();
        }

        static void StartButtons()
        {
            if (GUILayout.Button("Client")) ClientGameNetPortal.Instance.StartClient();
            if (GUILayout.Button("Server")) GameNetPortal.Instance.StartServer();
        }

        static void StatusLabels()
        {
            var mode = NetworkManager.Singleton.IsHost ?
                "Host" : NetworkManager.Singleton.IsServer ? "Server" : "Client";

            GUILayout.Label("Transport: " +
                            NetworkManager.Singleton.NetworkConfig.NetworkTransport.GetType().Name);
            GUILayout.Label("Mode: " + mode);
        }
    
    }
}
