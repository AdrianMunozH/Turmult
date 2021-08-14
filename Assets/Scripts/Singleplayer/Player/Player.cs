using TMPro;
using Singleplayer.Ui;
using UnityEngine;
using UnityEngine.UI;

namespace Singleplayer.Player
{
    public class Player : MonoBehaviour
    {
        [Header("Spieler Guthaben")]
        private int gold;
        public int startGold = 500;
        public int interestRateInPercentPerRound = 5;
        private int forest;

        public int Gold
        {
            get => gold;
            set => gold = value;
        }

        public int Forest
        {
            get => forest;
            set => forest = value;
        }

        public int Mountain
        {
            get => mountain;
            set => mountain = value;
        }

        public int Swamp
        {
            get => swamp;
            set => swamp = value;
        }

        public int startForest = 1;
        private int mountain;
        public int startMountain = 1;
        private int swamp;
        public int startSwamp = 1;

        [Header("Lifes")] 
        private int lifes;
        public int startLifes = 20;
        private float hpBarValue;
        
        [Header("Lifebar Color")] 
        public Color full = Color.green;
        public Color nearlyFull = Color.cyan;
        public Color medium = Color.yellow;
        public Color low = new Color(0.6f,0.4f,0.2f);
        public Color superlow = Color.red;
        
        [Header("Textfelder/Gameobjekte")] 
        public TextMeshProUGUI goldTMP;
        public TextMeshProUGUI mountainTMP;
        public TextMeshProUGUI forestTMP;
        public TextMeshProUGUI swampTMP;
        public Image hpBar;
        public TextMeshProUGUI gameOVER;
        
    
        // Start is called before the first frame update
        void Start()
        {

            gold = startGold;
            forest = startForest;
            mountain = startMountain;
            swamp = startSwamp;
            lifes = startLifes;
            //Berechnung des Fillamounts
            hpBarValue = (float) lifes / startLifes;
            hpBar.fillAmount = hpBarValue;
            
            //TODO Eventsubscription für Interestberechnung
        }
        

        private void FixedUpdate()
        {
            goldTMP.text = gold.ToString();
            mountainTMP.text = mountain.ToString();
            forestTMP.text = forest.ToString();
            swampTMP.text = swamp.ToString();
            
            //Berechnung des Fillamounts
            hpBarValue = (float) lifes / startLifes;
            hpBar.fillAmount = hpBarValue;
            hpBar.color = getHpBarColor();
        }

        private Color getHpBarColor()
        {
            if (hpBarValue > 0.95)
            {
                return full; 
            }
            else if (hpBarValue > 0.8)
            {
                return nearlyFull;
            }else if (hpBarValue > 0.6)
            {
                return medium;
            }else if (hpBarValue > 0.4)
            {
                return low;
            }
            return superlow;
        }

        public void LoseLife()
        {
            //TODO
        }


        


    }
}
