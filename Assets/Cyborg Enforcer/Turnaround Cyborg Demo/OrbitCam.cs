using UnityEngine;
using System.Collections;

public class OrbitCam : MonoBehaviour 
{
	public Transform target;
	public float distance = 5.0f;
	public float xSpeed = 120.0f;
	public float maxZoomSpeed = 1;
	public float sensitivity = 1;
	public float zoomSensitivity = 1;

	public float yAngle;
	
	public float xMinLimit = -20f;
	public float xMaxLimit = 80f;
	
	public float distanceMin = .5f;
	public float distanceMax = 15f;
	
	float x = 0.0f;
	float y = 0.0f;
	float speed = 0;
	float zoomSpeed = 0;
	
	// Use this for initialization
	void Start () {
		QualitySettings.antiAliasing = 8;
		Vector3 angles = transform.eulerAngles;
		x = angles.y;
		y = angles.x;
		
		// Make the rigid body not change rotation
		if (GetComponent<Rigidbody>())
			GetComponent<Rigidbody>().freezeRotation = true;
	}
	
	void LateUpdate () {
		if (target) {
			float targetSpeed = -Input.GetAxis("Horizontal") * xSpeed;
//			if (x < xMinLimit){targetSpeed = Mathf.Max(targetSpeed, 0f);}
//			if (x > xMaxLimit){targetSpeed = Mathf.Min(targetSpeed, 0f);}
			speed = Mathf.MoveTowards(speed, targetSpeed, sensitivity * Time.deltaTime);
			x += speed * 0.02f;
//			y -= Input.GetAxis("Mouse Y") * ySpeed * 0.02f;
			
//			x = ClampAngle(x, xMinLimit, xMaxLimit);
			
			Quaternion rotation = Quaternion.Euler(yAngle, x, 0);

			float targetZoomSpeed = Input.GetAxis("Vertical") * maxZoomSpeed;
			if (distance < distanceMin){targetZoomSpeed = Mathf.Min(targetZoomSpeed, 0f);}
			if (distance > distanceMax){targetZoomSpeed = Mathf.Max(targetZoomSpeed, 0f);}
			zoomSpeed = Mathf.MoveTowards(zoomSpeed, targetZoomSpeed, zoomSensitivity * Time.deltaTime);
			distance -= zoomSpeed;
//			distance = Mathf.Clamp(distance - Input.GetAxis("Vertical")*5, distanceMin, distanceMax);

			Vector3 negDistance = new Vector3(0.0f, 0.0f, -distance);
			Vector3 position = rotation * negDistance + target.position;
			
			transform.rotation = rotation;
			transform.position = position;
			
		}
		
	}
	
	public static float ClampAngle(float angle, float min, float max)
	{
		if (angle < -360F)
			angle += 360F;
		if (angle > 360F)
			angle -= 360F;
		return Mathf.Clamp(angle, min, max);
	}
	
	
}