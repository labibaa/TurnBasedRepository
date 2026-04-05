using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Performer character class specialization (entertainment/support-focused).
/// WARNING: Level-up methods are not yet implemented.
/// All methods log a warning to the console to flag missing logic.
/// TODO: Implement stat growth formulas.
/// </summary>
public class PerformerClass : CharacterBaseClasses
{
    public override void LevelUp()
    {
        Debug.LogWarning($"{characterName}: PerformerClass.LevelUp() not implemented.");
    }
    public override void ArcanaLevelUp()
    {
        Debug.LogWarning($"{characterName}: PerformerClass.ArcanaLevelUp() not implemented.");
    }
    public override void DexterityLevelUp()
    {
        Debug.LogWarning($"{characterName}: PerformerClass.DexterityLevelUp() not implemented.");
    }
    public override void EnduranceLevelUp()
    {
        Debug.LogWarning($"{characterName}: PerformerClass.EnduranceLevelUp() not implemented.");
    }
    public override void HealthLevelUp()
    {
        Debug.LogWarning($"{characterName}: PerformerClass.HealthLevelUp() not implemented.");
    }
    public override void IntelligenceLevelUp()
    {
        Debug.LogWarning($"{characterName}: PerformerClass.IntelligenceLevelUp() not implemented.");
    }
    public override void StrengthLevelUp()
    {
        Debug.LogWarning($"{characterName}: PerformerClass.StrengthLevelUp() not implemented.");
    }
}
