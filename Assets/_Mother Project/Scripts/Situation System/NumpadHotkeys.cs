using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class NumpadHotkeys : MonoBehaviour
{
    
    public List<GameObject> actionButtons;
    



    void Update()
    {
        if(GridSystem.instance.IsGridOn){


            if (Input.GetKeyDown(KeyCode.Backspace))
            {
                ButtonStackManager.instance.UndoStackEntry();
            }
        }


    }
}
