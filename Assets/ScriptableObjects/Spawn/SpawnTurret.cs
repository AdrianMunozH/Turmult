using Turrets;
using UnityEngine;

namespace ScriptableObjects.Spawn
{
    public class SpawnTurret : UnityEngine.MonoBehaviour
    {
        // An instance of the ScriptableObject defined above.
        public TurretScriptableObject spawnManagerValues;

        // This will be appended to the name of the created entities and increment when each is created.
        int instanceNumber = 1;

        void Start()
        {
            
        }
        
    }
}