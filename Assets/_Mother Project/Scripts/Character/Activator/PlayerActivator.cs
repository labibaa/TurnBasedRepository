using StarterAssets;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerActivator : MonoBehaviour
{
    public void ActivatePlayer()
    {
         GetComponent<ThirdPersonController>().DisableAnim();
         GetComponent<ThirdPersonController>().enabled = false;
        Cursor.lockState = CursorLockMode.None;
    }
    public void DeActivatePlayer()
    {
       
        gameObject.GetComponent<ThirdPersonController>().enabled = true;
    }
}
