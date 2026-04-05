using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Caster character class specialization.
/// Growth profile: high Intelligence/Arcana scaling, moderate HP/Dexterity,
/// low Strength scaling. All stat level-up methods are fully implemented.
///
/// Stat formula: Stat = Stat + (Level * GrowthFactor) + (Level^2 * ScalingFactor)
/// </summary>
public class CasterClass : CharacterBaseClasses
{
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
        HealthPoints = (int)(HealthPoints + (Level * 1.5f) + (Level * Level * 0.1));
    }
    public override void IntelligenceLevelUp()
    {
        Intelligence = (int)(Intelligence + (Level * 1.5f) + (Level * Level * 0.1));
    }
    public override void StrengthLevelUp()
    {
        Strength = (int)(Strength + (Level * 1.5f) + (Level * Level * 0.01));
    }

    /// <summary>
    /// Full level-up: scales all stats, DamageMultiplier, MaxXP, HP, and increments Level.
    /// </summary>
    public override void LevelUp()
    {
        Strength = (int)(Strength + (Level * 1.5f) + (Level * Level * 0.01));
        Dexterity = (int)(Dexterity + (Level * 1.02f) + (Level * Level * 0.1));
        Intelligence = (int)(Intelligence + (Level * 1.5f) + (Level * Level * 0.1));
        Arcana = (int)(Arcana + (Level * 1.5f) + (Level * Level * 0.1));
        Endurance = (int)(Endurance + (Level * 1.5f) + (Level * Level * 0.02));
        DamageMultiplier = (int)(DamageMultiplier + (Level * 0.2f) + (Level * Level * 0.01));
        MaxExperiencePoint += 100;
        HealthPoints = (int)(HealthPoints + (Level * 1.5f) + (Level * Level * 0.1));
        Level++;
    }
}
