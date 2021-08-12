using System;
using Control;
using Field;
using Player;
using Turrets;
using UnityEngine;

namespace Ui.Input
{
    public class PlayerInputManager : ModiStateMachine
    {
        public LayerMask CellLayerMask;
        public TurretScriptableObject[] turrets;
        private static PlayerInputManager _instance;
        [SerializeField]
        public UI ui;

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

        // turret bauen
        // brauchen wir vllt nicht hier
        public void BuyTurret(Player.Player player,Ressource.RessourceType ressourceEnum, int turret)
        {
            // get turret ressource
            // if ressource costs > player.getressourceoftype -> return;

            HCell deleteLater = new HCell();
            // else
            State.BuyTurret(deleteLater,ressourceEnum,turret);
        }



        // minions kaufen



        // feld einnehmen


        // überprüfungen
        public bool hasEnoughMoney()
        {
            return false;
        }


        /// 
        /// modi
        ///  


        //all minions dead method
        public void BattleStateOn()
        {
            SetState(new BattleState());
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
            }

            
        }
    }
    
}