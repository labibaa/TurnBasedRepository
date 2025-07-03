using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class ThirdRatePerformance : ICommand
{
    //deals no damage
    //only negates kill dmg 
    CharacterBaseClasses player;
    CharacterBaseClasses target;
    TemporaryStats playerTempStats;
    TemporaryStats targetTempStats;
    ActionStat thirdRatePerformance;


    public ThirdRatePerformance(CharacterBaseClasses playerAttacker, CharacterBaseClasses targetDefender, TemporaryStats currentStatPlayer, TemporaryStats currentStatTarget, ActionStat thirdRatePerformanceScriptable)
    {
        player = playerAttacker;
        target = targetDefender;
        playerTempStats = currentStatPlayer;
        targetTempStats = currentStatTarget;
        thirdRatePerformance = thirdRatePerformanceScriptable;

    }

    public async UniTask Execute()
    {
        
        float actionAccuracy = thirdRatePerformance.ActionAccuracy;
        if (targetTempStats.IsDodgeActive)
        {
            actionAccuracy = actionAccuracy - 50;
        }
        if (targetTempStats.IsBlockActive)
        {
            thirdRatePerformance.BasePower = thirdRatePerformance.BasePower - 30;
        }
        
     /*   if (ActionResolver.instance.ActionAccuracyCalculation(thirdRatePerformance.ActionAccuracy) && !playerTempStats.IsThirdRatePerformanceActive) 
        {
            targetTempStats.IsThirdRatePerformanceActive = true;
            
        }*/
        
    }

    public string GetActionName()
    {
        return "ThirdRatePerformance";
    }

    public int GetPVValue()
    {
        return thirdRatePerformance.PriorityValue;
    }

    public CharacterBaseClasses GetTarget()
    {
        return target;
    }

    public int GetAPValue()
    {
        return thirdRatePerformance.APCost;
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
