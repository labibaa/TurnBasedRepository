using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActionResolver : MonoBehaviour
{
    public static ActionResolver instance;

    [SerializeField]
    int defaultAP;
    [SerializeField]
    int maxAP;

    private void Awake()
    {
        if(instance == null)
        {
            instance = this;
        }
    }

    public float CalculateKillDamage(CharacterBaseClasses attacker,CharacterBaseClasses target,ActionStat actionStat) //this is the damage calculation code
    {
        float attackerOffenceModifier;
        attackerOffenceModifier = DictionaryManager.instance.GiveAttackOffenceModifier(actionStat.offenseModifier,attacker);
        float targetDefenseModifier = DictionaryManager.instance.GiveTargetDefenceModifier(actionStat.defenseModifier,target);

        int critModifier =  CriticalChance(actionStat);

        // healthDamage = (((attacker.BaseDamge + bonusDamage) * (attacker.AttackSelection[attack] * 0.04f)) / (Mathf.Pow(2, (defender.AttackSelection[defense] / attacker.AttackSelection[attack]))) * critModifier * weaknessModifier);
        float healthDamage = (((actionStat.BasePower + 0) * (attackerOffenceModifier * 0.04f)) / (Mathf.Pow(2, targetDefenseModifier / attackerOffenceModifier))) * critModifier * Random.Range(0.8f, 1.2f);

        return healthDamage;
    }
    




    public float CalculateDealDamage(CharacterBaseClasses attacker,CharacterBaseClasses target, ActionStat actionStat)
    {
       
        float attackerOffenceModifier = DictionaryManager.instance.GiveAttackOffenceModifier(actionStat.offenseModifier, attacker);
        float targetDefenseModifier = DictionaryManager.instance.GiveTargetDefenceModifier(actionStat.defenseModifier, target);

        int critModifier = CriticalChance(actionStat);
        //resolveDamage = (((attacker.BaseDamge + bonusDamage) * (attacker.AttackSelection[attack] )) / (Mathf.Pow(3, (defender.AttackSelection[defense] / attacker.AttackSelection[attack]))) * critModifier * moodModifier);
        float resolveDamage = (((actionStat.BasePower + 0) * (attackerOffenceModifier * 0.04f)) / (Mathf.Pow(3, targetDefenseModifier / attackerOffenceModifier))) * critModifier * Random.Range(0.8f, 1.2f);

        return resolveDamage;

    }

    public void CalculateNewKillDamage()
    {

    }




    public int CriticalChance( ActionStat critPercentage ) //if critSuccess is true 2 or 0
    {
        int critModifier = 0;
       
        float checkCrit = Random.Range(1, 100);

        if (checkCrit <= critPercentage.CritChance + critPercentage.condition)
        {
            critModifier = 2;
        }
        else
        {
            critModifier = 1;
        }
        return critModifier;
    }



    public int APResolver(int APPoint,int APCost)
    {
        

        return APPoint-APCost;
    }

    public int APCarryOver(int currentAP,int apToIncrease) => (currentAP + apToIncrease) > maxAP ? maxAP : (currentAP + apToIncrease);

    public bool ActionAccuracyCalculation(float accuracyPercentage)
    {

        float generateNumber = Random.Range(1,100);
     
        return generateNumber<=accuracyPercentage;
    }




    public int CalculateNewDamage(int diceValue,ImprovedActionStat actionScriptable)
    {
        
        foreach(var rangeMapping in actionScriptable.RangeMappings) //action damage range from dice value
        {
            if (diceValue>= rangeMapping.MinRangeValue && diceValue<=rangeMapping.MaxRangeValue)
            {
                return rangeMapping.MappedValue;
            }
        }
        return 0;
    }




}
