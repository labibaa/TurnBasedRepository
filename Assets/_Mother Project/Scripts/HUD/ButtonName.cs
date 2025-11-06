using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonName : MonoBehaviour
{
    [SerializeField]
    string actionName;

    public string GetButtonActionName()
    {
        return actionName;
    }
    public void SetButtonName(string actionName)
    {
        this.actionName = actionName;
    }
}
