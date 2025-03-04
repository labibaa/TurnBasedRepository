using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponManager : MonoBehaviour
{
    public static WeaponManager instance;

    //list of weapons to assign to each character according to type
    [SerializeField] protected List<ImprovedActionStat> DaggerAvailableActions = new List<ImprovedActionStat>();
    [SerializeField] protected List<ImprovedActionStat> SwordAvailableActions = new List<ImprovedActionStat>();
    [SerializeField] protected List<ImprovedActionStat> BowAndArrowAvailableActions = new List<ImprovedActionStat>();
    [SerializeField] protected List<ImprovedActionStat> TalismanAvailableActions = new List<ImprovedActionStat>();
    [SerializeField] protected List<ImprovedActionStat> HammerAvailableActions = new List<ImprovedActionStat>();
    [SerializeField] protected List<ImprovedActionStat> AxeAvailableActions = new List<ImprovedActionStat>();
    [SerializeField] protected List<ImprovedActionStat> SpearAvailableActions = new List<ImprovedActionStat>();
    [SerializeField] protected List<ImprovedActionStat> StaffAvailableActions = new List<ImprovedActionStat>();

    private void Awake()
    {
        if(instance == null)
        {
            instance = this;
        }
    }

    public List<ImprovedActionStat> GetDaggerAvailableActions()
    {
        return DaggerAvailableActions;
    }
    public void SetDaggerAvailableActions(ImprovedActionStat action)
    {
        DaggerAvailableActions.Add(action);
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
}
