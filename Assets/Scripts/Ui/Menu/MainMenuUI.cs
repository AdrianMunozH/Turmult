using System.Collections;
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

        [SerializeField] Animator transition;

        private void Start()
        {
            PlayerPrefs.GetString("PlayerName");
        }

        public void OnSinglePlayerClicked()
        {
            StartCoroutine(LevelTransition());
        }

        public void OnClientClicked()
        {
            PlayerPrefs.SetString("PlayerName", displayNameInputField.text);
            ClientGameNetPortal.Instance.StartClient();
        }

        IEnumerator LevelTransition()
        {
            transition.gameObject.SetActive(true);

            yield return new WaitForSeconds(4f);
            
            SceneManager.LoadScene("Scenes/Game_SinglePlayer");
        }
        
        
    }
}

