using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZoomingShield : MonoBehaviour
{
    Camera m_Camera;

    Renderer m_Renderer;
    void Start()
    {
        m_Renderer = GetComponent<Renderer>();
        m_Camera = Camera.main; 
    }

   
    void Update()
    {
        Vector3 ScreenPointTozoom = m_Camera.WorldToScreenPoint(transform.position);
        ScreenPointTozoom.x = ScreenPointTozoom.x /Screen.width;
        ScreenPointTozoom.y = ScreenPointTozoom.y /Screen.height;
        m_Renderer.material.SetVector("_ObjectScreenPosition", ScreenPointTozoom);
    }
}
