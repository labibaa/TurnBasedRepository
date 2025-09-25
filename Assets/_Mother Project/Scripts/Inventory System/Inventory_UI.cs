using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class Inventory_UI : MonoBehaviour
{
    [SerializeField] GameObject InventoryHolder;
    [SerializeField] Transform inventoryItem_panel;
    [SerializeField] Button inventoryItem_buttonPrefab;
    [SerializeField] Transform details_Panel;
    [SerializeField] GameObject itemDetailPanel_Prefab;
    [SerializeField] Transform StoreItem_panel;
    [SerializeField] Transform StoreItemDetails_Panel;
    [SerializeField] GameObject storeitemDetailPanel_Prefab;

    [SerializeField] private TextMeshProUGUI currencyText;
    [SerializeField] private TextMeshProUGUI playerName;
    [SerializeField] private Image mainCharacterSprite;
    [SerializeField] private Image secondaryCharacterSprite;

    public StoreObjects store;

    private void Update()
    {
        ShowCurrency();
        ShowMainCharacterSprite();
        ShowSecondaryCharacterSprite();
        RefreshInventoryUI();

        if (Input.GetKeyDown(KeyCode.L))
        {
            //RefreshStoreUI();
        }
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Cursor.lockState = CursorLockMode.Confined;
            InventoryHolder.SetActive(false);

            Time.timeScale = 1f;
        }

        
    }

    public void RefreshInventoryUI()
    {
        foreach (Transform child in inventoryItem_panel)
        {
            Destroy(child.gameObject);
        }

        var inventory = InventoryManager.Instance.GetCurrentInventory();
        if (inventory == null || inventory.Count == 0) return;

        InventoryItem latestItem = inventory[inventory.Count - 1];

        Button ItemButton = Instantiate(inventoryItem_buttonPrefab, inventoryItem_panel);
        TextMeshProUGUI SizeComponent = ItemButton.transform.Find("StackSize_Text").GetComponent<TextMeshProUGUI>();
        Image imgComponent = ItemButton.transform.Find("ItemImg").GetComponent<Image>();
        imgComponent.sprite = latestItem.itemClass.itemIcon;

        if (SizeComponent != null)
        {
            SizeComponent.text = latestItem.StackSize.ToString();
        }

        ItemButton.onClick.AddListener(() =>
        {
            ItemDetails(latestItem);
        });
    }



    public void RefreshStoreUI()
    {
        foreach (Transform child in StoreItem_panel)
        {
            Destroy(child.gameObject);
        }
        foreach (var item in store.storeObjects)
        {
            Button storeButton = Instantiate(inventoryItem_buttonPrefab, StoreItem_panel);
            TextMeshProUGUI SizeComponent = storeButton.transform.Find("StackSize_Text").GetComponent<TextMeshProUGUI>();
            Image imgComponent = storeButton.transform.Find("ItemImg").GetComponent<Image>();
            imgComponent.sprite = item.itemClass.itemIcon;
            if (SizeComponent != null)
            {
                SizeComponent.text = item.StackSize.ToString();
            }
            storeButton.onClick.AddListener(() =>
            {
                  StoreItemDetails(item);   
            });

        }
    }

    public void ItemDetails(InventoryItem item)
    {
        foreach (Transform child in details_Panel)
        {
            Destroy(child.gameObject);
        }
        GameObject itemDetailsPanel = Instantiate(itemDetailPanel_Prefab, details_Panel);
        Button detButton = itemDetailsPanel.transform.Find("Remove_Button").GetComponent<Button>();
        Button useButton = itemDetailsPanel.transform.Find("Use_Button").GetComponent<Button>();
        Image sprite = itemDetailsPanel.transform.Find("Item_Image").GetComponent<Image>();
        sprite.sprite = item.itemClass.itemIcon;
        detButton.onClick.AddListener(() =>
        {
            CurrencySystem.instance.ItemToRemove(item.itemClass);
            RefreshInventoryUI();
        });
        useButton.onClick.AddListener(() =>
        {
            CurrencySystem.instance.ItemToUse(item.itemClass);
            RefreshInventoryUI();
        });

    }

    public void StoreItemDetails(InventoryItem item)
    {
        foreach (Transform child in StoreItemDetails_Panel)
        {
            Destroy(child.gameObject);
        }
        GameObject itemDetailsPanel = Instantiate(storeitemDetailPanel_Prefab, StoreItemDetails_Panel);
        Button detButton = itemDetailsPanel.transform.Find("Add_Button").GetComponent<Button>();
        Image sprite = itemDetailsPanel.transform.Find("Item_Image").GetComponent<Image>();
        sprite.sprite = item.itemClass.itemIcon;
        detButton.onClick.AddListener(() =>
        {
            CurrencySystem.instance.ItemToAdd(item.itemClass);
        });

    }

    public void CloseInventory()
    {
        Cursor.lockState = CursorLockMode.Locked;
        //InventoryHolder.SetActive(false);
        Time.timeScale = 1f;
    }


    public void ShowCurrency()
    {
        if (currencyText != null)
        {
            currencyText.text = CurrencySystem.instance.GetCurrency().ToString();
        }
    }

    public void ShowMainCharacterSprite()
    {
        var currentMC = InventoryManager.Instance.GetCurrentMC();
        if (currentMC != null && mainCharacterSprite != null)
        {
            var avatar = currentMC.GetComponent<TemporaryStats>().avatarHead;
            if (avatar != null)
                mainCharacterSprite.sprite = avatar;

        }
        if (currentMC != null && playerName != null)
        {
            var name = currentMC.GetComponent<CharacterBaseClasses>().characterName;
            if (name != null)
                playerName.text = name;

        }
    }

    public void ShowSecondaryCharacterSprite()
    {
        

        foreach (var character in SwitchMC.Instance.characters)
        {
            
            var tempStats = character.GetComponent<TemporaryStats>();
            if (tempStats.isMainCharacter == false)
            {
                var avatar = character.GetComponent<TemporaryStats>().avatarHead;
               
                    secondaryCharacterSprite.sprite = avatar;
                    break; // Only one secondary character, exit after setting
                
            }
        }
    }
}
