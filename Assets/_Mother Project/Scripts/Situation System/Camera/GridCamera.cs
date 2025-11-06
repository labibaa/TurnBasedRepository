using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class
    GridCamera : MonoBehaviour
{

    [SerializeField]
    CinemachineVirtualCamera gridCam;
    [SerializeField]
    bool isGridCamera;


    private void Awake()
    {
        gridCam = GetComponent<CinemachineVirtualCamera>();
    }

    private void OnEnable()
    {
        //GridSystem.OnGridGeneration += setCamera;
        HealthManager.OnGridDisable += setCamera;
    }

    private void OnDisable()
    {
       // GridSystem.OnGridGeneration -= setCamera;
        HealthManager.OnGridDisable -= setCamera;
    }


    void setCamera()
    {
      
        if (!isGridCamera)
        {
            
            gridCam.Priority = 15;
            isGridCamera = true;
        }
        else
        {
            gridCam.Priority = 11;
            isGridCamera = false;
        }
       
    }
}
