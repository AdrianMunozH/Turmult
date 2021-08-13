using MLAPI;
using MLAPI.NetworkVariable;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Singleplayer.Field
{
    public class Ressource : MonoBehaviour
    {
        public float chanceRessourceField = 20f;

        public enum RessourceType
        {
            Berg = 0,
            Sumpf = 5,
            Wald = 10,
            Neutral = 15
        };

        public int _type;

        public int GetRessourceType()
        {
            return _type;
        }

        Ressource()
        {
            SetRandomType();
        }

        /**
     * Setzt den jeweiligen Ressourcentypen
     * Die Wahrscheinlichkeit, dass eine HCELL ein Ressourcenfeld ist liegt bei chanceRessourceField (default 20%).
     * Die Ressourcen-Typen an sich sind gleichmäßig mit 33% verteilt.
     */
        private void SetRandomType()
        {
            _type =(int)RessourceType.Neutral;
            if (chanceRessourceField >= Random.Range(1, 100))
            {
                switch (Random.Range(0, 3))
                {
                    case 0:
                        _type = (int) RessourceType.Berg;
                        break;
                    case 1:
                        _type = (int)RessourceType.Sumpf;
                        break;
                    case 2:
                        _type = (int)RessourceType.Wald;
                        break;
                }
            }
        }
    }
}