using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class CameraController : MonoBehaviour
{
    // For Framing Transposer we treat zoom as changing camera distance
    private const float MIN_CAMERA_DISTANCE = 6f;
    private const float MAX_CAMERA_DISTANCE = 18f;

    [SerializeField] private CinemachineVirtualCamera cinemachineVirtualCamera;

    private CinemachineFramingTransposer framingTransposer;
    private float targetCameraDistance;

    // track previous grid state so we react only on changes
    private bool prevIsGridOn = false;

    // -------- NEW: how the rig follows the main character when grid is OFF --------
    [Header("Rig follow when Grid is OFF")]
    [SerializeField] private Vector3 rigOffsetFromCharacter = Vector3.zero;
    [SerializeField] private float rigFollowLerpSpeed = 20f;
    [SerializeField] private bool copyRotationWhenGridOff = false;
    // ------------------------------------------------------------------------------

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
                // Grid turned ON -> make the vcam follow & look at this GameObject (the rig)
                cinemachineVirtualCamera.Follow = this.transform;
                cinemachineVirtualCamera.LookAt = this.transform;
            }
            else
            {
                // Grid turned OFF -> follow main character (if any)
                UpdateFollowTarget();

                // Also snap the rig to the main character so there is no jump when toggling back
                SnapRigToMainCharacter();
            }

            prevIsGridOn = isGridOn;
        }

        // If Grid is ON, allow manual camera control (movement/rotation/zoom)
        // These methods move/rotate this GameObject (which the vcam will follow).
        if (isGridOn)
        {
            HandleMovement();
        }

        HandleRotation();
        HandleZoom();

        // when grid is off, camera is controlled by Cinemachine following the mainCharacter
        // and the rig itself will follow the main character in LateUpdate (see below).
    }

    private void LateUpdate()
    {
        // When grid is OFF, keep the rig moving along with the main character
        if (GridSystem.instance != null && !GridSystem.instance.IsGridOn)
        {
            SmoothFollowRigToMainCharacter();
        }
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

    // -------- NEW: Rig follow helpers --------
    private void SmoothFollowRigToMainCharacter()
    {
        if (SwitchMC.Instance == null || SwitchMC.Instance.mainCharacter == null) return;

        Transform mainT = SwitchMC.Instance.mainCharacter.transform;

        // Position follow
        Vector3 targetPos = mainT.position + rigOffsetFromCharacter;
        transform.position = Vector3.Lerp(
            transform.position,
            targetPos,
            rigFollowLerpSpeed * Time.deltaTime
        );

        // Optional: copy rotation
        if (copyRotationWhenGridOff)
        {
            transform.rotation = Quaternion.Lerp(
                transform.rotation,
                mainT.rotation,
                rigFollowLerpSpeed * Time.deltaTime
            );
        }
    }

    private void SnapRigToMainCharacter()
    {
        if (SwitchMC.Instance == null || SwitchMC.Instance.mainCharacter == null) return;

        Transform mainT = SwitchMC.Instance.mainCharacter.transform;
        transform.position = mainT.position + rigOffsetFromCharacter;

        if (copyRotationWhenGridOff)
        {
            transform.rotation = mainT.rotation;
        }
    }
    // -----------------------------------------

    private void HandleMovement()
    {
        Vector3 inputMoveDir = Vector3.zero;

        if (Input.GetKey(KeyCode.W))
            inputMoveDir.z += 1f;
        if (Input.GetKey(KeyCode.S))
            inputMoveDir.z -= 1f;
        if (Input.GetKey(KeyCode.A))
            inputMoveDir.x -= 1f;
        if (Input.GetKey(KeyCode.D))
            inputMoveDir.x += 1f;

        if (inputMoveDir.sqrMagnitude < 0.01f)
            return;

        inputMoveDir = inputMoveDir.normalized;

        float moveSpeed = 10f;

        // Use the *camera's* orientation for movement
        Transform camT = cinemachineVirtualCamera.transform;

        Vector3 forward = camT.forward;
        forward.y = 0f;
        forward.Normalize();

        Vector3 right = camT.right;
        right.y = 0f;
        right.Normalize();

        Vector3 moveVector = forward * inputMoveDir.z + right * inputMoveDir.x;

        // Move the rig (this), camera follows via Cinemachine
        transform.position += moveVector * moveSpeed * Time.deltaTime;
    }

    private void HandleRotation()
    {
        float rotationInput = 0f;
        if (Input.GetKey(KeyCode.Q)) rotationInput += 1f;
        if (Input.GetKey(KeyCode.E)) rotationInput -= 1f;

        if (Mathf.Abs(rotationInput) < 0.01f)
            return;

        float rotationSpeed = 100f;
        float delta = rotationInput * rotationSpeed * Time.deltaTime;

        // Rotate the virtual camera around world Y
        cinemachineVirtualCamera.transform.Rotate(Vector3.up, delta, Space.World);
    }

    private void HandleZoom()
    {
        if (framingTransposer == null) return;

        float zoomAmount = 1f;

        if (Input.mouseScrollDelta.y > 0f)
            targetCameraDistance -= zoomAmount;
        else if (Input.mouseScrollDelta.y < 0f)
            targetCameraDistance += zoomAmount;

        targetCameraDistance = Mathf.Clamp(targetCameraDistance, MIN_CAMERA_DISTANCE, MAX_CAMERA_DISTANCE);

        float zoomSpeed = 10f;
        framingTransposer.m_CameraDistance = Mathf.Lerp(
            framingTransposer.m_CameraDistance,
            targetCameraDistance,
            Time.deltaTime * zoomSpeed
        );
    }
}
