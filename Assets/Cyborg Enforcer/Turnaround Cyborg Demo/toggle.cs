using UnityEngine;
using System.Collections;

public class toggle : MonoBehaviour {

	public KeyCode keycode;
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		if (Input.GetKeyUp (keycode))
		{
			GetComponent<Renderer>().enabled =!GetComponent<Renderer>().enabled;
		}
	}
}
