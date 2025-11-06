using StarterAssets;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryInteractor : MonoBehaviour, IInteractable
{
    [SerializeField] GameObject InventoryHolder;
    [SerializeField] Inventory_UI inventoryUI;
    public void Interact()
    {
        ActivateInventory();
    }

    public void ActivateInventory()
    {
        inventoryUI.ShowMainCharacterData();
        inventoryUI.RefreshStoreUI();
        inventoryUI.RefreshInventoryUI();
        /*foreach (var player in SwitchMC.Instance.characters)
        {
            player.GetComponent<ThirdPersonController>().enabled = false;
        }*/
        Cursor.lockState = CursorLockMode.None;
        InventoryHolder.SetActive(true);

        Time.timeScale = 0f;
    }

}
