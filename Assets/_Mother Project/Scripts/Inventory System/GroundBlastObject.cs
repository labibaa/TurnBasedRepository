using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Ground Blast", menuName = "ScriptableObjects/GroundBlast")]
public class GroundBlastObject : ItemClass
{
    public override ConsumableObject GetConsumableObject()
    {
        return null;
    }

    public override ItemClass GetItem()
    {
        if (itemPrice <= CurrencySystem.instance.GetCurrency())
        {
            return this;
        }
        return null;
    }

    public override ToolObject GetToolObject()
    {
        return null;
    }

    public override void UseObject(TemporaryStats player)
    {
        ActionArchive.instance.GroundBlast();
    }
}
