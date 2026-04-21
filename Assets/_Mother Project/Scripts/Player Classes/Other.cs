using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Generic/fallback character class for NPCs or unclassified characters.
/// WARNING: Level-up methods are not yet implemented.
/// All methods log a warning to the console to flag missing logic.
/// TODO: Implement stat growth formulas or remove if unused.
/// </summary>
public class Other : CharacterBaseClasses
{
    public override void LevelUp()
    {
        Debug.LogWarning($"{characterName}: Other.LevelUp() not implemented.");
    }
    public override void ArcanaLevelUp()
    {
        Debug.LogWarning($"{characterName}: Other.ArcanaLevelUp() not implemented.");
    }
    public override void DexterityLevelUp()
    {
        Debug.LogWarning($"{characterName}: Other.DexterityLevelUp() not implemented.");
    }
    public override void EnduranceLevelUp()
    {
        Debug.LogWarning($"{characterName}: Other.EnduranceLevelUp() not implemented.");
    }
    public override void HealthLevelUp()
    {
        Debug.LogWarning($"{characterName}: Other.HealthLevelUp() not implemented.");
    }
    public override void IntelligenceLevelUp()
    {
        Debug.LogWarning($"{characterName}: Other.IntelligenceLevelUp() not implemented.");
    }
    public override void StrengthLevelUp()
    {
        Debug.LogWarning($"{characterName}: Other.StrengthLevelUp() not implemented.");
    }
}
