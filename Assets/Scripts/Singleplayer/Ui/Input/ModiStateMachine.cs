using UnityEngine;

namespace Singleplayer.Ui.Input
{
    public abstract class ModiStateMachine : MonoBehaviour
    {
        protected ModiState State;
        public Player.Player player;

        public void SetState(ModiState state)
        {
            // sachen die gemacht werden bevor die state beendet wird -- object wird nicht wie monobehavior zerstört
            if(State != null)
                State.OnDestroy();
            //set
            
            State = state;
            // start methode ist für init von variablen usw.
            State.Start();
            State.SetPlayer(player);
        }
        public ModiState GetState()
        {
            return State;
        }
    }
}
