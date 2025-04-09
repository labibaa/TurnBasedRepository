using Cinemachine;
using UnityEngine;

public class Cinemachine_CameraShake : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        CinemachineBrain brain = Camera.main.GetComponent<CinemachineBrain>();
        if (brain != null)
        {
            CinemachineVirtualCameraBase activeCam = (CinemachineVirtualCameraBase)brain.ActiveVirtualCamera;
        }
        else
        {
            Debug.LogWarning("No CinemachineBrain found on the main camera. Did you forget to attach it, you chaotic artisan?");
        }
    }
}
