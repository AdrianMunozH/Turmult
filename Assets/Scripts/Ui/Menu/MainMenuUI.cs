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

        [SerializeField] private AudioSource audio;

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
            StartCoroutine(StartFade(audio, 4f, 0f));

            yield return new WaitForSeconds(5f);
            
            SceneManager.LoadScene("Scenes/Game_SinglePlayer");
        }
        public static IEnumerator StartFade(AudioSource audioSource, float duration, float targetVolume)
        {
            float currentTime = 0;
            float start = audioSource.volume;

            while (currentTime < duration)
            {
                currentTime += Time.deltaTime;
                audioSource.volume = Mathf.Lerp(start, targetVolume, currentTime / duration);
                yield return null;
            }
            yield break;
        }
        
        
    }
}

