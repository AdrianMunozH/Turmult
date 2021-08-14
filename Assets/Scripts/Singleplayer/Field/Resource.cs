using Field;
using MLAPI;
using MLAPI.NetworkVariable;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Singleplayer.Field
{
    public class Resource
    {
        public float chanceResourceField = 20f;

        public enum ResourceType
        {
            Berg = 0,
            Sumpf = 5,
            Wald = 10,
            Neutral = 15
        };

        private int _type;
        
        public Resource()
        {
            SetRandomType();
        }

        public ResourceType GetResource()
        {
            return (ResourceType) _type;
        }

        /**
     * Setzt den jeweiligen Ressourcentypen
     * Die Wahrscheinlichkeit, dass eine HCELL ein Ressourcenfeld ist liegt bei chanceRessourceField (default 20%).
     * Die Ressourcen-Typen an sich sind gleichmäßig mit 33% verteilt.
     */
        private void SetRandomType()
        {
            _type = (int) ResourceType.Neutral;
            if (chanceResourceField >= Random.Range(1, 100))
            {
                switch (Random.Range(0, 3))
                {
                    case 0:
                        _type = (int) ResourceType.Berg;
                        break;
                    case 1:
                        _type = (int) ResourceType.Sumpf;
                        break;
                    case 2:
                        _type = (int) ResourceType.Wald;
                        break;
                }
            }
        }

        public void SetSpecificType(ResourceType resType)
        {
            _type = (int) resType;
        }
    }
}