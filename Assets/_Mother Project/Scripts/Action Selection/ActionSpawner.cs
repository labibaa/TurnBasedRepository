using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActionSpawner : MonoBehaviour
{
    public static ActionSpawner Instance;

    public Transform mainPanel;
    public GameObject actionDetails;

    private void Awake()
    {
        if (Instance == null) 
        {
            Instance = this;
        }
    }

    public void ShowActionDetails()
    {
        actionDetails.SetActive(true);
    }
}
