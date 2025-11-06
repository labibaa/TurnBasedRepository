using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class DaggerSweep : ICommand
{
    TemporaryStats playerTempStats;
    CharacterBaseClasses playerBaseClass;
    ImprovedActionStat daggerSweepScriptable;
    CharacterBaseClasses closestTarget;

    public DaggerSweep( ImprovedActionStat daggerSweep, TemporaryStats PlayerTempStats, CharacterBaseClasses PlayerBaseClass, CharacterBaseClasses ClosestTarget)
    {
        daggerSweepScriptable = daggerSweep;
        playerTempStats = PlayerTempStats;
        playerBaseClass = PlayerBaseClass;
        closestTarget = ClosestTarget;
    }


    public async UniTask Execute()
    {
        List<CharacterBaseClasses> targetsInRange = GridMovement.instance.InAdjacentMatrix(playerTempStats.currentPlayerGridPosition, playerTempStats.CharacterTeam,daggerSweepScriptable.ActionRange, Color.clear);
        GridMovement.instance.ResetHighlightedPath();

        foreach (CharacterBaseClasses target in targetsInRange)
        {
            TurnManager.instance.ResetTargetHIghlightVisual();
            ICommand pushBack = new PushBack(playerTempStats.GetComponent<CharacterBaseClasses>(), target.GetComponent<CharacterBaseClasses>(), playerTempStats, target.GetComponent<TemporaryStats>(), daggerSweepScriptable, "SingleMelee");
            await pushBack.Execute();
        }

    }

    public string GetActionName()
    {
        return daggerSweepScriptable.ActionName;
    }

    public int GetPVValue()
    {
        return 0;
    }

    public CharacterBaseClasses GetTarget()
    {
        return closestTarget;
    }
    public int GetAPValue()
    {
        return daggerSweepScriptable.APCost;
    }

    public NavMeshAgent GetAgent()
    {
        return null;
    }
    public List<GameObject> GetPaths()
    {
        return null;
    }
    public string GetActionType()
    {
        return "Ranged";
    }
}
