using MLAPI;
using UnityEngine;

namespace Networking
{
    
    public class PlayerManager : MonoBehaviour
    {

        //Methode erstellt die Buttons und Label
        void OnGUI()
        {
            GUILayout.BeginArea(new Rect(10, 10, 300, 300));
            if (!NetworkManager.Singleton.IsClient && !NetworkManager.Singleton.IsServer)
            {
                StartButtons();
            }
            else
            {
                StatusLabels();
                LoseLife();
            }
            GUILayout.EndArea();
        }

        //StartButtons
        static void StartButtons()
        {
            if (GUILayout.Button("Host")) NetworkManager.Singleton.StartHost();
            if (GUILayout.Button("Client")) NetworkManager.Singleton.StartClient();
            if (GUILayout.Button("Server")) NetworkManager.Singleton.StartServer();
        }

        //Die Statusalbe und deren Veränderugnen
        static void StatusLabels()
        {
            var mode = NetworkManager.Singleton.IsHost ? "Host" :
                NetworkManager.Singleton.IsServer ? "Server" : "Client";

            GUILayout.Label("Transport: " +
                            NetworkManager.Singleton.NetworkConfig.NetworkTransport.GetType().Name);
            GUILayout.Label("Mode: " + mode);
            if (NetworkManager.Singleton.ConnectedClients.TryGetValue(NetworkManager.Singleton.LocalClientId,
                out var networkedClient))
            {
                var player = networkedClient.PlayerObject.GetComponent<Player>();
                if (player)
                { 
                    GUILayout.Label("Mode: " + player.getLifes());
                }
            }
        }
        

        static void LoseLife()
        {
            if (GUILayout.Button(NetworkManager.Singleton.IsServer ? "Lose Life" : "Request Life Loss"))
            {
                if (NetworkManager.Singleton.ConnectedClients.TryGetValue(NetworkManager.Singleton.LocalClientId,
                    out var networkedClient))
                {
                    var player = networkedClient.PlayerObject.GetComponent<Player>();
                    if (player)
                    { 
                        player.LoseLife();
                    }
                }
            }
        }
        
        
    }
}