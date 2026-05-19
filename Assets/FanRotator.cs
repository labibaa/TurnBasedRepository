using UnityEngine;

public class FanRotator : MonoBehaviour
{
    [Header("Rotation Settings")]
    [Tooltip("Degrees per second. Negative values reverse direction.")]
    public float rotationSpeed = -45f;

    void Update()
    {
        transform.Rotate(0f, 0f, rotationSpeed * Time.deltaTime, Space.Self);
    }
}