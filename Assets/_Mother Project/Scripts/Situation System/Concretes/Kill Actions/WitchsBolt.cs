using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class WitchsBolt : ICommand
{
    CharacterBaseClasses player;
    CharacterBaseClasses target;
    TemporaryStats playerTempStats;
    TemporaryStats targetTempStats;
    ActionStat witchsBolt;

    public WitchsBolt(CharacterBaseClasses playerAttacker, CharacterBaseClasses targetDefender, TemporaryStats currentStatPlayer, TemporaryStats currentStatTarget, ActionStat witchsBoltScriptable)
    {
        player = playerAttacker;
        target = targetDefender;
        playerTempStats = currentStatPlayer;
        targetTempStats = currentStatTarget;
        witchsBolt = witchsBoltScriptable;

    }

    public async UniTask Execute()
    {
        Debug.Log("Witch's Bolt executed");
        float actionAccuracy = witchsBolt.ActionAccuracy;
        if (targetTempStats.IsDodgeActive)
        {
            actionAccuracy = actionAccuracy - 50;
        }

        if (targetTempStats.IsBlockActive)
        {
            witchsBolt.BasePower = witchsBolt.BasePower - 30;
        }
        Debug.Log("AA" + ActionResolver.instance.ActionAccuracyCalculation(actionAccuracy));
        if (ActionResolver.instance.ActionAccuracyCalculation(actionAccuracy) && !playerTempStats.IsThirdRatePerformanceActive)
        {
            float damage = ActionResolver.instance.CalculateKillDamage(player, target, witchsBolt);
            if (targetTempStats.IsCounterActive)
            {
                playerTempStats.CurrentHealth = HealthManager.instance.HealthCalculation(damage, targetTempStats.CurrentHealth);
                await HealthManager.instance.PlayerMortality(playerTempStats,0, playerTempStats);
            }
            else
            {
              
                targetTempStats.CurrentHealth = HealthManager.instance.HealthCalculation(damage, targetTempStats.CurrentHealth);
                await HealthManager.instance.PlayerMortality(targetTempStats,0, playerTempStats);
                await HandleAnimation();
               

            }
            Debug.Log("damage" + damage);
        }

        //if (ActionResolver.instance.ActionAccuracyCalculation(actionAccuracy))
        //{
        //    bool isadjacent = GridMovement.instance.InAdjacentMatrix(player.gameObject.transform.position, target.gameObject.transform.position, witchsBolt.ActionRange);
        //    if (isadjacent)
        //    {

        //        float damage = ActionResolver.instance.CalculateKillDamage(player, target, witchsBolt);
        //        targetTempStats.CurrentHealth = HealthManager.instance.HealthCalculation(damage, targetTempStats.CurrentHealth);

        //    }
        //}

        
    }
    async UniTask HandleAnimation()
    {

        Vector3 directionToTarget = target.transform.position - player.transform.position;

        // Create a rotation based on the direction and apply it to the player
        Quaternion targetRotation = Quaternion.LookRotation(directionToTarget);
        player.transform.rotation = targetRotation;
      //  player.GetComponent<PlayParticle>().target = target.gameObject;
        await CutsceneManager.instance.PlayAnimationForCharacter(player.gameObject, GetActionName());
       
        //CutsceneManager.instance.PlayAnimationForCharacter(target.gameObject, "Hurt");

    }
    public string GetActionName()
    {
        return "WitchesBolt";
    }

    public int GetPVValue()
    {
        return witchsBolt.PriorityValue;
    }

    public CharacterBaseClasses GetTarget()
    {
        return target;
    }

    public int GetAPValue()
    {
        return witchsBolt.APCost;
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
