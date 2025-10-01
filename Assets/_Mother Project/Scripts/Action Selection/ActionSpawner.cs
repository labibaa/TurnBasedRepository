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

    [SerializeField] private TextMeshProUGUI currencyText;
    [SerializeField] private TextMeshProUGUI currentLevel;
    [SerializeField] private Image playerHP;
    [SerializeField] private Image mainCharacterSprite;

    private void Awake()
    {
        if (Instance == null) 
        {
            Instance = this;
        }
    }

    private void Update()
    {
        //if (Input.GetKeyDown(KeyCode.P))
        //{
        //    WeaponManager.instance.DefaultWeaponActions();  //in scene loading function
        //    Loadout();
        //    FillDefaultSlot();
        //}

        if (Input.GetKeyDown(KeyCode.V))
        {
            SwitchMC.Instance.mainCharacter.GetComponent<CharacterBaseClasses>().LevelUp();
           // WeaponManager.instance.WeaponLevelUp();
        }

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
    public void ShowActionDetails(string details)
    {
        actionDetailsText.text = details;
        actionDetailsPanel.SetActive(true);
    }
    public void FillDefaultSlot()
    {
        foreach (Transform child in parentSlot_selected)
        {
            DestroyAllChildren(child);
        }
        int i = 0;
        List<ImprovedActionStat> playerAvailableAction = SwitchMC.Instance.mainCharacter.GetComponent<CharacterBaseClasses>().GetAvailableActions(); 
        foreach (Transform child in parentSlot_selected)
        {
            GameObject existButton = Instantiate(actionButtonPrefab, child);
            ObjectDragDrop btn = existButton.GetComponent<ObjectDragDrop>();
            btn.isPrevAction = true;
            var action = playerAvailableAction[i];
            btn.ButtonSetup(action.ActionName, action);

            existButton.GetComponent<Button>().onClick.AddListener(
                () => ShowActionDetails(action.Description)
            );
            i++;
        }
    }
    public List<ImprovedActionStat> CheckMCWeapon()
    {
        if(SwitchMC.Instance.mainCharacter.GetComponent<CharacterBaseClasses>().EquipedWeapon == CurrentWeapon.Dagger)
        {
            return WeaponManager.instance.DaggerActiveActions;
        }
        else
        {
            return WeaponManager.instance.TalismanActiveActions;
        }
       
    }
    public void CheckEmptySlots()
    {
        CheckMCWeapon().Clear();
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
            if (CheckMCWeapon().Contains(existingScript.actionScriptable))
            {
                isWarning = true;
                Debug.Log("duplicate actions " + existingScript.actionScriptable);
                break;
            }
            else
            {
                isWarning = false;
            }
            //WeaponManager.instance.SetDaggerActiveActions(existingScript.actionScriptable);
            WeaponManager.instance.SetWeaponActiveActions(existingScript.actionScriptable);
        }
      

        if (!isWarning)
        {
            string fileName =  SwitchMC.Instance.mainCharacter .GetComponent<CharacterBaseClasses>().EquipedWeapon + ".json";
           // string fileName = "Dagger" + ".json";
            FileHandler.SaveToJsonData<ImprovedActionStat>(CheckMCWeapon(), fileName);
            Cursor.lockState = CursorLockMode.Locked;
            mainPanel.gameObject.SetActive(false);
            Time.timeScale = 1f;
        }

    }

    public void Loadout()
    {
       DestroyAllChildren(actionButtonContainer);
        List<ImprovedActionStat> playerAvailableAction = WeaponManager.instance.GetWeaponActiveActions();
        //List<ImprovedActionStat> playerAvailableAction = WeaponManager.instance.GetDaggerAvailableActions();
        foreach (ImprovedActionStat scriptable in playerAvailableAction)
        {
            GameObject newButton = Instantiate(actionButtonPrefab, actionButtonContainer);
            ObjectDragDrop btn = newButton.GetComponent<ObjectDragDrop>();
            btn.ButtonSetup(scriptable.ActionName, scriptable);
            newButton.GetComponent<Button>().onClick.AddListener(() => ShowActionDetails(scriptable.Description));
        }
    }

    public static void DestroyAllChildren( Transform parent)
    {
        for (int i = parent.childCount - 1; i >= 0; i--)
        {
            GameObject.Destroy(parent.GetChild(i).gameObject);
        }
    }
}
