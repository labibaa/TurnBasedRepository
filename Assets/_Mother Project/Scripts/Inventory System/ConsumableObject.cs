using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ConsumableObject Inventory", menuName = "ScriptableObjects/Counsumable")]
public class ConsumableObject : ItemClass
{
    public int ObjectBuff;
    public override ConsumableObject GetConsumableObject()
    {
        return this;
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
        player.CurrentHealth = HealthManager.instance.HealthCap(player.GetComponent<CharacterBaseClasses>().HealthPoints, HealthManager.instance.HealthCalculation(-ObjectBuff, player.CurrentHealth));
        Debug.Log(ObjectBuff + " is Healed");
        //ActionArchive.instance.GroundBlast();
    }
}
