using MLAPI;
using MLAPI.Messaging;
using UnityEngine;

namespace Enemies
{
    public class TestSpawn : NetworkBehaviour
    {
        [SerializeField] private NetworkObject prefab;
        // Start is called before the first frame update
        void Start()
        {
        
        }

        // Update is called once per frame
        void Update()
        {
        
        }

        public void SpawnTestObject()
        {
            SpawnObjectServerRpc();
        }
    
        [ServerRpc]
        private void SpawnObjectServerRpc()
        {
            NetworkObject no = Instantiate(prefab, Vector3.zero, Quaternion.identity);
            no.Spawn();
        }
    }
}
