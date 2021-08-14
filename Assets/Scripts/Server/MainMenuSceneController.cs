using System;
using System.Collections;
using DG.Tweening;
using MLAPI;
using MLAPI.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Server
{
    public class MainMenuSceneController : NetworkBehaviour
    {
        private GameObject _connectionMenu;
        private Vector3 _menuPos;

        private void Start()
        {
            _connectionMenu = GameObject.Find("FormularMultiplayer");
            if(_connectionMenu == null || !SceneManager.GetActiveScene().name.Equals("MainMenu")) throw new Exception("Bitte Erstelle GameObjekt 'FormularMultiplayer'");
            _connectionMenu.SetActive(false);
            _menuPos = _connectionMenu.transform.position;

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
            
            if (_connectionMenu.activeSelf)
            {
                
            }
            else
            {
                
                _connectionMenu.transform.position = new Vector3(_menuPos.x - 400, _menuPos.y, _menuPos.z);
                _connectionMenu.transform.DOMoveX(_menuPos.x, 0.5f);
                _connectionMenu.SetActive(true);
                
            }

        }

        

    }
}
