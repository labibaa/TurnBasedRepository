using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CharacterStats_UI : MonoBehaviour
{
    [SerializeField] GameObject StatsPanel;
    [SerializeField] TextMeshProUGUI hp_txt;
    [SerializeField] TextMeshProUGUI dex_txt;
    [SerializeField] TextMeshProUGUI end_txt;
    [SerializeField] TextMeshProUGUI str_txt;
    [SerializeField] TextMeshProUGUI int_txt;
    [SerializeField] TextMeshProUGUI arc_txt;
    [SerializeField] TextMeshProUGUI up_hp_txt;
    [SerializeField] TextMeshProUGUI up_dex_txt;
    [SerializeField] TextMeshProUGUI up_end_txt;
    [SerializeField] TextMeshProUGUI up_str_txt;
    [SerializeField] TextMeshProUGUI up_int_txt;
    [SerializeField] TextMeshProUGUI up_arc_txt;
    [SerializeField] TextMeshProUGUI characterName_txt;
    [SerializeField] TextMeshProUGUI currentXP_txt;
    [SerializeField] TextMeshProUGUI currentLvl_txt;
    [SerializeField] TextMeshProUGUI maxXP_txt;
    [SerializeField] GameObject xpNotification;
    

    public void CharacterStatsUI()
    {
        GameObject crntPlayer = SwitchMC.Instance.mainCharacter;
        int lvl = crntPlayer.GetComponent<CharacterBaseClasses>().Level;
        StatsPanel.SetActive(true);
        //characterName_txt.text = crntPlayer.GetComponent<CharacterBaseClasses>().characterName.ToString();
        currentXP_txt.text = crntPlayer.GetComponent<TemporaryStats>().CurrentExp.ToString();
        /* if(crntPlayer.GetComponent<TemporaryStats>().CurrentExp >= (10 * crntPlayer.GetComponent<CharacterBaseClasses>().Level * crntPlayer.GetComponent<CharacterBaseClasses>().Level))
         {
        (int)(HealthPoints + (Level * 1.2f) + (Level * Level * 0.1))
         }*/
        hp_txt.text = crntPlayer.GetComponent<CharacterBaseClasses>().HealthPoints.ToString();
        up_hp_txt.text = ((int)(crntPlayer.GetComponent<CharacterBaseClasses>().HealthPoints + (lvl * 1.2f) + (lvl * lvl * 0.1))).ToString();
        dex_txt.text = crntPlayer.GetComponent<CharacterBaseClasses>().Dexterity.ToString();
        end_txt.text = crntPlayer.GetComponent<CharacterBaseClasses>().Endurance.ToString();
        str_txt.text = crntPlayer.GetComponent<CharacterBaseClasses>().Strength.ToString();
        int_txt.text = crntPlayer.GetComponent<CharacterBaseClasses>().Intelligence.ToString();
        arc_txt.text = crntPlayer.GetComponent<CharacterBaseClasses>().Arcana.ToString();
        currentLvl_txt.text = crntPlayer.GetComponent<CharacterBaseClasses>().Level.ToString();
        //maxXP_txt.text = crntPlayer.GetComponent<CharacterBaseClasses>().MaxExperiencePoint.ToString();
    }
    public void HideUI()
    {
        StatsPanel.SetActive(false);
    }
    public void LevelUpCharacter()
    {
        GameObject crntPlayer = SwitchMC.Instance.mainCharacter;
        //upgrade cost 	"Upgrade Cost=10×(Current Stat Level)^2"
        if (crntPlayer.GetComponent<TemporaryStats>().CurrentExp >= (10 * crntPlayer.GetComponent<CharacterBaseClasses>().Level * crntPlayer.GetComponent<CharacterBaseClasses>().Level))
        {
            crntPlayer.GetComponent<TemporaryStats>().CurrentExp -= (10 * crntPlayer.GetComponent<CharacterBaseClasses>().Level * crntPlayer.GetComponent<CharacterBaseClasses>().Level);
            crntPlayer.GetComponent<CharacterBaseClasses>().LevelUp();
            crntPlayer.GetComponent<TemporaryStats>().SetCharacterStat();
            //CharacterStatsUI();
        }
        else
        {
            xpNotification.SetActive(true);
        }
    }

    public void LevelUpCharacterHealth()
    {
        GameObject crntPlayer = SwitchMC.Instance.mainCharacter;
        //upgrade cost 	"Upgrade Cost=10×(Current Stat Level)^2"
        if (crntPlayer.GetComponent<TemporaryStats>().CurrentExp >= (10 * crntPlayer.GetComponent<CharacterBaseClasses>().Level * crntPlayer.GetComponent<CharacterBaseClasses>().Level))
        {
            crntPlayer.GetComponent<TemporaryStats>().CurrentExp -= (10 * crntPlayer.GetComponent<CharacterBaseClasses>().Level * crntPlayer.GetComponent<CharacterBaseClasses>().Level);
            crntPlayer.GetComponent<CharacterBaseClasses>().HealthLevelUp();
            crntPlayer.GetComponent<TemporaryStats>().SetCharacterStat();
            //CharacterStatsUI();
        }
        else
        {
            xpNotification.SetActive(true);
        }
    }
    public void LevelUpCharacterDexterity()
    {
        GameObject crntPlayer = SwitchMC.Instance.mainCharacter;
        //upgrade cost 	"Upgrade Cost=10×(Current Stat Level)^2"
        if (crntPlayer.GetComponent<TemporaryStats>().CurrentExp >= (10 * crntPlayer.GetComponent<CharacterBaseClasses>().Level * crntPlayer.GetComponent<CharacterBaseClasses>().Level))
        {
            crntPlayer.GetComponent<TemporaryStats>().CurrentExp -= (10 * crntPlayer.GetComponent<CharacterBaseClasses>().Level * crntPlayer.GetComponent<CharacterBaseClasses>().Level);
            crntPlayer.GetComponent<CharacterBaseClasses>().DexterityLevelUp();
            crntPlayer.GetComponent<TemporaryStats>().SetCharacterStat();
            //CharacterStatsUI();
        }
        else
        {
            xpNotification.SetActive(true);
        }
    }
    public void LevelUpCharacterArcana()
    {
        GameObject crntPlayer = SwitchMC.Instance.mainCharacter;
        //upgrade cost 	"Upgrade Cost=10×(Current Stat Level)^2"
        if (crntPlayer.GetComponent<TemporaryStats>().CurrentExp >= (10 * crntPlayer.GetComponent<CharacterBaseClasses>().Level * crntPlayer.GetComponent<CharacterBaseClasses>().Level))
        {
            crntPlayer.GetComponent<TemporaryStats>().CurrentExp -= (10 * crntPlayer.GetComponent<CharacterBaseClasses>().Level * crntPlayer.GetComponent<CharacterBaseClasses>().Level);
            crntPlayer.GetComponent<CharacterBaseClasses>().ArcanaLevelUp();
            crntPlayer.GetComponent<TemporaryStats>().SetCharacterStat();
            //CharacterStatsUI();
        }
        else
        {
            xpNotification.SetActive(true);
        }
    }
    public void LevelUpCharacterEndurence()
    {
        GameObject crntPlayer = SwitchMC.Instance.mainCharacter;
        //upgrade cost 	"Upgrade Cost=10×(Current Stat Level)^2"
        if (crntPlayer.GetComponent<TemporaryStats>().CurrentExp >= (10 * crntPlayer.GetComponent<CharacterBaseClasses>().Level * crntPlayer.GetComponent<CharacterBaseClasses>().Level))
        {
            crntPlayer.GetComponent<TemporaryStats>().CurrentExp -= (10 * crntPlayer.GetComponent<CharacterBaseClasses>().Level * crntPlayer.GetComponent<CharacterBaseClasses>().Level);
            crntPlayer.GetComponent<CharacterBaseClasses>().EnduranceLevelUp();
            crntPlayer.GetComponent<TemporaryStats>().SetCharacterStat();
            //CharacterStatsUI();
        }
        else
        {
            xpNotification.SetActive(true);
        }
    }
    public void LevelUpCharacterStrength()
    {
        GameObject crntPlayer = SwitchMC.Instance.mainCharacter;
        //upgrade cost 	"Upgrade Cost=10×(Current Stat Level)^2"
        if (crntPlayer.GetComponent<TemporaryStats>().CurrentExp >= (10 * crntPlayer.GetComponent<CharacterBaseClasses>().Level * crntPlayer.GetComponent<CharacterBaseClasses>().Level))
        {
            crntPlayer.GetComponent<TemporaryStats>().CurrentExp -= (10 * crntPlayer.GetComponent<CharacterBaseClasses>().Level * crntPlayer.GetComponent<CharacterBaseClasses>().Level);
            crntPlayer.GetComponent<CharacterBaseClasses>().StregthLevelUp();
            crntPlayer.GetComponent<TemporaryStats>().SetCharacterStat();
            //CharacterStatsUI();
        }
        else
        {
            xpNotification.SetActive(true);
        }
    }
    public void LevelUpCharacterIntelligence()
    {
        GameObject crntPlayer = SwitchMC.Instance.mainCharacter;
        //upgrade cost 	"Upgrade Cost=10×(Current Stat Level)^2"
        if (crntPlayer.GetComponent<TemporaryStats>().CurrentExp >= (10 * crntPlayer.GetComponent<CharacterBaseClasses>().Level * crntPlayer.GetComponent<CharacterBaseClasses>().Level))
        {
            crntPlayer.GetComponent<TemporaryStats>().CurrentExp -= (10 * crntPlayer.GetComponent<CharacterBaseClasses>().Level * crntPlayer.GetComponent<CharacterBaseClasses>().Level);
            crntPlayer.GetComponent<CharacterBaseClasses>().IntelligenceLevelUp();
            crntPlayer.GetComponent<TemporaryStats>().SetCharacterStat();
            //CharacterStatsUI();
        }
        else
        {
            xpNotification.SetActive(true);
        }
    }

}
