using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Fighter character class specialization.
/// WARNING: Level-up methods are not yet implemented.
/// All methods log a warning to the console to flag missing logic.
/// TODO: Implement stat growth formulas (high STR/END, moderate HP/DEX, low INT/ARC).
/// </summary>
public class FighterClass : CharacterBaseClasses
{
    public override void LevelUp()
    {
        Debug.LogWarning($"{characterName}: FighterClass.LevelUp() not implemented.");
    }
    public override void ArcanaLevelUp()
    {
        Debug.LogWarning($"{characterName}: FighterClass.ArcanaLevelUp() not implemented.");
    }
    public override void DexterityLevelUp()
    {
        Debug.LogWarning($"{characterName}: FighterClass.DexterityLevelUp() not implemented.");
    }
    public override void EnduranceLevelUp()
    {
        Debug.LogWarning($"{characterName}: FighterClass.EnduranceLevelUp() not implemented.");
    }
    public override void HealthLevelUp()
    {
        Debug.LogWarning($"{characterName}: FighterClass.HealthLevelUp() not implemented.");
    }
    public override void IntelligenceLevelUp()
    {
        Debug.LogWarning($"{characterName}: FighterClass.IntelligenceLevelUp() not implemented.");
    }
    public override void StrengthLevelUp()
    {
        Debug.LogWarning($"{characterName}: FighterClass.StrengthLevelUp() not implemented.");
    }
}
