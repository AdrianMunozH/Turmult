using Cinemachine;
using UnityEngine;

namespace Singleplayer.Ui.Input
{
    public class PlayerInputManager : ModiStateMachine
    {
        private static PlayerInputManager _instance;
        [SerializeField]
        public UI ui;

        [SerializeField] private MenuController menuController;

        public static PlayerInputManager Instance
        {
            get { return _instance; }
        }
        
        void Awake()
        {
            if (_instance != null && _instance != this)
            {
                Destroy(gameObject);
            }
            else
            {
                _instance = this;
            }
        }
        
        
        
        // UI - aqcuire / build

        
        // input zeugs

        private void Update()
        {
            
            State.Input();


            if (UnityEngine.Input.GetKeyDown("1"))
            {
                ui.SetAcquireMode();
            } else if (UnityEngine.Input.GetKeyDown("2"))
            {
                
                ui.SelectType("Mountain");
            } else if (UnityEngine.Input.GetKeyDown("3"))
            {
                ui.SelectType("Forest");
            } else if (UnityEngine.Input.GetKeyDown("4"))
            {
                ui.SelectType("Swamp");
            }else if (UnityEngine.Input.GetKeyDown("5") && State.name.Equals("Build"))
            {
                if (ui.towerMode)
                {
                    ui.OpenMinionBuildMode();
                }
                else
                {
                    ui.OpenTowerBuildMode();
                }
            }else if (UnityEngine.Input.GetKeyDown(KeyCode.Escape))

            {
                if (!menuController.menuOpen)
                {
                    menuController.gameObject.SetActive(true);
                    Time.timeScale = 0;
                    menuController.OpenMainMenu();
                }

                else
                {
                    menuController.gameObject.SetActive(false);
                    Time.timeScale = 1;
                    menuController.OpenMainMenu();
                }
                    
                
                
            }

            
        }
    }
    
}