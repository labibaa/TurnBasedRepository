using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

public class StopVFX : MonoBehaviour
{

    [SerializeField] VisualEffect ToStopVFX;
    private void OnEnable()
    {
        SpawnVFX.OnStopVFX += StopVFXEffect;
    }
    private void OnDisable()
    {
        SpawnVFX.OnStopVFX -= StopVFXEffect;    
    }
    //subscribe to an event called when the vfx needs to deactivate
    public void StopVFXEffect()
    {
        ToStopVFX.Stop();
    }
}
