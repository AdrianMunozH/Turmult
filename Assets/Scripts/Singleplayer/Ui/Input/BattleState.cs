using UnityEngine;

namespace Singleplayer.Ui.Input
{
    public class BattleState : ModiState
    {
        public override void Start()
        {
            name = "Battle";
            Debug.Log(name + " Mode");
        }

        public override void Input()
        {
            throw new System.NotImplementedException();
        }
    }
    
}