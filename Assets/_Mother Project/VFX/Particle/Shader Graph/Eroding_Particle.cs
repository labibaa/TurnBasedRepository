using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Eroding_Particle : MonoBehaviour
{
    public float erodeDelay = 1.3f;
    public float erodeRefreshRate = 0.01f;
    public float erodeRate = 0.005f;
    public SkinnedMeshRenderer skinnedMeshRendererErodeObject;

    private void Start()
    {
        StartCoroutine(ErodeObject());
    }

    IEnumerator ErodeObject()
    {
        yield return new WaitForSeconds(erodeDelay);

        float t = 0;
        while (t < 1)
        {
            t += erodeRate;
            skinnedMeshRendererErodeObject.material.SetFloat("_AlphaClippingErode", t);
            yield return new WaitForSeconds(erodeRefreshRate);

        }
    }
}
