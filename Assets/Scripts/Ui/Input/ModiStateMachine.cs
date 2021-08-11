using UnityEngine;

namespace Ui.Input
{
    public abstract class ModiStateMachine : MonoBehaviour
    {
        protected ModiState State;

        public void SetState(ModiState state)
        {
            State = state;
            State.Start();
        }
    }
}
