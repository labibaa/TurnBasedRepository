using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Hunter character class specialization.
/// Growth profile: high Dexterity scaling, moderate Strength/Intelligence/Arcana,
/// low Endurance scaling. Individual stat level-ups are fully implemented.
///
/// Stat formula: Stat = Stat + (Level * GrowthFactor) + (Level^2 * ScalingFactor)
/// </summary>
public class HunterClass : CharacterBaseClasses
{
    protected override void Start()
    {
        base.Start();
    }

    /// <summary>
    /// Full level-up: scales DamageMultiplier, increases MaxXP by 100, and increments Level.
    /// Individual stats (HP, STR, DEX, etc.) are leveled separately via their own methods.
    /// </summary>
    public override void LevelUp()
    {
        DamageMultiplier = (int)(DamageMultiplier + (Level * 0.2f) + (Level * Level * 0.01));
        MaxExperiencePoint += 100;
        Level++;
    }

    public override void ArcanaLevelUp()
    {
        Arcana = (int)(Arcana + (Level * 1.5f) + (Level * Level * 0.1));
    }

    public override void DexterityLevelUp()
    {
        Dexterity = (int)(Dexterity + (Level * 1.02f) + (Level * Level * 0.1));
    }

    public override void EnduranceLevelUp()
    {
        Endurance = (int)(Endurance + (Level * 1.5f) + (Level * Level * 0.02));
    }

    public override void HealthLevelUp()
    {
        HealthPoints = (int)(HealthPoints + (Level * 1.2f) + (Level * Level * 0.1));
    }

    public override void IntelligenceLevelUp()
    {
        Intelligence = (int)(Intelligence + (Level * 1.5f) + (Level * Level * 0.1));
    }

    public override void StrengthLevelUp()
    {
        Strength = (int)(Strength + (Level * 1.5f) + (Level * Level * 0.01));
    }

    int GetDexteritydmgValue(int n)
    {
        return Mathf.FloorToInt((n + 3) / 2f) + 1;
    }
    int GetIntelligenceApValue(int n)
    {
        return Mathf.FloorToInt((n + 1) / 2f);
    }
}
