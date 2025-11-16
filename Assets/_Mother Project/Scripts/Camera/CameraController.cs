using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class CameraController : MonoBehaviour
{
    // For Framing Transposer we treat zoom as changing camera distance
    private const float MIN_CAMERA_DISTANCE = 2f;
    private const float MAX_CAMERA_DISTANCE = 26f;

    [SerializeField] private CinemachineVirtualCamera cinemachineVirtualCamera;

    private CinemachineFramingTransposer framingTransposer;
    private float targetCameraDistance;

    // track previous grid state so we react only on changes
    private bool prevIsGridOn = false;

    private void OnEnable()
    {
        // update follow target when SwitchMC signals a change
        SwitchMC.OnCharacterChange += UpdateFollowTarget;
    }

    private void OnDisable()
    {
        SwitchMC.OnCharacterChange -= UpdateFollowTarget;
    }

    private void Start()
    {
        if (cinemachineVirtualCamera == null)
        {
            Debug.LogError("Cinemachine Virtual Camera reference is not set on CameraController.");
            enabled = false;
            return;
        }

        framingTransposer = cinemachineVirtualCamera.GetCinemachineComponent<CinemachineFramingTransposer>();

        // if we found a framing transposer, read the initial camera distance; otherwise fallback
        if (framingTransposer != null)
            targetCameraDistance = framingTransposer.m_CameraDistance;
        else
            targetCameraDistance = (MIN_CAMERA_DISTANCE + MAX_CAMERA_DISTANCE) * 0.5f;

        // set initial follow target depending on grid state
        UpdateFollowTarget();

        prevIsGridOn = GridSystem.instance != null && GridSystem.instance.IsGridOn;
    }

    private void Update()
    {
        if (GridSystem.instance == null)
        {
            // fallback: if there's no grid system, keep following main character
            UpdateFollowTarget();
            return;
        }

        bool isGridOn = GridSystem.instance.IsGridOn;

        // react only when grid state changes
        if (isGridOn != prevIsGridOn)
        {
            if (isGridOn)
            {
                // Grid turned ON -> make the vcam follow & look at this GameObject
                cinemachineVirtualCamera.Follow = this.transform;
                cinemachineVirtualCamera.LookAt = this.transform;
            }
            else
            {
                // Grid turned OFF -> follow main character (if any)
                UpdateFollowTarget();
            }

            prevIsGridOn = isGridOn;
        }

        // If Grid is ON, allow manual camera control (movement/rotation/zoom)
        // These methods move/rotate this GameObject (which the vcam will follow).
        if (isGridOn)
        {
            HandleMovement();
            
        }
        else
        {
            // when grid is off, camera is controlled by Cinemachine following the mainCharacter
            // nothing else to do every frame here
        }

        HandleRotation();
        HandleZoom();
    }

    /// <summary>
    /// Ensure the virtual camera follows the current mainCharacter from SwitchMC when grid is off.
    /// </summary>
    private void UpdateFollowTarget()
    {
        // safety checks
        if (cinemachineVirtualCamera == null) return;
        if (GridSystem.instance != null && GridSystem.instance.IsGridOn)
        {
            // if grid is on, do not override the Follow target here
            return;
        }

        if (SwitchMC.Instance != null && SwitchMC.Instance.mainCharacter != null)
        {
            var t = SwitchMC.Instance.mainCharacter.transform;
            // assign both Follow and LookAt to the main character
            cinemachineVirtualCamera.Follow = t;
            cinemachineVirtualCamera.LookAt = t;
        }
        else
        {
            // no main character - clear targets
            cinemachineVirtualCamera.Follow = null;
            cinemachineVirtualCamera.LookAt = null;
        }
    }

    private void HandleMovement()
    {
        Vector3 inputMoveDir = new Vector3(0, 0, 0);
        if (Input.GetKey(KeyCode.W))
        {
            inputMoveDir.z = +1f;
        }
        if (Input.GetKey(KeyCode.S))
        {
            inputMoveDir.z = -1f;
        }
        if (Input.GetKey(KeyCode.A))
        {
            inputMoveDir.x = -1f;
        }
        if (Input.GetKey(KeyCode.D))
        {
            inputMoveDir.x = +1f;
        }

        float moveSpeed = 10f;

        Vector3 moveVector = transform.forward * inputMoveDir.z + transform.right * inputMoveDir.x;
        transform.position += moveVector * moveSpeed * Time.deltaTime;
    }

    private void HandleRotation()
    {
        float rotationInput = 0f;
        if (Input.GetKey(KeyCode.Q)) rotationInput = +1f;
        if (Input.GetKey(KeyCode.E)) rotationInput = -1f;

        float rotationSpeed = 100f;
        float delta = rotationInput * rotationSpeed * Time.deltaTime;

       
        cinemachineVirtualCamera.transform.Rotate(0f, delta, 0f, Space.World);
        
    }


    private void HandleZoom()
    {
        if (framingTransposer == null) return;

        float zoomAmount = 1f;
        if (Input.mouseScrollDelta.y > 0)
        {
            targetCameraDistance -= zoomAmount;
        }
        if (Input.mouseScrollDelta.y < 0)
        {
            targetCameraDistance += zoomAmount;
        }

        targetCameraDistance = Mathf.Clamp(targetCameraDistance, MIN_CAMERA_DISTANCE, MAX_CAMERA_DISTANCE);

        float zoomSpeed = 10f;
        framingTransposer.m_CameraDistance = Mathf.Lerp(framingTransposer.m_CameraDistance, targetCameraDistance, Time.deltaTime * zoomSpeed);
    }
}
