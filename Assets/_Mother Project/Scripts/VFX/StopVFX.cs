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
        SpawnVFX.OnStartVFX += StartVFXEffect;
    }
    private void OnDisable()
    {
        SpawnVFX.OnStopVFX -= StopVFXEffect;    
        SpawnVFX.OnStopVFX -= StartVFXEffect;    
    }
    //subscribe to an event called when the vfx needs to deactivate
    public void StopVFXEffect()
    {
        ToStopVFX.SendEvent("stop");
    }
    public void StartVFXEffect()
    {
        ToStopVFX.SendEvent("create");
    }
}
