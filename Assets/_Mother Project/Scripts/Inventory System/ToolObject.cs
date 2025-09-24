using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

[CreateAssetMenu(fileName = "Inventory Tool", menuName = "ScriptableObjects/Tool")]
public class ToolObject : ItemClass
{
    public int toolEffect;
    public override ConsumableObject GetConsumableObject()
    {
        return null;
    }

    public override ItemClass GetItem()
    {
        if (itemPrice <= CurrencySystem.instance.GetCurrency())
        {
            //WeaponManager.instance.SetDaggerAvailableActions(DAOScriptableObject.instance.GetImprovedActionData(StringData.directory, "Punch"));
            return this;
        }
        UI.instance.SendNotification("Not enough EXP");
        return null;
    }

    public override ToolObject GetToolObject()
    {
        return this;
    }

    public override void UseObject(TemporaryStats player)
    {
        Debug.Log("Waponsss");
        WeaponManager.instance.SetDaggerAvailableActions(DAOScriptableObject.instance.GetImprovedActionData(StringData.directory, "Punch"));
    }
}
