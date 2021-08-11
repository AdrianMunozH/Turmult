using MLAPI;
using MLAPI.Messaging;
using UnityEngine;

namespace Player
{
    public class LifeManager : NetworkBehaviour
    {

        [ServerRpc]
        public void ChangeLifesServerRpc()
        {
            
        }
    }
}