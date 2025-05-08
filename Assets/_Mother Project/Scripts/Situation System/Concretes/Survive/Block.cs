using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Block : ICommand
{

    CharacterBaseClasses player;
    CharacterBaseClasses target;
    TemporaryStats playerTempStats;
    TemporaryStats targetTempStats;
    ImprovedActionStat block;

    public Block(CharacterBaseClasses playerAttacker, CharacterBaseClasses targetDefender, TemporaryStats currentStatPlayer, TemporaryStats currentStatTarget, ImprovedActionStat blockScriptable)
    {
        player = playerAttacker;
        target = targetDefender;
        playerTempStats = currentStatPlayer;
        targetTempStats = currentStatTarget;
        block = blockScriptable;

    }

    public async UniTask Execute()
    {
        if (ActionResolver.instance.ActionAccuracyCalculation(block.ActionAccuracy) && player.gameObject.activeInHierarchy) {
            playerTempStats.IsBlockActive = true;
            
            await HandleAnimation();
           
        }
        
    }


    async UniTask HandleAnimation()
    {
        Transform closestTarget = TurnManager.instance.FindClosestTarget(TurnManager.instance.target, player.GetComponent<CharacterBaseClasses>());
        TempManager.instance.CharacterRotation(closestTarget.GetComponent<CharacterBaseClasses>(), player, 2f);

       /* player.GetComponent<PlayParticle>().actionSound = block.actionSound;   
        player.GetComponent<PlayParticle>().particlePrefab = block.ParticleSystem;
        player.GetComponent<PlayParticle>().particlePrefabHit = block.HitParticleSystem;*/
        //CutsceneManager.instance.virtualCamera.LookAt = player.gameObject.transform;
        //CutsceneManager.instance.virtualCamera.Follow = player.gameObject.transform;

        
        player.GetComponent<SpawnVFX>().SetOwnVFXPosition(player.GetComponent<VFXSpawnPosition>().CharacterBodyPosition[block.CharacterBodyLocation]);
       
        player.GetComponent<SpawnVFX>().SetVFXPrefab(block.PlayerActionVFX);
       
        player.GetComponent<SpawnVFX>().SetParticle(block.particle);
        player.GetComponent<SpawnVFX>().SetVFXSound(block.actionSound);



        //CutsceneManager.instance.virtualCamera.Priority = 15;
        await CutsceneManager.instance.PlayAnimationForCharacter(player.gameObject, GetActionName());      
    }


    public string GetActionName()
    {
        return "Block";
    }

    public int GetPVValue()
    {
        return block.PriorityValue;
    }

    public CharacterBaseClasses GetTarget()
    {
        return player;
    }
    public int GetAPValue()
    {
        return block.APCost;
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
