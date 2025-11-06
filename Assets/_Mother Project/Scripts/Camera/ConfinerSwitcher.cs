using Cinemachine;
using UnityEngine;

public class ConfinerSwitcher : MonoBehaviour
{
    public CinemachineVirtualCamera virtualCamera1;  // Assign the first Cinemachine Virtual Camera
    public CinemachineVirtualCamera virtualCamera2;  // Assign the second Cinemachine Virtual Camera
    public BoxCollider roomCollider;                // Assign the BoxCollider for Room

    private CinemachineConfiner confiner1;
    private CinemachineConfiner confiner2;

    void Start()
    {
        // Get the CinemachineConfiner components from both virtual cameras
        confiner1 = virtualCamera1.GetComponent<CinemachineConfiner>();
        confiner2 = virtualCamera2.GetComponent<CinemachineConfiner>();
    }

    // Function to change the confiner for both cameras dynamically
    public void SetConfinerForBothCameras(BoxCollider newCollider)
    {
        // Update the confiner's bounding volume for both cameras
        if (confiner1 != null)
        {
            
            confiner1.m_BoundingVolume = newCollider;
            confiner1.InvalidatePathCache();
        }

        if (confiner2 != null)
        {
            
            confiner2.m_BoundingVolume = newCollider;
            confiner2.InvalidatePathCache();
        }
    }

    // Use trigger enter to switch between the colliders
    private void OnTriggerEnter(Collider other)
    {
        
        if (other.CompareTag("Player"))
        {
            SetConfinerForBothCameras(roomCollider);
        }
    }

    //private void OnTriggerStay(Collider other)
    //{
        
    //    if (other.CompareTag("Player"))
    //    {
    //        SetConfinerForBothCameras(roomCollider);
    //    }
    //}
}
