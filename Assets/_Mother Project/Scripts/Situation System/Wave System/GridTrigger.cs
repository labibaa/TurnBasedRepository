using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class GridTrigger : MonoBehaviour
{
    public int WaveTriggerID;
    public GameObject GridStartLocation;
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && !GridSystem.instance.IsGridOn )
        {
            if (gameObject.CompareTag("ObjectiveGrid"))
            {
                TempManager.instance.IsObjective = true;
            }
            else
            {
                TempManager.instance.IsObjective = false;
            }
            GridSystem.instance.gridStartLocation = GridStartLocation;
            WaveManager.instance.IniCurrentWave(gameObject); 
            GridSystem.instance.GenerateGridOnButton();
        }
    }
}
