using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Inventory Weapon", menuName = "ScriptableObjects/Weapon")]
public class WeaponCardObject : ItemClass
{
    public CurrentWeapon weapon;
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
        return null;
    }

    public override void UseObject(TemporaryStats player)
    {
       
    }
}
