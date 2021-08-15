using System.Collections;
using System.Diagnostics;
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

        public int startForest = 0;
        private int mountain;
        public int startMountain = 0;
        private int swamp;
        public int startSwamp = 0;
        
        [Header("Spieler Affinität")]
        private int mountainAffinity;
        private int forestAffinity;
        private int swampAffinity;
        
        public int MountainAffinity
        {
            get => mountainAffinity;
            set => mountainAffinity = value;
        }
        
        public int ForestAffinity
        {
            get => forestAffinity;
            set => forestAffinity = value;
        }
        
        public int SwampAffinity
        {
            get => swampAffinity;
            set => swampAffinity = value;
        }

        [Header("Textfelder/Gameobjekte")] 
        public TextMeshProUGUI goldTMP;
        public TextMeshProUGUI mountainTMP;
        public TextMeshProUGUI forestTMP;
        public TextMeshProUGUI swampTMP;
        
        [SerializeField] public AudioSource mountainSong;
        [SerializeField] public AudioSource forestSong;
        [SerializeField] public AudioSource swampSong;

        public AudioSource currentSong;
        
    
        // Start is called before the first frame update
        void Start()
        {
            gold = startGold;
            forest = startForest;
            mountain = startMountain;
            swamp = startSwamp;

            int randomResource = Random.Range(1, 3);
            switch (randomResource)
            {
                case 1:
                    mountain += 1;
                    break;
                case 2:
                    forest += 1;
                    break;
                case 3:
                    swamp += 1;
                    break;
                default:
                    mountain += 1;
                    break;
            }

            //TODO Eventsubscription für Interestberechnung
        }
        

        private void FixedUpdate()
        {
            goldTMP.text = gold.ToString();
            mountainTMP.text = mountain.ToString();
            forestTMP.text = forest.ToString();
            swampTMP.text = swamp.ToString();

        }


        public void SetSong(AudioSource song)
        {
            int volume;
            if (currentSong == song)
            {
                volume = 0;
                StartCoroutine(StartFade(song, 5f, volume));
                currentSong = null;
            }
                
            else
            {
                volume = 1;
                StartCoroutine(StartFade(song, 5f, volume));
                currentSong = song;
            }
        }
        
        public static IEnumerator StartFade(AudioSource audioSource, float duration, float targetVolume)
        {
            float currentTime = 0;
            float start = audioSource.volume;

            while (currentTime < duration)
            {
                currentTime += Time.deltaTime;
                audioSource.volume = Mathf.Lerp(start, targetVolume, currentTime / duration);
                yield return null;
            }
            yield break;
        }


    }
}
