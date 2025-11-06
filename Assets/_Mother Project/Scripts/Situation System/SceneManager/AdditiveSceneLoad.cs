using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;
using UnityEngine.SceneManagement;
using AsyncOperation = UnityEngine.AsyncOperation;

public class AdditiveSceneLoad : MonoBehaviour
{
    public static AdditiveSceneLoad Instance;

    [SerializeField] SceneField[] loadScenes;
    [SerializeField] SceneField[] unloadScenes;
    List<AsyncOperation> sceneLoadingProgress = new List<AsyncOperation>();

    private void Awake()
    {
        if(Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            //var d = new GameObject { name = "[LoadSceneManager]" };
            Destroy(gameObject);
        }
    }
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.N))
        {
            LoadScene();
            UnloadScene();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        LoadScene();
    }

    public void LoadScene()
    {
        for(int i = 0; i < loadScenes.Length; i++)
        {
            bool isSceneLoaded = false;
            for(int j = 0; j< SceneManager.sceneCount; j++)
            {
                Scene loadedScene = SceneManager.GetSceneAt(j); 
                if (loadedScene.name == loadScenes[i].SceneName) {
                isSceneLoaded = true;
                break;
                }
            }

            if (!isSceneLoaded)
            {
                AsyncOperation asyncOperation =  SceneManager.LoadSceneAsync(loadScenes[i], LoadSceneMode.Additive);
                sceneLoadingProgress.Add(asyncOperation);

                //StartCoroutine(SceneLoadProgress());
            }
        }
    }

    public void UnloadScene()
    {
        for(int i = 0; i< unloadScenes.Length; i++)
        {
            for(int j = 0; j< SceneManager.sceneCount; j++)
            {
                Scene loadedScene = SceneManager.GetSceneAt(j);
                if(loadedScene.name == unloadScenes[i].SceneName)
                {
                    SceneManager.UnloadSceneAsync(unloadScenes[i]);
                }
            }
        }
    }

    public IEnumerator SceneLoadProgress()
    {
        foreach(var pro in sceneLoadingProgress)
        {
            pro.allowSceneActivation = false;
            while (!pro.isDone)
            {
               
                if (pro.progress >= 0.9f)
                {
                    pro.allowSceneActivation = true;
                    // UnloadScene();
                }
            }
        }
     
        yield return null;
    }
}
