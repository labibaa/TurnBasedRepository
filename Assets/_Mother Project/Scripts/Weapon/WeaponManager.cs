using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Rendering;
using UnityEngine.TextCore.Text;
using static ImprovedActionStat;

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
    [SerializeField] protected List<ImprovedActionStat> ButcherAvailableActions = new List<ImprovedActionStat>();

    public List<ImprovedActionStat> DaggerActiveActions { get; private set; } = new List<ImprovedActionStat>() ;
   // public List<RangeMappingSaveData> RangeMappingSaveDatas  = new List<RangeMappingSaveData>() ;

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
            { CurrentWeapon.Butcher,        GetButcherAvailableActions },
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
    public List<ImprovedActionStat> GetButcherAvailableActions()
    {
        return ButcherAvailableActions;
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
                SaveMappings(item);
            }
        }
        else
        {
            Debug.LogWarning("No actions mapped for this weapon!");
        }
    }

    public void LoadWeaponMapping() // call when continue game is pressed 
    {
        foreach(var character in SwitchMC.Instance.characters)
        {
            if (weaponActions.TryGetValue(character.GetComponent<CharacterBaseClasses>().EquipedWeapon, out var actionGetter))
            {
                foreach (var item in actionGetter())
                {
                    LoadMappings(item);
                }
            }
            else
            {
                Debug.LogWarning("No actions mapped for this weapon!");
            }
        }
    }
    public void SaveMappings(ImprovedActionStat actionStat)
    {
       // RangeMappingSaveDatas.Clear();
        RangeMappingSaveData saveData = new RangeMappingSaveData
        {
            mappings = new List<RangeMapping>(actionStat.RangeMappings) // copy array into list
        };
        //RangeMappingSaveDatas.Add(saveData);
        string fileName = actionStat.name + ".json";
        FileHandler.SaveToJsonData(new List<RangeMappingSaveData> { saveData }, fileName);
    }

    public void LoadMappings(ImprovedActionStat actionStat)
    {
        string fileName = actionStat.name + ".json";
        List<RangeMappingSaveData> loadedList = FileHandler.LoadJsonData<RangeMappingSaveData>(fileName);

        if (loadedList != null && loadedList.Count > 0)
        {
            actionStat.RangeMappings = loadedList[0].mappings.ToArray();
            Debug.Log("Loaded RangeMappings for " + actionStat.name);
        }
    }


}


[Serializable]
public class RangeMappingSaveData
{
    public List<RangeMapping> mappings;
}
