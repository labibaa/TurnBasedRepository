using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class DelayClass : MonoBehaviour
{
    public static DelayClass instance;
    // Start is called before the first frame update
    void Start()
    {
        if (instance==null)
        {
            instance = this;
        }
    }




    public void AddDelay(float delayTime)
    {
        StartCoroutine(ExecuteDelay(delayTime));
    }

    IEnumerator ExecuteDelay(float delayTime)
    {
        yield return new WaitForSecondsRealtime(delayTime);
    }

}
