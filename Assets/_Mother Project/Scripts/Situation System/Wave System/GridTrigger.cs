using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class GridTrigger : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && !GridSystem.instance.IsGridOn )
        {
           // WaveManager.instance.GridStartAssasinate();
            GridSystem.instance.GenerateGridOnButton();
        }
    }
}
