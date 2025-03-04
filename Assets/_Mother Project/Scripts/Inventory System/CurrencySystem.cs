using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CurrencySystem : MonoBehaviour
{
    public static CurrencySystem instance;

    public static event ItemAdded OnItemAdded;
    public delegate void ItemAdded(ItemClass item);

    public static event ItemRemoved OnItemRemoved;
    public delegate void ItemRemoved(ItemClass item);

    public static event ItemUsed OnItemUsed;
    public delegate void ItemUsed(ItemClass item);
    private void Start()
    {
        if(instance == null)
        {
            instance = this;
        }
    }

    public void ItemToAdd(ItemClass item)
    {
        //add condition to buy the item 
        OnItemAdded?.Invoke(item);
    }
    public void ItemToRemove(ItemClass item)
    {
        OnItemRemoved?.Invoke(item);
    }
    public void ItemToUse(ItemClass item)
    {
        OnItemUsed?.Invoke(item);
    }
}
