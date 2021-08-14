using Events;
using UnityEngine;
using ValueObjects;

namespace Singleplayer.Ui
{
    public class MenuController : MonoBehaviour
    {
        public GameObject mainMenu;
        public GameObject settings;
        public GameObject credits;
        public BoolObject showUi;
        public bool menuOpen = false;

    
        public void PlayClickSound()
        {
            //SoundManager.PlaySoundOnce(SoundManager.Sound.MenuClick,SoundAssets._fx,0.15f);
        }
        public void OpenMainMenu()
        {
            if (!menuOpen)
            {
                credits.SetActive(false);
                settings.SetActive(false);
                mainMenu.SetActive(true);
                menuOpen = true;
            }
            else
            {
                credits.SetActive(false);
                settings.SetActive(false);
                mainMenu.SetActive(false);
                menuOpen = false;
            }
            
        }

        public void BackToMainMenu()
        {
            credits.SetActive(false);
            settings.SetActive(false);
            mainMenu.SetActive(true);
        }
        

        public void OpenSettings()
        {
            settings.SetActive(true);
            mainMenu.SetActive(false);
        }
    

        public void OpenCredits()
        {
            credits.SetActive(true);
            mainMenu.SetActive(false);
        }

        public void QuitGame()
        {
            Application.Quit();
        }
        

        // public void ToggleUi()
        // {
        //     ToggleBoolObject.Toggle(showUi);
        // }
    }
}

