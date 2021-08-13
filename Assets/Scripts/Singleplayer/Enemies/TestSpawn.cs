using MLAPI;
using MLAPI.Messaging;
using UnityEngine;

namespace Singleplayer.Enemies
{
    public class TestSpawn : NetworkBehaviour
    {
        [SerializeField] private GameObject prefab;
        // Start is called before the first frame update
        void Start()
        {
        
        }

        // Update is called once per frame
        void Update()
        {
            if (!IsOwner) return;
            if (Input.GetKey(KeyCode.X))
            {
                SpawnTestObject();
            }
        }

        public void SpawnTestObject()
        {
            Debug.Log("Running RPC");
            SpawnObjectServerRpc();

        }
    
        [ServerRpc(RequireOwnership =  false)]
        private void SpawnObjectServerRpc()
        {
            GameObject go = Instantiate(prefab, Vector3.zero, Quaternion.identity);
            go.GetComponent<NetworkObject>().Spawn();
        }
    }
}
