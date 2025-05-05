using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//using UnityEngine.Mathf;

public class Torch : MonoBehaviour
{
    public Light VariableLights;
    public float range = 1.2f;
    // Start is called before the first frame update
    void Start()
    {
        VariableLights = GetComponent<Light>();
    }

    // Update is called once per frame
    void Update()
    {
        VariableLights.intensity = Mathf.PingPong(Time.time, 1);
        
    }
}
