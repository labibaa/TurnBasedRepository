using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class KeenSenses : ICommand
{
    //no damage dealing
    CharacterBaseClasses player;
    CharacterBaseClasses target;
    TemporaryStats playerTempStats;
    TemporaryStats targetTempStats;
    ActionStat keenSenses;


    public KeenSenses(CharacterBaseClasses playerAttacker, CharacterBaseClasses targetDefender, TemporaryStats currentStatPlayer, TemporaryStats currentStatTarget, ActionStat keenSensesScriptable)
    {
        player = playerAttacker;
        target = targetDefender;
        playerTempStats = currentStatPlayer;
        targetTempStats = currentStatTarget;
        keenSenses = keenSensesScriptable;

    }

    public KeenSenses()
    {
    }

    public async UniTask Execute()
    {
        playerTempStats.CurrentEndurance = playerTempStats.CurrentEndurance * 2;
    }

    public string GetActionName()
    {
        return "KeenSenses";
    }

    public int GetPVValue()
    {
        return keenSenses.PriorityValue;
    }

    public CharacterBaseClasses GetTarget()
    {
        return target;
    }
    public int GetAPValue()
    {
        return keenSenses.APCost;
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
