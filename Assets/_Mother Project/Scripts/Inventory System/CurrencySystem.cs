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

    [SerializeField]protected int CurrentXp = 0; //xp is currency

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
        GameObject currentMC = InventoryManager.Instance.GetCurrentMC();
        if (currentMC.GetComponent<CharacterBaseClasses>().characterName == "Mon")
        {
            if (item.GetItem() != null && item.GetToolObject() != null)
            {
                OnItemAdded?.Invoke(item);

                CurrentXp -= item.itemPrice;
                currentMC.GetComponent<TemporaryStats>().CurrentExp = CurrentXp;
            }
           
        }
        else if (currentMC.GetComponent<CharacterBaseClasses>().characterName == "Roud")
        {
            if (item.GetItem() != null && item.GetConsumableObject() != null)
            {
                OnItemAdded?.Invoke(item);

                CurrentXp -= item.itemPrice;
                currentMC.GetComponent<TemporaryStats>().CurrentExp = CurrentXp;
            }

        }
    }
    public void ItemToRemove(ItemClass item)
    {
        OnItemRemoved?.Invoke(item);
    }
    public void ItemToUse(ItemClass item)
    {
        OnItemUsed?.Invoke(item);
    }

    public void SetCurrency(int xp) //have to set cureency every time item is added
    {
        CurrentXp = xp; //current mc player currrent xp;
    }

    public int GetCurrency()
    {
        return CurrentXp;
    }
}
