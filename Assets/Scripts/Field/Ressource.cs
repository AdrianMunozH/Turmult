using Random = UnityEngine.Random;

namespace Field
{
    public class Ressource
    {
        public float chanceRessourceField = 20f;

        public enum RessourceType
        {
            Berg = 0,
            Sumpf = 5,
            Wald = 10,
            Neutral = 15
        };


        private RessourceType type;

        public Ressource()
        {
            SetType();
        }

        public RessourceType GetRessourceType()
        {
            return type;
        }

        /**
     * Setzt den jeweiligen Ressourcentypen
     * Die Wahrscheinlichkeit, dass eine HCELL ein Ressourcenfeld ist liegt bei chanceRessourceField (default 20%).
     * Die Ressourcen-Typen an sich sind gleichmäßig mit 33% verteilt.
     */
        private void SetType()
        {
            if (chanceRessourceField >= Random.Range(1, 100))
            {
                switch (Random.Range(0, 3))
                {
                    case 0:
                        type = RessourceType.Berg;
                        break;
                    case 1:
                        type = RessourceType.Sumpf;
                        break;
                    case 2:
                        type = RessourceType.Wald;
                        break;
                }
            }
            else
            {
                type = RessourceType.Neutral;
            }
        }
    }
}