using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AddingItem : MonoBehaviour
{
    [SerializeField] ItemClass itemClass;
    public void SelectItem()
    {
        CurrencySystem.instance.ItemToAdd(itemClass);
    }
    public void RemoveSelectedItem()
    {
        CurrencySystem.instance.ItemToRemove(itemClass);
    }
}
