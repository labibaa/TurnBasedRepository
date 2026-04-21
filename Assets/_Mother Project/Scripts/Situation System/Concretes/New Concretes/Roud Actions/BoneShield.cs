using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class BoneShield: ICommand
{
    CharacterBaseClasses player;
    CharacterBaseClasses target;
    TemporaryStats playerTempStats;
    TemporaryStats targetTempStats;
    ImprovedActionStat boneShield;


    public BoneShield(CharacterBaseClasses playerAttacker, CharacterBaseClasses targetDefender, TemporaryStats currentStatPlayer, TemporaryStats currentStatTarget, ImprovedActionStat boneShieldScriptable)
    {
        player = playerAttacker;
        target = targetDefender;
        playerTempStats = currentStatPlayer;
        targetTempStats = currentStatTarget;
        boneShield = boneShieldScriptable;

    }

    public async UniTask Execute()
    { 

        if (ActionResolver.instance.ActionAccuracyCalculation(boneShield.ActionAccuracy)&& player.gameObject.activeInHierarchy)
        {
            int healPoint = -10;
            playerTempStats.CurrentHealth =  HealthManager.instance.HealthCalculation(healPoint, playerTempStats.CurrentHealth);
            await HandleAnimation();
    
        }
    }


    async UniTask HandleAnimation()
    {
        Transform closestTarget = TurnManager.instance.FindClosestTarget(TurnManager.instance.target, player.GetComponent<CharacterBaseClasses>());
        await TempManager.instance.CharacterRotation(closestTarget.GetComponent<CharacterBaseClasses>(), player, 2f);

        //CutsceneManager.instance.virtualCamera.LookAt = player.gameObject.transform;
        //CutsceneManager.instance.virtualCamera.Follow = player.gameObject.transform;
        //CutsceneManager.instance.virtualCamera.Priority = 15;

        //player.GetComponent<SpawnVFX>().SetTargetAnimator(target.gameObject);
        //player.GetComponent<SpawnVFX>().SetTargetVFXPosition(target.gameObject);
        //player.GetComponent<SpawnVFX>().SetOwnVFXPosition(player.gameObject.GetComponent<VFXSpawnPosition>().MidBody);
        player.GetComponent<SpawnVFX>().SetVFXSound(boneShield.actionSound);

        await CutsceneManager.instance.PlayAnimationForCharacter(player.gameObject, GetActionName());

        //player.GetComponent<ArrowSpawner>().SpawnArrow(player.gameObject, target.gameObject);
    }


    public string GetActionName()
    {
        return "BoneShield";
    }

    public int GetPVValue()
    {
        return boneShield.PriorityValue;
    }

    public CharacterBaseClasses GetTarget()
    {
        return player;
    }

    public int GetAPValue()
    {
        return boneShield.APCost;
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
