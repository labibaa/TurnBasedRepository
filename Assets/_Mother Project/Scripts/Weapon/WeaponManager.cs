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

    public const int MaxWeaponLevel = 10;
    private const string WeaponLevelsFile = "WeaponLevels.json";

    public Dictionary<CurrentWeapon, Func<List<ImprovedActionStat>>> weaponActions;
    public Dictionary<CurrentWeapon, int> weaponLevels;

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
    [SerializeField] protected List<ImprovedActionStat> ArcherAvailableActions = new List<ImprovedActionStat>();

    public List<ImprovedActionStat> DaggerActiveActions  = new List<ImprovedActionStat>() ;
    public List<ImprovedActionStat> TalismanActiveActions  = new List<ImprovedActionStat>() ;
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
            { CurrentWeapon.Archer,         GetArcherAvailableActions },
        };

        weaponLevels = new Dictionary<CurrentWeapon, int>();
        foreach (CurrentWeapon weapon in Enum.GetValues(typeof(CurrentWeapon)))
        {
            weaponLevels[weapon] = 0;
        }
    }
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.B))
        {
            WeaponManager.instance.DefaultWeaponActions();
        }
    }
    public void SetDaggerAvailableActions(ImprovedActionStat action)
    {
        DaggerAvailableActions.Add(action);
    }
    public void SetWeaponActiveActions(ImprovedActionStat action)
    {
        if(SwitchMC.Instance.mainCharacter.GetComponent<CharacterBaseClasses>().EquipedWeapon == CurrentWeapon.Dagger)
        {
            DaggerActiveActions.Add(action);
        }
        else
        {
            TalismanActiveActions.Add(action);
        }
      
    }
    public List<ImprovedActionStat> GetWeaponActiveActions()
    {
        if (SwitchMC.Instance.mainCharacter.GetComponent<CharacterBaseClasses>().EquipedWeapon == CurrentWeapon.Dagger)
        {
            return DaggerAvailableActions;
        }
        else
        {
            return TalismanAvailableActions;
        }
    }

    public List<ImprovedActionStat> GetDaggerAvailableActions()
    {
        return DaggerActiveActions;
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
        return TalismanActiveActions;
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
    public List<ImprovedActionStat> GetArcherAvailableActions()
    {
        return ArcherAvailableActions;
    }

    public void LoadWeaponData()
    {
        //string fileName = "Dagger" + ".json";
   
        DaggerActiveActions.Clear();
        List<ImprovedActionStat> Dagger = FileHandler.LoadJsonData<ImprovedActionStat>("Dagger.json");
        foreach (var item in Dagger)
        {
            DaggerActiveActions.Add(DAOScriptableObject.instance.GetImprovedActionData(StringData.directory, item.ActionName));
        }
        TalismanActiveActions.Clear();
        List<ImprovedActionStat> Talisman = FileHandler.LoadJsonData<ImprovedActionStat>("Talisman.json");
        foreach (var item in Talisman)
        {
            TalismanActiveActions.Add(DAOScriptableObject.instance.GetImprovedActionData(StringData.directory, item.ActionName));
        }
        
    }
    public void DefaultWeaponActions()
    {
        DaggerActiveActions.Clear();
        DaggerActiveActions.Add(DAOScriptableObject.instance.GetImprovedActionData(StringData.directory, "DaggerThrow"));
        DaggerActiveActions.Add(DAOScriptableObject.instance.GetImprovedActionData(StringData.directory, "Stab"));
        DaggerActiveActions.Add(DAOScriptableObject.instance.GetImprovedActionData(StringData.directory, "Assassinate"));
        DaggerActiveActions.Add(DAOScriptableObject.instance.GetImprovedActionData(StringData.directory, "DaggerSweep"));
        DaggerActiveActions.Add(DAOScriptableObject.instance.GetImprovedActionData(StringData.directory, "DaggerRising"));
       // DaggerActiveActions.Add(DAOScriptableObject.instance.GetImprovedActionData(StringData.directory, "Puncture"));
        DaggerActiveActions.Add(DAOScriptableObject.instance.GetImprovedActionData(StringData.directory, "Imbuement"));
        //string fileName = "Dagger" + ".json";
        FileHandler.SaveToJsonData<ImprovedActionStat>(DaggerActiveActions, "Dagger.json");
        TalismanActiveActions.Clear();
        TalismanActiveActions.Add(DAOScriptableObject.instance.GetImprovedActionData(StringData.directory, "BoneShield"));
        TalismanActiveActions.Add(DAOScriptableObject.instance.GetImprovedActionData(StringData.directory, "BoneSpear"));
        TalismanActiveActions.Add(DAOScriptableObject.instance.GetImprovedActionData(StringData.directory, "SoulTransfer"));
       // TalismanActiveActions.Add(DAOScriptableObject.instance.GetImprovedActionData(StringData.directory, "MagicSiphon"));
        TalismanActiveActions.Add(DAOScriptableObject.instance.GetImprovedActionData(StringData.directory, "SoulSteal"));
        TalismanActiveActions.Add(DAOScriptableObject.instance.GetImprovedActionData(StringData.directory, "Impale"));

        FileHandler.SaveToJsonData<ImprovedActionStat>(TalismanActiveActions, "Talisman.json");

    }

    public void CheckWeaponlvlUpCost_PlayerXp()
    {
        GameObject crntPlayer = SwitchMC.Instance.mainCharacter;
        var stats = crntPlayer.GetComponent<TemporaryStats>();
        var baseClasses = crntPlayer.GetComponent<CharacterBaseClasses>();
        CurrentWeapon weapon = baseClasses.EquipedWeapon;

        if (weaponLevels[weapon] >= MaxWeaponLevel)
        {
            Debug.Log($"{weapon} is already at max level ({MaxWeaponLevel}).");
            return;
        }

        int cost = 10 * baseClasses.Level * baseClasses.Level;
        if (stats.CurrentExp < cost) return;

        stats.CurrentExp -= cost;
        WeaponLevelUp(crntPlayer);
    }
    void WeaponLevelUp(GameObject CurrentCharacter)
    {
        CurrentWeapon weapon = CurrentCharacter.GetComponent<CharacterBaseClasses>().EquipedWeapon;
        if (!weaponActions.TryGetValue(weapon, out var actionGetter))
        {
            Debug.LogWarning("No actions mapped for this weapon!");
            return;
        }

        foreach (var item in actionGetter())
        {
            if (item.RangeMappings == null || item.RangeMappings.Length == 0) continue;

            int topTierIndex = 0;
            for (int i = 1; i < item.RangeMappings.Length; i++)
            {
                if (item.RangeMappings[i].MaxRangeValue > item.RangeMappings[topTierIndex].MaxRangeValue)
                    topTierIndex = i;
            }
            item.RangeMappings[topTierIndex].MappedValue += 1;

            SaveMappings(item);
        }

        weaponLevels[weapon]++;
        SaveWeaponLevels();
    }

    public void LoadWeaponMapping() // call when continue game is pressed
    {
        LoadWeaponLevels();
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

    private void SaveWeaponLevels()
    {
        var entries = new List<WeaponLevelEntry>();
        foreach (var kv in weaponLevels)
        {
            entries.Add(new WeaponLevelEntry { weapon = kv.Key, level = kv.Value });
        }
        FileHandler.SaveToJsonData(entries, WeaponLevelsFile);
    }

    private void LoadWeaponLevels()
    {
        List<WeaponLevelEntry> loaded = FileHandler.LoadJsonData<WeaponLevelEntry>(WeaponLevelsFile);
        if (loaded == null) return;
        foreach (var entry in loaded)
        {
            weaponLevels[entry.weapon] = Mathf.Clamp(entry.level, 0, MaxWeaponLevel);
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

[Serializable]
public struct WeaponLevelEntry
{
    public CurrentWeapon weapon;
    public int level;
}
