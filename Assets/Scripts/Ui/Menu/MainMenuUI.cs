using DapperDino.UMT.Lobby.Networking;
using Networking;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Ui.Lobby
{
    public class MainMenuUI : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private TMP_InputField displayNameInputField;

        private void Start()
        {
            PlayerPrefs.GetString("PlayerName");
        }

        public void OnSinglePlayerClicked()
        {
            SceneManager.LoadScene("Scenes/Game_SinglePlayer");
        }

        public void OnClientClicked()
        {
            PlayerPrefs.SetString("PlayerName", displayNameInputField.text);
            ClientGameNetPortal.Instance.StartClient();
        }
        
        
    }
}

