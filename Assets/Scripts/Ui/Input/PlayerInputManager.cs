using System;
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
        

        public static PlayerInputManager Instance
        {
            get { return _instance; }
        }
        void Start()
        {
            if (_instance != null && _instance != this)
            {
                Destroy(gameObject);
            }
            else
            {
                _instance = this;
            }

            SetState(new AcquireState());
            State.SetLayerMask(CellLayerMask);
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
            
        }
    }
    
}