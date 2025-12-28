using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DadiSceneInteractor : MonoBehaviour, IInteractable
{
    public void dadiSceneChange()
    {
        Debug.Log("scene change");
        SceneManager.LoadScene(1);
    }

    public void Interact(GameObject p)
    {
        dadiSceneChange();

    }
    public bool IsGridTrigger()
    {
        return false;
    }
}
