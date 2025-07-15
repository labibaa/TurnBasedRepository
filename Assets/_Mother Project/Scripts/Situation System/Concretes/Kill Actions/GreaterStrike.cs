using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class GreaterStrike : ICommand
{

    CharacterBaseClasses player;
    CharacterBaseClasses target;
    TemporaryStats playerTempStats;
    TemporaryStats targetTempStats;
    ActionStat greaterStrike;






    public GreaterStrike(CharacterBaseClasses playerAttacker, CharacterBaseClasses targetDefender, TemporaryStats currentStatPlayer, TemporaryStats currentStatTarget, ActionStat greaterStrikeScriptable)
    {
        player = playerAttacker;
        target = targetDefender;
        playerTempStats = currentStatPlayer;
        targetTempStats = currentStatTarget;
        greaterStrike = greaterStrikeScriptable;

    }




    public async UniTask Execute()
    {

        

        float actionAccuracy = greaterStrike.ActionAccuracy;
        if (targetTempStats.IsDodgeActive)
        {
            actionAccuracy = actionAccuracy - 50;
        }
        if (targetTempStats.IsBlockActive)
        {
            greaterStrike.BasePower = greaterStrike.BasePower - 30;
        }
        
/*        if (ActionResolver.instance.ActionAccuracyCalculation(actionAccuracy) && !playerTempStats.IsThirdRatePerformanceActive) {
      
            float damage = ActionResolver.instance.CalculateKillDamage(player, target, greaterStrike);
            if (targetTempStats.IsCounterActive)
            {
                playerTempStats.CurrentHealth = HealthManager.instance.HealthCalculation(damage, targetTempStats.CurrentHealth);
                HealthManager.instance.PlayerMortality(playerTempStats, 0, playerTempStats);
            }
            else
            {
                targetTempStats.CurrentHealth = HealthManager.instance.HealthCalculation(damage, targetTempStats.CurrentHealth);
                HealthManager.instance.PlayerMortality(targetTempStats, 0, playerTempStats);
            }
          
            
        }*//*        if (ActionResolver.instance.ActionAccuracyCalculation(actionAccuracy) && !playerTempStats.IsThirdRatePerformanceActive) {
      
            float damage = ActionResolver.instance.CalculateKillDamage(player, target, greaterStrike);
            if (targetTempStats.IsCounterActive)
            {
                playerTempStats.CurrentHealth = HealthManager.instance.HealthCalculation(damage, targetTempStats.CurrentHealth);
                HealthManager.instance.PlayerMortality(playerTempStats, 0, playerTempStats);
            }
            else
            {
                targetTempStats.CurrentHealth = HealthManager.instance.HealthCalculation(damage, targetTempStats.CurrentHealth);
                HealthManager.instance.PlayerMortality(targetTempStats, 0, playerTempStats);
            }
          
            
        }*/

    }

    public string GetActionName()
    {
        return "GreaterStrike";
    }

    public int GetPVValue()
    {
        return greaterStrike.PriorityValue;
    }

    public CharacterBaseClasses GetTarget()
    {
        return target;//:)
    }
    public int GetAPValue()
    {
        return greaterStrike.APCost;
    }

    public NavMeshAgent GetAgent()
    {
        throw new System.NotImplementedException();
    }

    public List<GameObject> GetPaths()
    {
        throw new System.NotImplementedException();
    }

    public string GetActionType()
    {
        throw new System.NotImplementedException();
    }
}
