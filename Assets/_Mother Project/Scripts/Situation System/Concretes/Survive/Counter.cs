using Cysharp.Threading.Tasks;
using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.AI;

public class Counter : ICommand
{
    // Negates all damages

    CharacterBaseClasses player;
    CharacterBaseClasses target;
    TemporaryStats playerTempStats;
    TemporaryStats targetTempStats;
    ImprovedActionStat counter;


    public Counter(CharacterBaseClasses playerAttacker, CharacterBaseClasses targetDefender, TemporaryStats currentStatPlayer, TemporaryStats currentStatTarget, ImprovedActionStat counterScriptable)
    {
        player = playerAttacker;
        target = targetDefender;
        playerTempStats = currentStatPlayer;
        targetTempStats = currentStatTarget;
        counter = counterScriptable;

    }
    public async UniTask Execute()
    {
        if (ActionResolver.instance.ActionAccuracyCalculation(counter.ActionAccuracy))
        {
            Debug.Log("Counter");
           playerTempStats.IsCounterActive = true; //dodge opponents action

            //hit opponent with an attack
            await HandleAnimation();  
        
        }


    }
    async UniTask HandleAnimation()
    {

        Transform closestTarget = TurnManager.instance.FindClosestTarget(TurnManager.instance.target, player.GetComponent<CharacterBaseClasses>());
        TempManager.instance.CharacterRotation(closestTarget.GetComponent<CharacterBaseClasses>(), player, 2f);

        
        //CutsceneManager.instance.virtualCamera.LookAt = player.gameObject.transform;
        //CutsceneManager.instance.virtualCamera.Follow = player.gameObject.transform;
        //CutsceneManager.instance.virtualCamera.Priority = 15;



        player.GetComponent<SpawnVFX>().SetOwnVFXPosition(player.GetComponent<VFXSpawnPosition>().CharacterBodyPosition[counter.CharacterBodyLocation]);
        player.GetComponent<SpawnVFX>().SetVFXPrefab(counter.PlayerActionVFX);
        player.GetComponent<SpawnVFX>().SetTargetHitVFXPrefab(counter.TargetHitVFX);
        player.GetComponent<SpawnVFX>().SetParticle(counter.particle);
        player.GetComponent<SpawnVFX>().SetVFXSound(counter.actionSound);
        player.GetComponent<SpawnVFX>().SetTargetAnimation(counter.TargetHurtAnimation);

        await CutsceneManager.instance.PlayAnimationForCharacter(player.gameObject, GetActionName());
    }



    public string GetActionName()
    {
        return "Counter";
    }

    public int GetPVValue()
    {
        return counter.PriorityValue;
    }

    public CharacterBaseClasses GetTarget()
    {
        return player;
    }

    public int GetAPValue()
    {
        return counter.APCost;
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
        return "Ranged";
    }
}
