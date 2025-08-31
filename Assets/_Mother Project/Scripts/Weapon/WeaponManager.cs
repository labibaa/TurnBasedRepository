using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Rendering;
using UnityEngine.TextCore.Text;

public class WeaponManager : MonoBehaviour
{
    public static WeaponManager instance;

    public Dictionary<CurrentWeapon, Func<List<ImprovedActionStat>>> weaponActions;

    //list of weapons to assign to each character according to type
    [SerializeField] protected List<ImprovedActionStat> DaggerAvailableActions = new List<ImprovedActionStat>();
    [SerializeField] protected List<ImprovedActionStat> SwordAvailableActions = new List<ImprovedActionStat>();
    [SerializeField] protected List<ImprovedActionStat> BowAndArrowAvailableActions = new List<ImprovedActionStat>();
    [SerializeField] protected List<ImprovedActionStat> TalismanAvailableActions = new List<ImprovedActionStat>();
    [SerializeField] protected List<ImprovedActionStat> HammerAvailableActions = new List<ImprovedActionStat>();
    [SerializeField] protected List<ImprovedActionStat> AxeAvailableActions = new List<ImprovedActionStat>();
    [SerializeField] protected List<ImprovedActionStat> SpearAvailableActions = new List<ImprovedActionStat>();
    [SerializeField] protected List<ImprovedActionStat> StaffAvailableActions = new List<ImprovedActionStat>();
    [SerializeField] protected List<ImprovedActionStat> SpoonAvailableActions = new List<ImprovedActionStat>();

    public List<ImprovedActionStat> DaggerActiveActions { get; private set; } = new List<ImprovedActionStat>() ;

    private void Awake()
    {
        if(instance == null)
        {
            instance = this;
        }
        weaponActions = new Dictionary<CurrentWeapon, Func<List<ImprovedActionStat>>>
        {
            { CurrentWeapon.Dagger,         GetDaggerAvailableActions },
            { CurrentWeapon.Sword,          GetSwordAvailableActions },
            { CurrentWeapon.BowAndArrow,    GetBowAndArrowAvailableActions },
            { CurrentWeapon.Talisman,       GetTalismanAvailableActions },
            { CurrentWeapon.Hammer,         GetHammerAvailableActions },
            { CurrentWeapon.Axe,            GetAxeAvailableActions },
            { CurrentWeapon.Spear,          GetSpearAvailableActions },
            { CurrentWeapon.Staff,          GetStaffAvailableActions },
            { CurrentWeapon.Spoon,          GetSpoonAvailableActions },
        };

    }
    public void SetDaggerAvailableActions(ImprovedActionStat action)
    {
        DaggerAvailableActions.Add(action);
    }
    public void SetDaggerActiveActions(ImprovedActionStat action)
    {
        DaggerActiveActions.Add(action);
    }
    public List<ImprovedActionStat> GetDaggerActiveActions()
    {
        return DaggerActiveActions;
    }
    public List<ImprovedActionStat> GetDaggerAvailableActions()
    {
        return DaggerAvailableActions;
    }
    public List<ImprovedActionStat> GetSwordAvailableActions()
    {
        return SwordAvailableActions;
    }
    public List<ImprovedActionStat> GetBowAndArrowAvailableActions()
    {
        return BowAndArrowAvailableActions;
    }
    public List<ImprovedActionStat> GetTalismanAvailableActions()
    {
        return TalismanAvailableActions;
    } 
    public List<ImprovedActionStat> GetHammerAvailableActions()
    {
        return HammerAvailableActions;
    }
    public List<ImprovedActionStat> GetAxeAvailableActions()
    {
        return AxeAvailableActions;
    }
    public List<ImprovedActionStat> GetSpearAvailableActions()
    {
        return SpearAvailableActions;
    }
    public List<ImprovedActionStat> GetStaffAvailableActions()
    {
        return StaffAvailableActions;
    }
    public List<ImprovedActionStat> GetSpoonAvailableActions()
    {
        return SpoonAvailableActions;
    }

    public void LoadWeaponData()
    {
        //string fileName = SwitchMC.Instance.mainCharacter.GetComponent<CharacterBaseClasses>().EquipedWeapon + ".json";
        string fileName = "Dagger" + ".json";
        DaggerActiveActions = FileHandler.LoadJsonData<ImprovedActionStat>(fileName);
    }
    public void DefaultWeaponActions()
    {
        DaggerActiveActions.Clear();
        DaggerActiveActions.Add(DAOScriptableObject.instance.GetImprovedActionData(StringData.directory, "DaggerThrow"));
        DaggerActiveActions.Add(DAOScriptableObject.instance.GetImprovedActionData(StringData.directory, "Stab"));
        DaggerActiveActions.Add(DAOScriptableObject.instance.GetImprovedActionData(StringData.directory, "Assassinate"));
        DaggerActiveActions.Add(DAOScriptableObject.instance.GetImprovedActionData(StringData.directory, "DaggerSweep"));
        //string fileName =  SwitchMC.Instance.mainCharacter .GetComponent<CharacterBaseClasses>().EquipedWeapon + ".json";
        string fileName = "Dagger" + ".json";
        FileHandler.SaveToJsonData<ImprovedActionStat>(DaggerActiveActions, fileName);
    }
    public void WeaponLevelUp()
    {
        if (weaponActions.TryGetValue(SwitchMC.Instance.mainCharacter.GetComponent<CharacterBaseClasses>().EquipedWeapon, out var actionGetter)) 
        {
            foreach (var item in actionGetter())
            {
                for (int i = 0; i < item.RangeMappings.Length; i++)
                {
                    item.RangeMappings[i].MappedValue = item.RangeMappings[i].MappedValue + 2;
                }
            }
        }
        else
        {
            Debug.LogWarning("No actions mapped for this weapon!");
        }
    }
}
