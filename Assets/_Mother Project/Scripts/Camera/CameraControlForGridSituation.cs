using UnityEngine;
using Cinemachine;

public class CameraControlForGridSituation : MonoBehaviour
{
    public CinemachineVirtualCamera cinemachineCam;
    public float rotationSpeed = 100f;
    public float zoomSpeed = 2f;
    public float minZoom = 5f;
    public float maxZoom = 20f;
    public float fixedHeight = 10f; // Set the desired fixed height

    private CinemachineFramingTransposer framingTransposer;

    void Start()
    {
        if (cinemachineCam == null)
        {
            cinemachineCam = GetComponent<CinemachineVirtualCamera>();
        }

        // Get the FramingTransposer component to handle zoom
        framingTransposer = cinemachineCam.GetCinemachineComponent<CinemachineFramingTransposer>();

        // Ensure the camera starts at the desired height
        Vector3 currentPosition = transform.position;
        transform.position = new Vector3(currentPosition.x, fixedHeight, currentPosition.z);
    }

    void LateUpdate() // Use LateUpdate to allow other updates to complete
    {
        // Check if the grid is on before running any functionality
        //if (GridSystem.instance != null && !GridSystem.instance.IsGridOn)
        //{
        //    return;  // Exit early if the grid is off
        //}

        HandleRotation();
        HandleZoom();

        // Maintain the fixed height after rotation and zoom
        Vector3 currentPosition = transform.position;
        transform.position = new Vector3(currentPosition.x, fixedHeight, currentPosition.z);

        // Lock the X rotation to prevent it from changing
        Vector3 currentRotation = transform.rotation.eulerAngles;
        transform.rotation = Quaternion.Euler(30f, currentRotation.y, currentRotation.z); // Fix X rotation to 30 degrees
    }

    void HandleRotation()
    {
        // Get the current rotation (only Y-axis)
        float yRotation = transform.eulerAngles.y;

        // Rotate the camera on the Y-axis using Q and E keys
        if (Input.GetKey(KeyCode.Q))
        {
            yRotation += rotationSpeed * Time.deltaTime;
        }
        else if (Input.GetKey(KeyCode.E))
        {
            yRotation -= rotationSpeed * Time.deltaTime;
        }

        // Apply the Y-axis rotation, keeping other axes unaffected
        transform.rotation = Quaternion.Euler(30f, yRotation, 0); // Keep X at 30 degrees
    }

    void HandleZoom()
    {
        // Zoom in and out by adjusting the Camera Distance in FramingTransposer
        float scrollInput = Input.GetAxis("Mouse ScrollWheel");
        if (scrollInput != 0f)
        {
            float currentZoom = framingTransposer.m_CameraDistance;

            currentZoom -= scrollInput * zoomSpeed;
            currentZoom = Mathf.Clamp(currentZoom, minZoom, maxZoom);

            // Apply the zoom to the camera's distance
            framingTransposer.m_CameraDistance = currentZoom;
        }
    }
}
