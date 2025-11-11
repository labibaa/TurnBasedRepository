using UnityEngine;
using Cinemachine;

public class IsoMetricToTPS : MonoBehaviour
{
    public CinemachineVirtualCamera isoCamera;
    public CinemachineVirtualCamera tpsCamera;
    public KeyCode switchKey = KeyCode.I;

    private bool isIsometricActive = true;

    private void OnEnable()
    {
        if (isoCamera != null && tpsCamera != null)
        {
            isoCamera.Priority = 5;
            tpsCamera.Priority = 15;
        }
        else
        {
            Debug.LogWarning("Please assign both the isometric and TPS cameras.");
        }
    }
    private void OnDisable()
    {
        if (isoCamera != null && tpsCamera != null)
        {
            isoCamera.Priority = 5;
            tpsCamera.Priority = 5;
        }

    }
    void Start()
    {
        if (isoCamera != null && tpsCamera != null)
        {
            isoCamera.Priority = 5;
            tpsCamera.Priority = 5;
        }
        else
        {
            Debug.LogError("Please assign both the isometric and TPS cameras.");
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(switchKey))
        {
            isIsometricActive = !isIsometricActive;

            if (isIsometricActive)
            {
                isoCamera.Priority = 15;
                tpsCamera.Priority = 5;
            }
            else
            {
                isoCamera.Priority = 5;
                tpsCamera.Priority = 15;
            }
        }
    }
}
