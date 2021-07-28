using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneChanger : MonoBehaviour
{

    public Animator transition;
    public float transitionTime = 1.5f;
    //private RespawnPoint rp;

    void Start()
    {
        //rp = GameObject.Find("RespawnPoint").GetComponent<RespawnPoint>();
    }

    void OnTriggerEnter(Collider other)
    {
        StartCoroutine(LoadLevel(SceneManager.GetActiveScene().buildIndex -1));
    }

   IEnumerator LoadLevel(int levelIndex)
   {
       transition.SetTrigger("Start");
       yield return new WaitForSeconds(transitionTime);
       SceneManager.LoadScene(levelIndex);
   }

   public void NextScene()
   {
       StartCoroutine(LoadLevel(SceneManager.GetActiveScene().buildIndex + 1));
   }

   public void LoadSpecificScene(int index)
   {
       StartCoroutine(LoadLevel(index));
   }


   public void ResetLevel()
   {
       /*
       if (rp.respawn)
       {
           StartCoroutine(LoadLevel(SceneManager.GetActiveScene().buildIndex+1));   
           
       }
       else
       {
           StartCoroutine(LoadLevel(SceneManager.GetActiveScene().buildIndex));   
       }
       */
   }
}
