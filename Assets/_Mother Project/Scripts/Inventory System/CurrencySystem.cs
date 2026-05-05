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

    private void Awake()
    {
        if(instance == null)
        {
            instance = this;
        }
    }

    public void ItemToAdd(ItemClass item)
    {
        if (item.GetItem() == null) return;

        TemporaryStats stats = InventoryManager.Instance.GetCurrentMC().GetComponent<TemporaryStats>();
        if (stats.CurrentExp < item.itemPrice) return;

        OnItemAdded?.Invoke(item);
        stats.CurrentExp -= item.itemPrice;
    }
    public void ItemToRemove(ItemClass item)
    {
        OnItemRemoved?.Invoke(item);
    }
    public void ItemToUse(ItemClass item)
    {
        OnItemUsed?.Invoke(item);
    }

    public int GetCurrency()
    {
        return InventoryManager.Instance.GetCurrentMC().GetComponent<TemporaryStats>().CurrentExp;
    }
}
