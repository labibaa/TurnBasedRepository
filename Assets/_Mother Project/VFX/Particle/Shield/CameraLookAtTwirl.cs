using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraLookAtTwirl : MonoBehaviour
{
  
    Camera CamToLook;
    void Start()
    {
        CamToLook = Camera.main;
    }


    void Update()
    {
        transform.forward = CamToLook.transform.position - transform.position;
    }
}
