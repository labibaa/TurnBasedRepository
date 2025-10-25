using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Analytics;

public class RespawnLoadScene : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && LoadSceneManager.instance.playerDefeatCounter > 0)
        {
           LoadSceneManager.instance.LoadScene(ShowSavedData.Instance.GetDeathScene(SwitchMC.Instance.mainCharacter));
        }

    }

}
