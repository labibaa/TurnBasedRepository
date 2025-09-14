using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.TextCore.Text;
using UnityEngine.UI;

public class ActionSpawner : MonoBehaviour
{
    public static ActionSpawner Instance;

    public Transform mainPanel;
    public GameObject actionDetailsPanel;
    public TextMeshProUGUI actionDetailsText;
    public Transform parentSlot_selected;
    public Transform actionButtonContainer; // Parent UI panel (e.g. GridLayoutGroup)
    public GameObject actionButtonPrefab;
    bool isWarning;
    private void Awake()
    {
        if (Instance == null) 
        {
            Instance = this;
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.P)) 
        {
            WeaponManager.instance.DefaultWeaponActions();  //in scene loading function
            Loadout();
            FillDefaultSlot();
        }

        if (Input.GetKeyDown(KeyCode.V))
        {
            SwitchMC.Instance.mainCharacter.GetComponent<CharacterBaseClasses>().LevelUp();
            WeaponManager.instance.WeaponLevelUp();
        }
        if (Input.GetKeyDown(KeyCode.B))
        {
            WeaponManager.instance.LoadWeaponMapping();
        }
    }
    public void ShowActionDetails(string details)
    {
        actionDetailsText.text = details;
        actionDetailsPanel.SetActive(true);
    }
    public void FillDefaultSlot()
    {
        int i = 0;
        List<ImprovedActionStat> playerAvailableAction = WeaponManager.instance.GetDaggerActiveActions();
        foreach (Transform child in parentSlot_selected)
        {
            GameObject existButton = Instantiate(actionButtonPrefab, child);
            ObjectDragDrop btn = existButton.GetComponent<ObjectDragDrop>();
            btn.isPrevAction = true;
            btn.ButtonSetup(playerAvailableAction[i].ActionName, playerAvailableAction[i]);
            existButton.GetComponent<Button>().onClick.AddListener(() => ShowActionDetails(playerAvailableAction[i].Description));
            i++;
        }
    }

    public void CheckEmptySlots()
    {
        WeaponManager.instance.DaggerActiveActions.Clear();
        foreach (Transform child in parentSlot_selected)
        {
            if(child.childCount <= 0)
            {
                isWarning = true;
                Debug.Log("Empty Child: " + child.name);
                break;
            }
            else
            {
                isWarning = false;
            }
            Transform existingChild = child.GetChild(0);

            ObjectDragDrop existingScript = existingChild.GetComponent<ObjectDragDrop>();
            if (WeaponManager.instance.DaggerActiveActions.Contains(existingScript.actionScriptable))
            {
                isWarning = true;
                Debug.Log("duplicate actions " + existingScript.actionScriptable);
                break;
            }
            else
            {
                isWarning = false;
            }
            WeaponManager.instance.SetDaggerActiveActions(existingScript.actionScriptable);
        }

        if (!isWarning)
        {
            //string fileName =  SwitchMC.Instance.mainCharacter .GetComponent<CharacterBaseClasses>().EquipedWeapon + ".json";
            string fileName = "Dagger" + ".json";
            FileHandler.SaveToJsonData<ImprovedActionStat>(WeaponManager.instance.DaggerActiveActions, fileName);
        }

    }

    public void Loadout()
    {
        // List<ImprovedActionStat> playerAvailableAction = SwitchMC.Instance.mainCharacter.GetComponent<CharacterBaseClasses>().GetAvailableActions();
        List<ImprovedActionStat> playerAvailableAction = WeaponManager.instance.GetDaggerAvailableActions();
        foreach (ImprovedActionStat scriptable in playerAvailableAction)
        {
            GameObject newButton = Instantiate(actionButtonPrefab, actionButtonContainer);
            ObjectDragDrop btn = newButton.GetComponent<ObjectDragDrop>();
            btn.ButtonSetup(scriptable.ActionName, scriptable);
            newButton.GetComponent<Button>().onClick.AddListener(() => ShowActionDetails(scriptable.Description));
        }
    }
}
