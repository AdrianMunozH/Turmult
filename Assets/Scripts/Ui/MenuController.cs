using Events;
using UnityEngine;
using ValueObjects;

public class MenuController : MonoBehaviour
{
    public GameObject mainMenu;
    public GameObject settings;
    public GameObject credits;
    public GameObject steuerung;
    public SceneChanger SceneChanger;
    public BoolObject showUi; 

    
    public void PlayClickSound()
    {
        //SoundManager.PlaySoundOnce(SoundManager.Sound.MenuClick,SoundAssets._fx,0.15f);
    }
    

    public void OpenSettings()
    {
        settings.SetActive(true);
        mainMenu.SetActive(false);
    }
    
    public void OpenSteuerung()
    {
        steuerung.SetActive(true);
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

    public void OpenMainMenu()
    {
        credits.SetActive(false);
        settings.SetActive(false);
        steuerung.SetActive(false);
        mainMenu.SetActive(true);
    }

    public void BackToMainMenu()
    {
        SceneChanger.LoadSpecificScene(0);
    }

    public void ResetLevel()
    {
        SceneChanger.ResetLevel();
        ToggleUi();
    }
    
    public void ToggleUi()
    {
        ToggleBoolObject.Toggle(showUi);
    }
}
