using UnityEngine;
using UnityEngine.Serialization;

namespace Networking
{
    public class SceneData : MonoBehaviour
    {
        public string sceneMainMenu = "MainMenu";
        public string sceneLobby = "Lobby";
        public string sceneGame = "Game";
    
        public static SceneData Instance;

    
        void Awake()
        {
            if (Instance != null)
            {
                Debug.LogError("Es gibt mehr als einen Buildmanager in der Szene: Bitte nur eine pro Szene!");
                return;
            }

            Instance = this;
        }
    }
}
