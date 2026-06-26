using UnityEngine;

public class UILookATCamera : MonoBehaviour
{
    private Camera MainCamera;
    private void Start()
    {
        MainCamera = Camera.main;

    }

    private void LateUpdate()
    {
        var rotation = MainCamera.transform.rotation;
        transform.LookAt(transform.position + rotation * Vector3.forward, rotation * Vector3.up);
    }
}
