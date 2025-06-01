using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class DaggerSweep : ICommand
{
    TemporaryStats playerTempStats;
    ImprovedActionStat daggerSweepScriptable;

    public DaggerSweep( ImprovedActionStat daggerSweep, TemporaryStats PlayerTempStats)
    {
        daggerSweepScriptable = daggerSweep;
        playerTempStats = PlayerTempStats;
    }


    public async UniTask Execute()
    {
        List<CharacterBaseClasses> targetsInRange = GridMovement.instance.InAdjacentMatrix(playerTempStats.currentPlayerGridPosition, playerTempStats.CharacterTeam,daggerSweepScriptable.ActionRange, Color.clear);
        GridMovement.instance.ResetHighlightedPath();
        ImprovedActionStat pushBackScriptable = DAOScriptableObject.instance.GetImprovedActionData(StringData.directory, "PushBack");
        foreach (CharacterBaseClasses target in targetsInRange)
        {
            target.GetComponent<TemporaryStats>().EnemyTargetSelectionParticle.SetActive(false);
            TurnManager.instance.ResetTargetHIghlightVisual();
            ICommand pushBack = new PushBack(playerTempStats.GetComponent<CharacterBaseClasses>(), target.GetComponent<CharacterBaseClasses>(), playerTempStats, target.GetComponent<TemporaryStats>(), pushBackScriptable, "SingleMelee");
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
        return playerTempStats.GetComponent<CharacterBaseClasses>();
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
