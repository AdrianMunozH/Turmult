using UnityEngine;

namespace Ui.Input
{
    public abstract class ModiStateMachine : MonoBehaviour
    {
        protected ModiState State;
        public Player.Player player;

        public void SetState(ModiState state)
        {
            State = state;
            State.Start();
            State.SetPlayer(player);
        }
    }
}
