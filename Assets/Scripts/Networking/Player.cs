using MLAPI;
using MLAPI.Messaging;
using MLAPI.NetworkVariable;
using UnityEngine;

namespace Networking
{
    public class Player : NetworkBehaviour
    {
        public int maxLifes = 25;
        private int lifes = 25;
        
        public NetworkVariableInt life = new NetworkVariableInt(new NetworkVariableSettings
        {
            WritePermission = NetworkVariablePermission.ServerOnly,
            ReadPermission = NetworkVariablePermission.Everyone
        });
        
       public override void NetworkStart()
       {
          InitLife();
       }
        
        public void LoseLife()
        {
            if (NetworkManager.Singleton.IsServer)
            {
                life.Value--;
                Debug.Log(life.Value + "server");
            }
            else
            {
                SubmitSubtractLifeServerRpc();
            }
        }

        public void InitLife()
        {
            if (NetworkManager.Singleton.IsServer)
            {

                    life.Value = getMaxLifes();
                
            }
            else
            {
                SubmitInitLifeServerRpc();
            }
        }

        [ServerRpc]
        void SubmitSubtractLifeServerRpc(ServerRpcParams rpcParams = default)
        {
            life.Value--;
 
        }
        
        [ServerRpc]
        void SubmitInitLifeServerRpc(ServerRpcParams rpcParams = default)
        {
            life.Value = getMaxLifes();
        }


        int getMaxLifes()
        {
            return maxLifes;
        }

        public int getLifes()
        {
            return lifes;
        }

        void Update()
        {
            lifes = life.Value;
        }
        

    }
}