using System.Collections.Generic;
using UnityEngine;

public abstract class CharacterBaseClasses : MonoBehaviour
{
    public string characterName;
    public Sprite avatarHead;
    public string characterClass;
    [SerializeField] private string description;
    [SerializeField] private int level;
    [SerializeField] private int strength;
    [SerializeField] private int dexterity;
    [SerializeField] private int intelligence;
    [SerializeField] private int arcana;
    [SerializeField] private int endurance;
    [SerializeField] private int healthPoints;
    [SerializeField] private int resolvePoints;
    [SerializeField] private float skill;
    [SerializeField] private float mind;
    [SerializeField] float damageMultiplier;
    [SerializeField] int MaxExp;
    [SerializeField] int lootUltiPoints;
    public CurrentWeapon EquipedWeapon;
    [SerializeField]
    protected List<ImprovedActionStat> characterAvailableActions = new List<ImprovedActionStat>();
    [SerializeField]
    protected UltimateActionsFactory playerUltimateFactory;
    protected IUltimate playerUltimate;
    [SerializeField]
    protected ActionStat warpSurge;
    [SerializeField]
    protected ActionStat groundBlast; 
    [SerializeField]
    protected ActionStat dash;
    [SerializeField]
    protected ActionStat move;


    //  [SerializeField] private int baseDamage;


    public string CharacterName { get => characterName; set => characterName = value; }
    public string Description { get => description; set => description = value; }

    public int Level { get => level; set => level = value; }
    public int Strength { get => strength; set => strength = value; }
    public int Dexterity { get => dexterity; set => dexterity = value; }
    public int Intelligence { get => intelligence; set => intelligence = value; }
    public int Arcana { get => arcana; set => arcana = value; }
    public float Skill { get => skill; set => skill = value; }
    public int Endurance { get => endurance; set => endurance = value; }
    public float Mind { get => mind; set => mind = value; }
    public int HealthPoints { get => healthPoints; set => healthPoints = value; }
    public int ResolvePoints { get => resolvePoints; set => resolvePoints = value; }
    // public int BaseDamage { get => baseDamage; set => baseDamage = value; }
    public float DamageMultiplier { get => damageMultiplier; set => damageMultiplier = value; }
    public int MaxExperiencePoint { get => MaxExp; set => MaxExp = value; }
    public int LootUltiPoints { get => lootUltiPoints; set => lootUltiPoints = value; }

    //public float BaseDamage;


    // Add any abstract methods or other members as needed.
    protected virtual void Start()
    {
        if (playerUltimateFactory)
        {
            playerUltimate = playerUltimateFactory.CreateUltimate();
        }
    }
    public abstract void LevelUp();

    protected virtual void AddActionToAbility(ImprovedActionStat improvedAction) { 

        characterAvailableActions.Add(improvedAction);
    }
    protected virtual void RemoveActionToAbility(ImprovedActionStat improvedAction)
    {

        characterAvailableActions.Remove(improvedAction);
    }

    protected virtual bool IsActionUnlocked(ImprovedActionStat improvedAction)
    {

        return characterAvailableActions.Contains(improvedAction);
    }

    public List<ImprovedActionStat> GetAvailableActions()
    {
        return characterAvailableActions;
    }

    public void SetAvailableActions(List<ImprovedActionStat> weaponActions)
    {
         characterAvailableActions = weaponActions;
    }
    public ActionStat GetWarpAction()
    {
        return warpSurge;
    }
    public ActionStat GetDashAction()
    {
        return dash;
    } 
    
    public ActionStat GetMoveAction()
    {
        return move;
    }
    public ActionStat GetGroundBlastAction()
    {
        return groundBlast;
    }
    public IUltimate GetPlayerUltimate()
    {
        return playerUltimate;
    }
    public UltimateActionsFactory GetUltimateScripitable()
    {
        return playerUltimateFactory;
    }
}