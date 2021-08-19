using System.Collections;
using DapperDino.UMT.Lobby.Networking;
using Networking;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Ui.Menu
{
    public class MainMenuUI : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private TMP_InputField displayNameInputField;

        [SerializeField] Animator transition;

        [SerializeField] private AudioSource audio;

        [SerializeField] private GameObject[] tipps;

        private void Start()
        {
            PlayerPrefs.GetString("PlayerName");
        }

        public void OnSinglePlayerClicked()
        {
            StartCoroutine(LevelTransition("Scenes/Game_SinglePlayer"));
        }

        public void OnTutorialClicked()
        {
            StartCoroutine(LevelTransition("Scenes/Tutorial"));
        }

        public void OnClientClicked()
        {
            PlayerPrefs.SetString("PlayerName", displayNameInputField.text);
            ClientGameNetPortal.Instance.StartClient();
        }

        IEnumerator LevelTransition(string name)
        {
            transition.gameObject.SetActive(true);
            StartCoroutine(StartFade(audio, 4f, 0f));
            yield return new WaitForSeconds(1f);
            tipps[Random.Range(0,4)].SetActive(true);

            yield return new WaitForSeconds(5f);
            
            SceneManager.LoadScene(name);
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

