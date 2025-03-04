using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class Inventory_UI : MonoBehaviour
{
    [SerializeField] Transform inventoryItem_panel;
    [SerializeField] Button inventoryItem_buttonPrefab;
    [SerializeField] Transform details_Panel;
    [SerializeField] GameObject itemDetailPanel_Prefab;
    [SerializeField] Transform StoreItem_panel;
    [SerializeField] Transform StoreItemDetails_Panel;
    [SerializeField] GameObject storeitemDetailPanel_Prefab;

    public StoreObjects store;
    public void RefreshInventoryUI()
    {
        foreach (Transform child in inventoryItem_panel)
        {
            Destroy(child.gameObject);
        }
        foreach (var item in InventoryManager.Instance.InventoryObjects)
        {
            Button ItemButton = Instantiate(inventoryItem_buttonPrefab,inventoryItem_panel);
            TextMeshProUGUI SizeComponent = ItemButton.transform.Find("StackSize_Text").GetComponent<TextMeshProUGUI>();
            ItemButton.GetComponent<Image>().sprite = item.itemClass.itemIcon;
            if (SizeComponent != null)
            {
                SizeComponent.text = item.StackSize.ToString();
            }
            ItemButton.onClick.AddListener(() =>
            {
                /// CurrencySystem.instance.ItemToAdd(item.itemClass);
                ItemDetails(item);
            });
            
        }
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
            storeButton.GetComponent<Image>().sprite = item.itemClass.itemIcon;
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
}
