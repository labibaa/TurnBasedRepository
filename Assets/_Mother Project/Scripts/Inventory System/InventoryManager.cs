using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.TextCore.Text;

public class InventoryManager : MonoBehaviour
{
    public static InventoryManager Instance;

    // Dictionary to store character inventories (fast lookup)
    private Dictionary<GameObject, Dictionary<ItemClass, InventoryItem>> itemDictionaries = new Dictionary<GameObject, Dictionary<ItemClass, InventoryItem>>();

    private GameObject currentCharacter; // The currently active character

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }
    }

    private void OnEnable()
    {
        CurrencySystem.OnItemAdded += AddItem;
        CurrencySystem.OnItemRemoved += RemoveItem;
        CurrencySystem.OnItemUsed += UseItem;
        SwitchMC.OnCharacterChange += SetCurrentMC;
    }

    private void OnDisable()
    {
        CurrencySystem.OnItemAdded -= AddItem;
        CurrencySystem.OnItemRemoved -= RemoveItem;
        CurrencySystem.OnItemUsed -= UseItem;
        SwitchMC.OnCharacterChange -= SetCurrentMC;
    }

    private void Start()
    {
        if (SwitchMC.Instance.mainCharacter != null)
        {
            SetCurrentMC();
        }
    }

    public void SetCurrentMC()
    {
        if (SwitchMC.Instance == null || SwitchMC.Instance.mainCharacter == null)
        {
            Debug.LogError("SwitchMC or mainCharacter is missing!");
            return;
        }

        currentCharacter = SwitchMC.Instance.mainCharacter;

        if (!itemDictionaries.ContainsKey(currentCharacter))
        {
            itemDictionaries[currentCharacter] = new Dictionary<ItemClass, InventoryItem>();
        }

        TemporaryStats playerStats = currentCharacter.GetComponent<TemporaryStats>();
        if (playerStats == null)
        {
            Debug.LogError("TemporaryStats component missing on mainCharacter.");
            return;
        }

        CurrencySystem.instance.SetCurrency(playerStats.CurrentExp);
    }
    public void SetInventoryItems(GameObject owner)
    {
        if (itemDictionaries.TryGetValue(owner, out var innerDict))
        {
            owner.GetComponent<CharacterBaseClasses>().SetAvailableItems( innerDict.Values.ToList()); // returns all InventoryItems for this GameObject
        }
    }
    Dictionary<ItemClass,InventoryItem> GetInventoryItems(GameObject owner)
    {
        var newDict =  owner.GetComponent<CharacterBaseClasses>().GetAvailableItems().ToDictionary(item => item.itemClass, item => item);
        itemDictionaries[owner] = newDict;
        return newDict;
    }
    public void AddItem(ItemClass item)
    {
        if (currentCharacter == null)
        {
            Debug.LogError("No current character set!");
            return;
        }

        var dictionary = GetInventoryItems(currentCharacter);

        if (dictionary.TryGetValue(item, out InventoryItem inventoryItem))
        {
            inventoryItem.AddToStack();
            Debug.Log($"{item} added to stack for {currentCharacter.name}");
        }
        else
        {
            InventoryItem newInventoryItem = new InventoryItem(item);
            dictionary[item] = newInventoryItem;
            Debug.Log($"{item} added to inventory for {currentCharacter.name}");
        }
        SetInventoryItems(currentCharacter);
        //ShowSavedData.Instance.AddCharacterData(currentCharacter);
    }

    public void RemoveItem(ItemClass item)
    {
        if (currentCharacter == null)
        {
            Debug.LogError("No current character set!");
            return;
        }

        var dictionary = GetInventoryItems(currentCharacter);

        if (dictionary.TryGetValue(item, out InventoryItem inventoryItem))
        {
            inventoryItem.RemoveFromStack();

            if (inventoryItem.StackSize == 0)
            {
                dictionary.Remove(item);
                Debug.Log($"{item} removed from {currentCharacter.name}'s inventory");
            }
        }
        SetInventoryItems(currentCharacter);
       // ShowSavedData.Instance.AddCharacterData(currentCharacter);
    }

    public void UseItem(ItemClass item)
    {
        if (currentCharacter == null)
        {
            Debug.LogError("No current character set!");
            return;
        }

        var dictionary = GetInventoryItems(currentCharacter);

        if (dictionary.TryGetValue(item, out InventoryItem inventoryItem))
        {
            TemporaryStats playerStats = currentCharacter.GetComponent<TemporaryStats>();
            if (playerStats == null)
            {
                Debug.LogError("TemporaryStats component missing on current character.");
                return;
            }

            item.UseObject(playerStats);
            RemoveItem(item);
        }
    }

    

    public List<InventoryItem> GetCurrentInventory()
    {
        if (currentCharacter == null)
        {
            Debug.LogError("No current character set!");
            return new List<InventoryItem>();
        }

        return new List<InventoryItem>(itemDictionaries[currentCharacter].Values);
    }

    public GameObject GetCurrentMC()
    {
        return currentCharacter;
    }
}
