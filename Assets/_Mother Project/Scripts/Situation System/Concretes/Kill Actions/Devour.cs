using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Devour : ICommand
{
    CharacterBaseClasses player;
    CharacterBaseClasses target;
    TemporaryStats playerTempStats;
    TemporaryStats targetTempStats;
    ActionStat devour;


    public Devour(CharacterBaseClasses playerAttacker, CharacterBaseClasses targetDefender, TemporaryStats currentStatPlayer, TemporaryStats currentStatTarget, ActionStat devourScriptable)
    {
        player = playerAttacker;
        target = targetDefender;
        playerTempStats = currentStatPlayer;
        targetTempStats = currentStatTarget;
        devour = devourScriptable;

    }





    public async UniTask Execute()
    {
        float actionAccuracy = devour.ActionAccuracy;
        if (targetTempStats.IsDodgeActive)
        {
            actionAccuracy = actionAccuracy - 50;
        }

        if (targetTempStats.IsBlockActive)
        {
            devour.BasePower = devour.BasePower - 30;
        }

/*        if (ActionResolver.instance.ActionAccuracyCalculation(actionAccuracy) && !playerTempStats.IsThirdRatePerformanceActive)
        {

            Debug.Log("Devour executed");
            float damage = ActionResolver.instance.CalculateKillDamage(player, target, devour);//counter won't work if player chooses devour
            targetTempStats.CurrentHealth = HealthManager.instance.HealthCalculation(damage, targetTempStats.CurrentHealth);
           
            if (targetTempStats.CurrentHealth < 0)
            {
                targetTempStats.CurrentHealth = 0;
                playerTempStats.CurrentHealth = HealthManager.instance.HealthCalculation(-damage / 2.0f, playerTempStats.CurrentHealth); ;
                Debug.Log("health done");
                HealthManager.instance.PlayerMortality(targetTempStats,0,playerTempStats);
            }

        }*/
        //if (ActionResolver.instance.ActionAccuracyCalculation(actionAccuracy) && !targetTempStats.IsThirdRatePerformanceActive) {
        //    bool isadjacent = GridMovement.instance.InAdjacentMatrix(player.gameObject.transform.position, target.gameObject.transform.position, devour.ActionRange);
        //    if (isadjacent) {

        //        float damage = ActionResolver.instance.CalculateKillDamage(player, target, devour)*1000;
        //        targetTempStats.CurrentHealth = HealthManager.instance.HealthCalculation(damage, targetTempStats.CurrentHealth);
        //        Debug.Log("Damage done");
        //        if (targetTempStats.CurrentHealth < 0)
        //        {
        //            targetTempStats.CurrentHealth = 0;
        //            playerTempStats.CurrentHealth = HealthManager.instance.HealthCalculation(-damage / 2.0f, playerTempStats.CurrentHealth); ;
        //            Debug.Log("health done");
        //        }




        //    }
        //}
    }

    public string GetActionName()
    {
        return "Devour";
    }

    public string GetActionType()
    {
        throw new System.NotImplementedException();
    }

    public NavMeshAgent GetAgent()
    {
        throw new System.NotImplementedException();
    }

    public int GetAPValue()
    {
        return devour.APCost;
    }

    public List<GameObject> GetPaths()
    {
        throw new System.NotImplementedException();
    }

    public int GetPVValue()
    {
        return devour.PriorityValue;
    }

    public CharacterBaseClasses GetTarget()
    {
        return target;
    }
}
