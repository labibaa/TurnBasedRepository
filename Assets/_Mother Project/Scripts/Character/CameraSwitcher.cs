using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;


public class CameraSwitcher : MonoBehaviour
{
    private BoxCollider boxCollider;
    [SerializeField]
    private List<CinemachineVirtualCamera> cameras; // List of all cameras
    [SerializeField]
    private int activeCameraIndex = 0; // Index of the active camera
    private void Start()
    {
        boxCollider = GetComponent<BoxCollider>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (TempManager.instance.currentState == GameStates.StartTurn &&
            GridSystem.instance.IsGridOn &&
            other.gameObject == TurnManager.instance.players[TurnManager.instance.currentPlayerIndex].gameObject)
        {
            SwitchCamera(activeCameraIndex);
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (TempManager.instance.currentState == GameStates.StartTurn &&
            GridSystem.instance.IsGridOn &&
            other.gameObject == TurnManager.instance.players[TurnManager.instance.currentPlayerIndex].gameObject)
        {
            SwitchCamera(activeCameraIndex);
        }
    }

    //private void OnTriggerExit(Collider other)
    //{
    //    if (TempManager.instance.currentState == GameStates.StartTurn &&
    //        GridSystem.instance.IsGridOn &&
    //        other.gameObject == TurnManager.instance.players[TurnManager.instance.currentPlayerIndex].gameObject)
    //    {
    //        ResetCameraPriority();
    //    }
    //}

    private void SwitchCamera(int index)
    {
        for (int i = 0; i < cameras.Count; i++)
        {
            if (i == index)
            {
                cameras[i].Priority = 16; // Set the active camera's priority higher
            }
            else
            {
                cameras[i].Priority = 10; // Lower the priority of other cameras
            }
        }
    }

    private void ResetCameraPriority()
    {
        foreach (var cam in cameras)
        {
            cam.Priority = 10; // Reset all cameras to their lower priority
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.J))
        {
            if (boxCollider != null)
            {
                boxCollider.enabled = !boxCollider.enabled;
            }


        }
    }
}
