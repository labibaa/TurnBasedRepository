using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
        return this;
    }

    public override ToolObject GetToolObject()
    {
        return this;
    }

    public override void UseObject(TemporaryStats player)
    {
        Debug.Log("Waponsss");
        WeaponManager.instance.SetDaggerAvailableActions(DAOScriptableObject.instance.GetImprovedActionData(StringData.directory, "Buff"));
    }
}
