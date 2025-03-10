using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class InventoryManager : MonoBehaviour
{
    // Singleton instance of the InventoryManager
    public static InventoryManager Instance;

    // Reference to the player's character stats
    [SerializeField] TemporaryStats playerCharacter;

    // List of inventory items
    public List<InventoryItem> InventoryObjects = new List<InventoryItem>();
    // Dictionary to quickly look up items in the inventory
    private Dictionary<ItemClass, InventoryItem> itemDictionary = new Dictionary<ItemClass, InventoryItem>();

    private void OnEnable()
    {
        // Subscribe to item addition and removal events
        CurrencySystem.OnItemAdded += AddItem;
        CurrencySystem.OnItemRemoved += RemoveItem;
        CurrencySystem.OnItemUsed += UseItem;
        SwitchMC.OnCharacterChange += SetCurrentMC;
    }

    private void OnDisable()
    {
        // Unsubscribe from item addition and removal events
        CurrencySystem.OnItemAdded -= AddItem;
        CurrencySystem.OnItemRemoved -= RemoveItem;
        CurrencySystem.OnItemUsed -= UseItem;
        SwitchMC.OnCharacterChange -= SetCurrentMC;
    }

    private void Start()
    {
        // Ensure only one instance of InventoryManager exists
        if (Instance == null)
        {
            Instance = this;
        }
    }

    // Adds an item to the inventory
    public void AddItem(ItemClass item)
    {
        // Check if the item already exists in the inventory
        if (itemDictionary.TryGetValue(item, out InventoryItem inventoryItem))
        {
            // Increase the item stack count if it exists
            inventoryItem.AddToStack();
            Debug.Log(item + " Added to stack");
        }
        else
        {
            // Create a new inventory item and add it to the list and dictionary
            InventoryItem newInventoryItem = new InventoryItem(item);
            InventoryObjects.Add(newInventoryItem);
            itemDictionary.Add(item, newInventoryItem);
            Debug.Log(item + " Added to inventory");
        }
    }

    // Removes an item from the inventory
    public void RemoveItem(ItemClass item)
    {
        // Check if the item exists in the inventory
        if (itemDictionary.TryGetValue(item, out InventoryItem inventoryItem))
        {
            // Reduce the stack size of the item
            inventoryItem.RemoveFromStack();
            // If the stack size reaches zero, remove the item from inventory
            if (inventoryItem.StackSize == 0)
            {
                InventoryObjects.Remove(inventoryItem);
                itemDictionary.Remove(item);
            }
        }
    }

    // Uses an item and removes it from inventory if necessary
    public void UseItem(ItemClass item)
    {
        // Check if the item exists in the inventory
        if (itemDictionary.TryGetValue(item, out InventoryItem inventoryItem))
        {
            // Apply the item's effect to the player character
            item.UseObject(playerCharacter);
            // Reduce the stack size of the item
            inventoryItem.RemoveFromStack();
            // If the stack size reaches zero, remove the item from inventory
            if (inventoryItem.StackSize == 0)
            {
                InventoryObjects.Remove(inventoryItem);
                itemDictionary.Remove(item);
            }
        }
    }

    public void SetCurrentMC()
    {
        playerCharacter = SwitchMC.Instance.mainCharacter.GetComponent<TemporaryStats>();
        CurrencySystem.instance.SetCurrency(playerCharacter.CurrentExp);
    }

}
