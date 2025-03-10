using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class ItemClass : ScriptableObject
{
    public string itemName;
    public Sprite itemIcon;
    public int itemPrice;

    public abstract ItemClass GetItem();
    public abstract ToolObject GetToolObject();
    public abstract ConsumableObject GetConsumableObject();
    public abstract void UseObject(TemporaryStats player);
}
