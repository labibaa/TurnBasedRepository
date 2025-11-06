using Cysharp.Threading.Tasks;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoading : MonoBehaviour
{

    [SerializeField] SceneField SceneName;
    [SerializeField] RandomSceneLoad RandomSceneLoad;
    public String LastPosition;
    public bool IsRandom;

    /* private void OnTriggerEnter(Collider other)
     {
         PlayerPrefs.SetString("LastPosition", LastPosition);
         LoadSceneManager.instance.LoadScene(SceneName);
     }*/

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (LoadSceneManager.instance.ToAddUnlinkedCharacter)
            {
                LoadSceneManager.instance.LoadScene(LoadSceneManager.instance.leftOutcharacters[0].GetComponent<TemporaryStats>().currentScene);
            }
            else if (IsRandom)
            {
                RandomSceneLoad.LoadRandomScene();
            }
            else
            {
                LoadSceneManager.instance.LoadScene(SceneName);
            }
          
        }
        
    }

    //newgame button
    public async void LoadNextScene()
    {
        await LoadSceneManager.instance.NormalSceneLoading(SceneName);
    }
}
