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
    [SerializeField] Button storeItem_buttonPrefab;
    [SerializeField] Transform StoreItem_panel;
    [SerializeField] Transform StoreItemDetails_Panel;
    [SerializeField] GameObject storeitemDetailPanel_Prefab;

    [SerializeField] private TextMeshProUGUI currencyText;
    [SerializeField] private TextMeshProUGUI currentLevel;
    [SerializeField] private Image playerHP;
    [SerializeField] private Image mainCharacterSprite;
  //  [SerializeField] private Image secondaryCharacterSprite;

    public StoreObjects store;

    private void Update()
    {
        //ShowMainCharacterSprite();
        //ShowSecondaryCharacterSprite();
        //RefreshInventoryUI();

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
        var currentMC = InventoryManager.Instance.GetCurrentMC();
        foreach (var item in currentMC.GetComponent<CharacterBaseClasses>().GetAvailableItems())
        {
            Button ItemButton = Instantiate(inventoryItem_buttonPrefab, inventoryItem_panel);
            TextMeshProUGUI SizeComponent = ItemButton.transform.Find("Quantity").GetComponent<TextMeshProUGUI>();
            TextMeshProUGUI name_Txt = ItemButton.transform.Find("Title").GetComponent<TextMeshProUGUI>();
            name_Txt.text = item.itemClass.itemName.ToString();
            Image imgComponent = ItemButton.transform.Find("ICON").GetComponent<Image>();
            imgComponent.sprite = item.itemClass.itemIcon;

            if (SizeComponent != null)
            {
                SizeComponent.text = item.StackSize.ToString();
            }

            ItemButton.onClick.AddListener(() =>
            {
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
            Button storeButton = Instantiate(storeItem_buttonPrefab, StoreItem_panel);
            TextMeshProUGUI priceComponent = storeButton.transform.Find("Price_Text").GetComponent<TextMeshProUGUI>();
            TextMeshProUGUI name_Txt = storeButton.transform.Find("Title").GetComponent<TextMeshProUGUI>();
            name_Txt.text = item.itemClass.itemName.ToString();
            Image imgComponent = storeButton.transform.Find("ICON").GetComponent<Image>();
            imgComponent.sprite = item.itemClass.itemIcon;
            if (priceComponent != null)
            {
                priceComponent.text = item.itemClass.itemPrice.ToString();
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
        TextMeshProUGUI name_Txt = itemDetailsPanel.transform.Find("ActionDetails").GetComponent<TextMeshProUGUI>();
        name_Txt.text = item.itemClass.itemName.ToString();
        detButton.onClick.AddListener(() =>
        {
            CurrencySystem.instance.ItemToRemove(item.itemClass);
            RefreshInventoryUI();
            Destroy(itemDetailsPanel);
        });
        useButton.onClick.AddListener(() =>
        {
            //CurrencySystem.instance.ItemToUse(item.itemClass);
            RefreshInventoryUI();
            Destroy(itemDetailsPanel);
        });

    }

    public void StoreItemDetails(InventoryItem item)
    {
        foreach (Transform child in StoreItemDetails_Panel)
        {
            Destroy(child.gameObject);
        }
        GameObject itemDetailsPanel = Instantiate(storeitemDetailPanel_Prefab, StoreItemDetails_Panel);
        Button addButton = itemDetailsPanel.transform.Find("Yes").GetComponent<Button>();
        Button noButton = itemDetailsPanel.transform.Find("No").GetComponent<Button>();
        TextMeshProUGUI nameTxt = itemDetailsPanel.transform.Find("Name_Text").GetComponent<TextMeshProUGUI>();
        nameTxt.text = item.itemClass.itemName.ToString();
        Image sprite = itemDetailsPanel.transform.Find("Item_Image").GetComponent<Image>();
        sprite.sprite = item.itemClass.itemIcon;
        addButton.onClick.AddListener(() =>
        {
            CurrencySystem.instance.ItemToAdd(item.itemClass);
            currencyText.text = CurrencySystem.instance.GetCurrency().ToString();
            RefreshInventoryUI();
            Destroy(itemDetailsPanel);
        });
        noButton.onClick.AddListener(() =>
        {
            Destroy(itemDetailsPanel);
        });

    }

    public void CloseInventory()
    {
        Cursor.lockState = CursorLockMode.Locked;
        //InventoryHolder.SetActive(false);
        Time.timeScale = 1f;
    }


    public void ShowMainCharacterData()
    {
        var currentMC = InventoryManager.Instance.GetCurrentMC();
       // ShowSavedData.Instance.LoadTemporaryStatsNextScene(currentMC); 

        mainCharacterSprite.sprite = currentMC.GetComponent<TemporaryStats>().avatarHead; 
        playerHP.fillAmount = currentMC.GetComponent<CharacterBaseClasses>().HealthPoints;
        currencyText.text = CurrencySystem.instance.GetCurrency().ToString();
        currentLevel.text = currentMC.GetComponent<CharacterBaseClasses>().Level.ToString();
    }

/*    public void ShowSecondaryCharacterSprite()
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
    }*/
}
