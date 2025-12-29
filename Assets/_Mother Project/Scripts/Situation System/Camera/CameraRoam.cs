using UnityEngine;

public enum CameraStyle
{
    freeRoam,
    topDown
}
public class CameraRoam : MonoBehaviour
{
    [Header("FreeRoam Camera Settings")]
    public float lookSpeedH = 1f;
    public float lookSpeedV = 1f;
    public float zoomSpeed = 20f;
    public float dragSpeed = 6f;
    public float panSpeed = 20f;
    public float cameraPanSpeed = 5f;
    private Vector3 lastFreeCameraPosition;
    private Vector3 lastFreeCameraRotation;


    private float yaw = 0f;
    private float pitch = 0f;

    [Header("General Settings")]
    public bool isCamTopDown = false;
    public CameraStyle cameraStyle;

    [Header("TopDown Camera Settings")]
    public Vector3 topDownCameraPosition;
    public Vector3 topDownCameraRotation;

    Camera _camera;
    GameObject target;

    void Start()
    {
        //_camera = GetComponent<Camera>();
        lastFreeCameraPosition = new Vector3(transform.position.x, transform.position.y, transform.position.z);
    }

    void Update()
    {
        //Look around with Right Mouse
        if (Input.GetMouseButton(1))
        {
            yaw = lookSpeedH * Input.GetAxis("Mouse X");
            pitch = lookSpeedV * Input.GetAxis("Mouse Y");

            //transform.eulerAngles = new Vector3(pitch, yaw, 0f);

            transform.Rotate(0, yaw, 0, Space.World);
            transform.Rotate(-pitch, 0, 0);
        }

        if (Input.GetMouseButton(2))
        {
            Vector3 projectVector = Vector3.ProjectOnPlane(Camera.main.transform.forward, Vector3.up);
            transform.Translate(projectVector * Input.GetAxisRaw("Mouse Y") * cameraPanSpeed, Space.World);
            transform.Translate(Camera.main.transform.right * Input.GetAxisRaw("Mouse X") * cameraPanSpeed, Space.World);
        }

        //Movement with W,A,S,D

        if (Input.GetKey("w") && Input.GetMouseButton(1))
        {
            transform.position += transform.forward * (Time.deltaTime * panSpeed);
        }

        if (Input.GetKey("d") && Input.GetMouseButton(1))
        {
            transform.position += transform.right * (Time.deltaTime * panSpeed);
        }

        if (Input.GetKey("a") && Input.GetMouseButton(1))
        {
            transform.position -= transform.right * (Time.deltaTime * panSpeed);
        }

        if (Input.GetKey("s") && Input.GetMouseButton(1))
        {
            transform.position -= transform.forward * (Time.deltaTime * panSpeed);
        }

        //Zoom in and out with Mouse Wheel
       // transform.Translate(0, 0, Input.GetAxis("Mouse ScrollWheel") * zoomSpeed, Space.Self);

    }

    Bounds CalculateBounds(GameObject go)
    {
        Bounds b = new Bounds(go.transform.position, Vector3.zero);
        Object[] rList = go.GetComponentsInChildren(typeof(Renderer));
        foreach (Renderer r in rList)
        {
            b.Encapsulate(r.bounds);
        }
        return b;
    }

    void FocusCameraOnGameObject(Camera c, GameObject go)
    {
        Bounds b = CalculateBounds(go);
        Vector3 max = b.size;
        float radius = Mathf.Max(max.x, Mathf.Max(max.y, max.z));
        float dist = radius / (Mathf.Sin(c.fieldOfView * Mathf.Deg2Rad / 2f));
        c.transform.position = go.transform.position + transform.rotation * Vector3.forward * -dist;
    }
    /*
    public void Interact()
    {

        isCamTopDown = false;
        cameraStyle = CameraStyle.freeRoam;
        ChangeCameraPos(isCamTopDown);

    }

    public void TopDown()
    {

        isCamTopDown = true;
        cameraStyle = CameraStyle.topDown;
        ChangeCameraPos(isCamTopDown);

    }

    public void ChangeCameraPos(bool goTop)
    {
        if (goTop)
        {
            lastFreeCameraPosition = new Vector3(transform.position.x, transform.position.y, transform.position.z);
            lastFreeCameraRotation = new Vector3(transform.rotation.eulerAngles.x, transform.rotation.eulerAngles.y, transform.rotation.eulerAngles.z);
            transform.position = topDownCameraPosition;
            transform.eulerAngles = topDownCameraRotation;

            lookSpeedH = 0f;
            lookSpeedV = 0f;
            zoomSpeed = 2f;
            dragSpeed = 0f;
            panSpeed = 20f;
        }
        else
        {
            transform.position = lastFreeCameraPosition;
            transform.eulerAngles = lastFreeCameraRotation;
            lookSpeedH = 1f;
            lookSpeedV = 1f;
            zoomSpeed = 2f;
            dragSpeed = 6f;
            panSpeed = 20f;
        }
    }
    */
}