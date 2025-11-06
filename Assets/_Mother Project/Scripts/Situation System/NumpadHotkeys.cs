using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class NumpadHotkeys : MonoBehaviour
{
    
    public List<GameObject> actionButtons;
    



    void Update()
    {
        if(GridSystem.instance.IsGridOn && TurnManager.instance.players[TurnManager.instance.currentPlayerIndex].gameObject.tag == "Player"){


            //if (Input.GetKeyDown(KeyCode.Alpha1))
            //{
            //    if (DAOScriptableObject.instance.GetImprovedActionData(StringData.directory, "Heal")?.APCost <= temp.CurrentAP || (DAOScriptableObject.instance.GetActionData(StringData.directory, "Heal")?.APCost <= temp.CurrentAP))
            //        TempManager.instance.ShowTargetList("Heal");
            //}
            //if (Input.GetKeyDown(KeyCode.A))
            //{
            //    if (DAOScriptableObject.instance.GetImprovedActionData(StringData.directory, "DaggerThrow")?.APCost <= temp.CurrentAP || (DAOScriptableObject.instance.GetActionData(StringData.directory, "DaggerThrow")?.APCost <= temp.CurrentAP))
            //        TempManager.instance.ShowTargetList("DaggerThrow");
            //}
            //if (Input.GetKeyDown(KeyCode.S))
            //{
            //    if (DAOScriptableObject.instance.GetImprovedActionData(StringData.directory, "Stab")?.APCost <= temp.CurrentAP || (DAOScriptableObject.instance.GetActionData(StringData.directory, "Stab")?.APCost <= temp.CurrentAP))
            //        TempManager.instance.ShowTargetList("Stab");
            //}
            //if (Input.GetKeyDown(KeyCode.D))
            //{
            //    if (DAOScriptableObject.instance.GetImprovedActionData(StringData.directory, "Punch")?.APCost <= temp.CurrentAP || (DAOScriptableObject.instance.GetActionData(StringData.directory, "Punch")?.APCost <= temp.CurrentAP))
            //        TempManager.instance.ShowTargetList("Punch");
            //}
            if (Input.GetKeyDown(KeyCode.Backspace))
            {
                ButtonStackManager.instance.UndoStackEntry();
            }
            //if (Input.GetKeyDown(KeyCode.Keypad5))
            //{
            //    TriggerButton(4);
            //}
            //if (Input.GetKeyDown(KeyCode.W))
            //{
            //    TriggerButton(5);
            //}
            //if (Input.GetKeyDown(KeyCode.Keypad7))
            //{
            //    TriggerButton(6);
            //}

            //if (Input.GetKeyDown(KeyCode.Keypad9))
            //{
            //    TriggerButton(8);
            //}
        }


    }
}
