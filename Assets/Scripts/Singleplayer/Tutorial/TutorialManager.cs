using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.Assertions.Must;
using UnityEngine.SceneManagement;

namespace Singleplayer.Tutorial
{
    public class TutorialManager : MonoBehaviour
    {
        public GameObject[] popUps;
        private int popUpIndex;

        [SerializeField] private GameObject sunkenLand;
        [SerializeField] private GameObject newLand; 
        [SerializeField] private GameObject[] newSunkenLand; 
        [SerializeField] private GameObject newResourceLand;

        [SerializeField] private GameObject[] cylinder;

        [SerializeField] private TextMeshProUGUI resource;

        [SerializeField] private TextMeshProUGUI money;

        [SerializeField] private GameObject towerView;
        [SerializeField] private GameObject towerLock;
        [SerializeField] private GameObject mountainSelector;
        [SerializeField] private GameObject mountainBg;

        [SerializeField] private GameObject towerPreview;
        [SerializeField] private GameObject tower;

        [SerializeField] private GameObject timer;

        [SerializeField] private Animator transition;
        [SerializeField] private AudioSource audio;
        
        

        private void Update()
        {
            for (int i = 0; i < popUps.Length; i++)
            {
                if (i == popUpIndex)
                {
                    popUps[i].SetActive(true);
                }
                else
                {
                    popUps[i].SetActive(false);
                }
            }
            if (popUpIndex == 0)
            {
                if (popUps[popUpIndex].GetComponent<PopUp>().next)
                    popUpIndex++;
            }
            if (popUpIndex == 1)
            {
                if (popUps[popUpIndex].GetComponent<PopUp>().next)
                    popUpIndex++;
            }
            if (popUpIndex == 2)
            {
                if (popUps[popUpIndex].GetComponent<PopUp>().next)
                    popUpIndex++;
            }
            if (popUpIndex == 3)
            {
                if (popUps[popUpIndex].GetComponent<PopUp>().next)
                {
                    cylinder[0].SetActive(true);
                    popUpIndex++;
                }
                    
            }
            if (popUpIndex == 4)
            {
                if (popUps[popUpIndex].GetComponent<PopUp>().next)
                {
                    cylinder[0].SetActive(false);
                    GetLand();
                    money.text = "20";
                    popUpIndex++;
                }
            }
            if (popUpIndex == 5)
            {
                if (popUps[popUpIndex].GetComponent<PopUp>().next)
                    popUpIndex++;
            }
            if (popUpIndex == 6)
            {
                if (popUps[popUpIndex].GetComponent<PopUp>().next)
                {
                    cylinder[1].SetActive(true);
                    popUpIndex++;
                }
                    
            }
            if (popUpIndex == 7)
            {
                if (popUps[popUpIndex].GetComponent<PopUp>().next)
                {
                    cylinder[1].SetActive(false);
                    cylinder[2].SetActive(true);
                    GetResource();
                    resource.text = "1";
                    popUpIndex++;
                }
            }
            if (popUpIndex == 8)
            {
                if (popUps[popUpIndex].GetComponent<PopUp>().next)
                {
                    cylinder[2].SetActive(false);
                    popUpIndex++;
                }
                    
            }
            if (popUpIndex == 9)
            {
                if (popUps[popUpIndex].GetComponent<PopUp>().next)
                {
                    towerView.SetActive(true);
                    mountainSelector.SetActive(true);
                    mountainBg.SetActive(true);
                    popUpIndex++;
                }
                    
            }
            if (popUpIndex == 10)
            {
                if (popUps[popUpIndex].GetComponent<PopUp>().next)
                {
                    towerLock.SetActive(false);
                    resource.text = "0";
                    popUpIndex++;
                }
                    
            }
            if (popUpIndex == 11)
            {
                if (popUps[popUpIndex].GetComponent<PopUp>().next)
                {
                    money.text = "0";
                    towerPreview.SetActive(true);
                    cylinder[2].SetActive(true);
                    popUpIndex++;
                }
            }
            if (popUpIndex == 12)
            {
                if (popUps[popUpIndex].GetComponent<PopUp>().next)
                {
                    
                    towerPreview.SetActive(false);
                    cylinder[2].SetActive(false);
                    tower.SetActive(true);
                    popUpIndex++;
                }
                    
            }
            if (popUpIndex == 13)
            {
                if (popUps[popUpIndex].GetComponent<PopUp>().next)
                {
                    timer.SetActive(true);
                    popUpIndex++;
                }
            }
            if (popUpIndex == 14)
            {
                if (popUps[popUpIndex].GetComponent<PopUp>().next)
                {
                    
                    popUpIndex++;
                }
            }
            if (popUpIndex == 15)
            {
                if (popUps[popUpIndex].GetComponent<PopUp>().next)
                {
                    StartCoroutine(LevelTransition());
                    popUpIndex++;
                }
            }
            if (popUpIndex == 16)
            {
                if (popUps[popUpIndex].GetComponent<PopUp>().next)
                {
                    
                }
            }
            
        }

        private void GetLand()
        {
            sunkenLand.SetActive(false);
            newLand.SetActive(true);
            newLand.transform.DOLocalMoveY(0f, 1f);
            foreach (var land in newSunkenLand)
            {
                land.transform.DOLocalMoveY(0, 1.2f);
            }
        }

        private void GetResource()
        {
            newSunkenLand[0].transform.DOLocalMoveY(-30f,1f);
            newResourceLand.transform.DOLocalMoveY(0f, 1f);
        }

        public void LoadSceneMode()
        {
            foreach (var popUp in popUps)
            {
                popUp.SetActive(false);
            }
            StartCoroutine(LevelTransition());
            this.enabled = false;
        }
        
        IEnumerator LevelTransition()
        {
            transition.gameObject.SetActive(true);
            StartCoroutine(StartFade(audio, 4f, 0f));

            yield return new WaitForSeconds(5f);
            
            SceneManager.LoadScene("Scenes/MainMenu");
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

