using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Talker character class specialization (dialogue/social-focused).
/// WARNING: Level-up methods are not yet implemented.
/// All methods log a warning to the console to flag missing logic.
/// TODO: Implement stat growth formulas (high INT, moderate ARC/DEX, low STR/END).
/// </summary>
public class TalkerClass : CharacterBaseClasses
{
    public override void LevelUp()
    {
        Debug.LogWarning($"{characterName}: TalkerClass.LevelUp() not implemented.");
    }
    public override void ArcanaLevelUp()
    {
        Debug.LogWarning($"{characterName}: TalkerClass.ArcanaLevelUp() not implemented.");
    }
    public override void DexterityLevelUp()
    {
        Debug.LogWarning($"{characterName}: TalkerClass.DexterityLevelUp() not implemented.");
    }
    public override void EnduranceLevelUp()
    {
        Debug.LogWarning($"{characterName}: TalkerClass.EnduranceLevelUp() not implemented.");
    }
    public override void HealthLevelUp()
    {
        Debug.LogWarning($"{characterName}: TalkerClass.HealthLevelUp() not implemented.");
    }
    public override void IntelligenceLevelUp()
    {
        Debug.LogWarning($"{characterName}: TalkerClass.IntelligenceLevelUp() not implemented.");
    }
    public override void StrengthLevelUp()
    {
        Debug.LogWarning($"{characterName}: TalkerClass.StrengthLevelUp() not implemented.");
    }
}
