using System;
using MLAPI;
using MLAPI.Messaging;
using MLAPI.NetworkVariable;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Field
{
    public class Ressource : NetworkBehaviour
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
        [SerializeField]
        private NetworkVariable<int> netType;

        void OnEnable()
        {
            netType.OnValueChanged += ValueChanged;
        }
        
        void OnDisable()
        {
            netType.OnValueChanged -= ValueChanged;
        }

        private void ValueChanged(int previousvalue, int newvalue)
        {
            if (!IsServer) return;
            _type = netType.Value;
        }

        public int GetRessourceType()
        {
            return netType.Value;
        }

        public void SetNeutralType()
        {
            if (!IsServer) return;
            netType.Value = (int) RessourceType.Neutral;
        }

        /**
     * Setzt den jeweiligen Ressourcentypen
     * Die Wahrscheinlichkeit, dass eine HCELL ein Ressourcenfeld ist liegt bei chanceRessourceField (default 20%).
     * Die Ressourcen-Typen an sich sind gleichmäßig mit 33% verteilt.
     */
        public void SetRandomType()
        {
            int resType =(int)RessourceType.Neutral;
            if (chanceRessourceField >= Random.Range(1, 100))
            {
                switch (Random.Range(0, 3))
                {
                    case 0:
                        resType = (int) RessourceType.Berg;
                        break;
                    case 1:
                        resType = (int)RessourceType.Sumpf;
                        break;
                    case 2:
                        resType = (int)RessourceType.Wald;
                        break;
                }
            }
            ChangeResourceType(resType);
        }

        public void SetSpecificType(RessourceType ressourceType)
        {
            if (!IsServer) return;
            netType.Value =  (int)ressourceType;
        }
        
        public void ChangeResourceType(int resType)
        {
            if (!IsServer) return;
            netType.Value =  resType;
        }
    }
}