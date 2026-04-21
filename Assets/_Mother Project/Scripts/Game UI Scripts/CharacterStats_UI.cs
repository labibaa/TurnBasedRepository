using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// UI controller for the character stats panel.
/// Displays current stats and XP for the active main character,
/// and handles individual stat level-up requests via button callbacks.
///
/// Dependencies:
///   - SwitchMC (singleton) — provides the current main character GameObject.
///   - CharacterBaseClasses — base stat data and level-up methods on the character.
///   - TemporaryStats — runtime stat mirror (CurrentExp, SetCharacterStat).
///
/// All public LevelUpCharacter*() methods are safe to bind directly to Unity UI buttons.
/// </summary>
public class CharacterStats_UI : MonoBehaviour
{
    [Header("Stats Panel")]
    [SerializeField] GameObject StatsPanel;

    [Header("Current Stat Text Fields")]
    [SerializeField] TextMeshProUGUI hp_txt;
    [SerializeField] TextMeshProUGUI dex_txt;
    [SerializeField] TextMeshProUGUI end_txt;
    [SerializeField] TextMeshProUGUI str_txt;
    [SerializeField] TextMeshProUGUI int_txt;
    [SerializeField] TextMeshProUGUI arc_txt;

    [Header("Projected Stat Text Fields (after level-up)")]
    [SerializeField] TextMeshProUGUI up_hp_txt;
    [SerializeField] TextMeshProUGUI up_dex_txt;
    [SerializeField] TextMeshProUGUI up_end_txt;
    [SerializeField] TextMeshProUGUI up_str_txt;
    [SerializeField] TextMeshProUGUI up_int_txt;
    [SerializeField] TextMeshProUGUI up_arc_txt;

    [Header("Meta Info")]
    [SerializeField] TextMeshProUGUI characterName_txt;
    [SerializeField] TextMeshProUGUI currentXP_txt;
    [SerializeField] TextMeshProUGUI currentLvl_txt;
    [SerializeField] TextMeshProUGUI maxXP_txt;

    [Header("Notifications")]
    [SerializeField] GameObject xpNotification;

    /// <summary>
    /// Populates the stats panel with the current main character's data.
    /// Call this when opening the stats screen or after a level-up to refresh values.
    /// </summary>
    public void CharacterStatsUI()
    {
        GameObject crntPlayer = SwitchMC.Instance.mainCharacter;
        var baseStats = crntPlayer.GetComponent<CharacterBaseClasses>();
        var tempStats = crntPlayer.GetComponent<TemporaryStats>();
        int lvl = baseStats.Level;
        StatsPanel.SetActive(true);
        //characterName_txt.text = baseStats.characterName.ToString();
        currentXP_txt.text = tempStats.CurrentExp.ToString();
        hp_txt.text = baseStats.HealthPoints.ToString();
        up_hp_txt.text = baseStats.GetProjectedHealthPoints().ToString();
        dex_txt.text = baseStats.Dexterity.ToString();
        end_txt.text = baseStats.Endurance.ToString();
        str_txt.text = baseStats.Strength.ToString();
        int_txt.text = baseStats.Intelligence.ToString();
        arc_txt.text = baseStats.Arcana.ToString();
        currentLvl_txt.text = baseStats.Level.ToString();
        //maxXP_txt.text = baseStats.MaxExperiencePoint.ToString();
    }

    /// <summary>Hides the stats panel.</summary>
    public void HideUI()
    {
        StatsPanel.SetActive(false);
    }

    /// <summary>
    /// Shared level-up logic: checks XP against upgrade cost, deducts XP,
    /// invokes the specific level-up action, and syncs TemporaryStats.
    /// Shows xpNotification if the player cannot afford the upgrade.
    /// </summary>
    /// <param name="levelUpAction">The specific stat level-up method to invoke on CharacterBaseClasses.</param>
    private void TryLevelUpStat(Action<CharacterBaseClasses> levelUpAction)
    {
        GameObject crntPlayer = SwitchMC.Instance.mainCharacter;
        var baseStats = crntPlayer.GetComponent<CharacterBaseClasses>();
        var tempStats = crntPlayer.GetComponent<TemporaryStats>();
        int upgradeCost = baseStats.GetUpgradeCost();
        if (tempStats.CurrentExp >= upgradeCost)
        {
            tempStats.CurrentExp -= upgradeCost;
            levelUpAction(baseStats);
            tempStats.SetCharacterStat();
        }
        else
        {
            xpNotification.SetActive(true);
        }
    }

    // --- Button callbacks (bind these in the Unity Inspector) ---
    public void LevelUpCharacter()          => TryLevelUpStat(s => s.LevelUp());
    public void LevelUpCharacterHealth()    => TryLevelUpStat(s => s.HealthLevelUp());
    public void LevelUpCharacterDexterity() => TryLevelUpStat(s => s.DexterityLevelUp());
    public void LevelUpCharacterArcana()    => TryLevelUpStat(s => s.ArcanaLevelUp());
    public void LevelUpCharacterEndurence() => TryLevelUpStat(s => s.EnduranceLevelUp());
    public void LevelUpCharacterStrength()  => TryLevelUpStat(s => s.StrengthLevelUp());
    public void LevelUpCharacterIntelligence() => TryLevelUpStat(s => s.IntelligenceLevelUp());
}
