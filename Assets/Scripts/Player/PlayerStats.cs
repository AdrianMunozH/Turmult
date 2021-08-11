using UnityEngine;

namespace Player
{
    public class PlayerStats : MonoBehaviour
    {
        [Header("Spieler Guthaben")]
        private int gold;
        public int startGold = 500;
        private int wald;
        public int startWald = 1;
        private int berg;
        public int startBerg = 1;
        private int sumpf;
        public int startSumpf = 1;


        [Header("Lifes")] 
        private int lifes;
        public int startLives = 20;

        public int Gold
        {
            get => gold;
            set => gold = value;
        }

        public int Wald
        {
            get => wald;
            set => wald = value;
        }

        public int Berg
        {
            get => berg;
            set => berg = value;
        }

        public int Sumpf
        {
            get => sumpf;
            set => sumpf = value;
        }

        public int Lifes
        {
            get => lifes;
            set => lifes = value;
        }

        // Start is called before the first frame update
        void Start()
        {
            gold = startGold;
        }

        // Update is called once per frame
        void Update()
        {
        
        }

        public int getLifes()
        {
            return lifes;
        }
    }
}
