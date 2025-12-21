using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoxInteract : MonoBehaviour,IInteractable
{
    public void boxInterracted()
    {
        Debug.Log("Box says hi");
      //  UI.instance.SendNotification("Player Rp is Zero");
    }

    public void Interact(GameObject p)
    {
        boxInterracted();
    }
}
