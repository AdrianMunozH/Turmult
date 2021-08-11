using System;
using MLAPI;
using MLAPI.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Server
{
    public class MainMenuSceneController : NetworkBehaviour
    {
        private GameObject _connectionMenu;

        private void Start()
        {
            _connectionMenu = GameObject.Find("FormularMultiplayer");
            if(_connectionMenu == null || !SceneManager.GetActiveScene().name.Equals("MainMenu")) throw new Exception("Bitte Erstelle GameObjekt 'FormularMultiplayer'");
            _connectionMenu.SetActive(false);

        }

        public override void NetworkStart()
        {
            base.NetworkStart();
            if (IsClient)
            {
                enabled = false;
                return;
            }
        }

        public void ChangeScene(string name)
        {
            NetworkSceneManager.SwitchScene(name);
        }

        public void ShowConnectionMenu()
        {
            if(_connectionMenu.activeSelf){_connectionMenu.SetActive(false);}
            else
            {
                _connectionMenu.SetActive(true);
            }

        }

    }
}
