using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Analytics;

public class RandomSceneLoad : MonoBehaviour
{
    [SerializeField] List<SceneField> sceneFields = new List<SceneField>();
    [SerializeField] SceneField FinalScene;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            LoadRandomScene();
        }
    }

    public void LoadRandomScene()
    {
         
        if(sceneFields.Count <= 0)
        {
            LoadSceneManager.instance.LoadScene(FinalScene);
        }
        else
        {
            int r = Random.Range(0, sceneFields.Count);
            LoadSceneManager.instance.LoadScene(sceneFields[r]);
            sceneFields.Remove(sceneFields[r]);
        }
        
    }
}
