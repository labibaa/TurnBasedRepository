using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon_Interactor : MonoBehaviour, IInteractable
{
    [SerializeField] GameObject WeaponPanelHolder;
    public void Interact()
    {
        ActivateWeaponShop();
    }

    public void ActivateWeaponShop()
    {
        /*foreach (var player in SwitchMC.Instance.characters)
        {
            player.GetComponent<ThirdPersonController>().enabled = false;
        }*/
       // WeaponManager.instance.DefaultWeaponActions();  //in scene loading function
        Cursor.lockState = CursorLockMode.None;
        WeaponPanelHolder.SetActive(true);
        Time.timeScale = 0f;
        ActionSpawner.Instance.Loadout();
        ActionSpawner.Instance.FillDefaultSlot();
    }
}
