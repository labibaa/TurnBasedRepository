using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

public class EndVfx : MonoBehaviour
{
    public List<VisualEffect> effect;

    private void OnEnable()
    {
       BaseDOThandler.OnAnyEffectFinished += EndVfxKey;
    }

    private void OnDisable()
    {
       BaseDOThandler.OnAnyEffectFinished -= EndVfxKey;
       // EndVfxKey();
    }

    public void EndVfxKey()
    {
        
        if (effect != null)
        {
            foreach (VisualEffect vis in effect)
            {
                
                vis.SendEvent("end");
                vis.SendEvent("stop");
                StartCoroutine(DestroyVFX(vis));
            }
            
        }

    }

    public IEnumerator DestroyVFX(VisualEffect desEffect)
    {
        yield return new WaitForSeconds(.1f);
        Destroy(desEffect.gameObject);
        //  Destroy(hiteffect.gameObject);
        
    }
}
