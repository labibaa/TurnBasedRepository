using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using static Unity.Collections.AllocatorManager;

public class Imbuement : ICommand
{
    CharacterBaseClasses player;
    CharacterBaseClasses target;
    TemporaryStats playerTempStats;
    TemporaryStats targetTempStats;
    ImprovedActionStat imbuement;


    public Imbuement(CharacterBaseClasses playerAttacker, CharacterBaseClasses targetDefender, TemporaryStats currentStatPlayer, TemporaryStats currentStatTarget, ImprovedActionStat ImbuementScriptable)
    {
        player = playerAttacker;
        target = targetDefender;
        playerTempStats = currentStatPlayer;
        targetTempStats = currentStatTarget;
        imbuement = ImbuementScriptable;

    }



    public async UniTask Execute()
    {

            playerTempStats.IsBlockActive = true;

            await HandleAnimation();

        
    }


    async UniTask HandleAnimation()
    {
        Transform closestTarget = TurnManager.instance.FindClosestTarget(TurnManager.instance.target, player.GetComponent<CharacterBaseClasses>());
        await TempManager.instance.CharacterRotation(closestTarget.GetComponent<CharacterBaseClasses>(), player, 2f);
/*
        player.GetComponent<SpawnVFX>().SetTargetAnimator(target.gameObject);
        player.GetComponent<SpawnVFX>().SetTargetVFXPosition(target.gameObject);
        player.GetComponent<SpawnVFX>().SetOwnVFXPosition(player.gameObject.GetComponent<VFXSpawnPosition>().MidBody);
        player.GetComponent<SpawnVFX>().SetVFXPrefab(imbuement.PlayerActionVFX);
        player.GetComponent<SpawnVFX>().SetTargetHitVFXPrefab(imbuement.TargetHitVFX);
        player.GetComponent<SpawnVFX>().SetParticle(imbuement.particle);
        player.GetComponent<SpawnVFX>().SetVFXSound(imbuement.actionSound);
        player.GetComponent<SpawnVFX>().SetTargetAnimation(imbuement.TargetHurtAnimation);*/

        await CutsceneManager.instance.PlayAnimationForCharacter(player.gameObject, GetActionName());

        //player.GetComponent<ArrowSpawner>().SpawnArrow(player.gameObject, target.gameObject);
    }


    public string GetActionName()
    {
        return imbuement.ActionName;
    }

    public int GetPVValue()
    {
        return imbuement.PriorityValue;
    }

    public CharacterBaseClasses GetTarget()
    {
        return player;
    }

    public int GetAPValue()
    {
        return imbuement.APCost;
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
        return "Melee";
    }
    int checkOrder()
    {
        return TurnManager.instance.players.IndexOf(target.GetComponent<PlayerTurn>()) - TurnManager.instance.players.IndexOf(player.GetComponent<PlayerTurn>());

    }
}
