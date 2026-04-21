using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Abstract base class for all character types in the game.
/// Defines core stats (HP, STR, DEX, INT, ARC, END), level-up contracts,
/// and manages available actions, items, and ultimate abilities.
/// Subclasses must implement all individual stat level-up methods.
/// </summary>
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
    [SerializeField] protected List<InventoryItem> characterAvailableItems = new List<InventoryItem>();
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

    #region Properties
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
    public float DamageMultiplier { get => damageMultiplier; set => damageMultiplier = value; }
    public int MaxExperiencePoint { get => MaxExp; set => MaxExp = value; }
    public int LootUltiPoints { get => lootUltiPoints; set => lootUltiPoints = value; }
    #endregion

    protected virtual void Start()
    {
        if (playerUltimateFactory)
        {
            playerUltimate = playerUltimateFactory.CreateUltimate();
        }
    }

    #region Upgrade Cost & Stat Projections
    /// <summary>
    /// Returns the XP cost to upgrade any stat at the current level.
    /// Formula: 10 * Level^2
    /// </summary>
    public int GetUpgradeCost()
    {
        return 10 * level * level;
    }

    /// <summary>
    /// Returns the projected HP value after a health level-up at the current level.
    /// Formula: HP + (Level * 1.2) + (Level^2 * 0.1)
    /// </summary>
    public int GetProjectedHealthPoints()
    {
        return (int)(healthPoints + (level * 1.2f) + (level * level * 0.1));
    }
    #endregion

    #region Abstract Level-Up Methods
    /// <summary>Performs a full level-up: increments level and scales DamageMultiplier/MaxXP. Subclass-specific.</summary>
    public abstract void LevelUp();
    /// <summary>Increases HealthPoints using class-specific growth formula.</summary>
    public abstract void HealthLevelUp();
    /// <summary>Increases Dexterity using class-specific growth formula.</summary>
    public abstract void DexterityLevelUp();
    /// <summary>Increases Arcana using class-specific growth formula.</summary>
    public abstract void ArcanaLevelUp();
    /// <summary>Increases Intelligence using class-specific growth formula.</summary>
    public abstract void IntelligenceLevelUp();
    /// <summary>Increases Strength using class-specific growth formula.</summary>
    public abstract void StrengthLevelUp();
    /// <summary>Increases Endurance using class-specific growth formula.</summary>
    public abstract void EnduranceLevelUp();
    #endregion

    #region Action & Item Management
    /// <summary>Adds an action to this character's available ability list.</summary>
    protected virtual void AddActionToAbility(ImprovedActionStat improvedAction)
    {
        characterAvailableActions.Add(improvedAction);
    }

    /// <summary>Removes an action from this character's available ability list.</summary>
    protected virtual void RemoveActionToAbility(ImprovedActionStat improvedAction)
    {
        characterAvailableActions.Remove(improvedAction);
    }

    /// <summary>Checks whether a specific action has been unlocked for this character.</summary>
    protected virtual bool IsActionUnlocked(ImprovedActionStat improvedAction)
    {
        return characterAvailableActions.Contains(improvedAction);
    }

    public List<ImprovedActionStat> GetAvailableActions()
    {
        return characterAvailableActions;
    }
    public List<InventoryItem> GetAvailableItems()
    {
        return characterAvailableItems;
    }
    public void SetAvailableItems(List<InventoryItem> items)
    {
       characterAvailableItems = items;
    }
    public void SetAvailableActions(List<ImprovedActionStat> weaponActions)
    {
         characterAvailableActions = weaponActions;
    }
    #endregion

    #region Action Stat Getters
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
    #endregion

    #region Ultimate
    public IUltimate GetPlayerUltimate()
    {
        return playerUltimate;
    }
    public UltimateActionsFactory GetUltimateScripitable()
    {
        return playerUltimateFactory;
    }
    #endregion
}
