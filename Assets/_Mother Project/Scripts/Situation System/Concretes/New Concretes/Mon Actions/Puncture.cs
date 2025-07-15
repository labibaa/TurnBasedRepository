using Cysharp.Threading.Tasks;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Puncture : ICommand
{
    CharacterBaseClasses player;
    CharacterBaseClasses target;
    TemporaryStats playerTempStats;
    TemporaryStats targetTempStats;
    ImprovedActionStat puncture;
    string ActionType;

    public Puncture(CharacterBaseClasses playerAttacker, CharacterBaseClasses targetDefender, TemporaryStats currentStatPlayer, TemporaryStats currentStatTarget, ImprovedActionStat PuntureScriptable, string actionType)
    {
        player = playerAttacker;
        target = targetDefender;
        playerTempStats = currentStatPlayer;
        targetTempStats = currentStatTarget;
        puncture = PuntureScriptable;
        ActionType = actionType;
    }

    public async UniTask Execute()
    {
        int attackOrder = checkOrder();//need to add death check later
        GameObject effectObj = new GameObject("punctureDOT");
        var handler = effectObj.AddComponent<PunctureDOTHandler>();

        GameObject vfxObj = OrbSpawner.instance.SpawnDotVFX(puncture.PlayerActionVFX, target.transform);

        handler.SetPunctureIAS(puncture);
        handler.Initialize(playerTempStats, targetTempStats, puncture.PriorityValue, attackOrder, vfxObj);
        await HandleAnimation();

    }

    async UniTask HandleAnimation()
    {
        Transform closestTarget = TurnManager.instance.FindClosestTarget(TurnManager.instance.target, player.GetComponent<CharacterBaseClasses>());
        await TempManager.instance.CharacterRotation(closestTarget.GetComponent<CharacterBaseClasses>(), player, 2f);
        
                player.GetComponent<SpawnVFX>().SetTargetAnimator(target.gameObject);
                player.GetComponent<SpawnVFX>().SetTargetVFXPosition(target.gameObject);
                player.GetComponent<SpawnVFX>().SetOwnVFXPosition(player.gameObject.GetComponent<VFXSpawnPosition>().MidBody);
                player.GetComponent<SpawnVFX>().SetVFXPrefab(puncture.PlayerActionVFX);
                player.GetComponent<SpawnVFX>().SetTargetHitVFXPrefab(puncture.TargetHitVFX);
                player.GetComponent<SpawnVFX>().SetParticle(puncture.particle);
                player.GetComponent<SpawnVFX>().SetVFXSound(puncture.actionSound);
                player.GetComponent<SpawnVFX>().SetTargetAnimation(puncture.TargetHurtAnimation);

        await CutsceneManager.instance.PlayAnimationForCharacter(player.gameObject, GetActionName());

        //player.GetComponent<ArrowSpawner>().SpawnArrow(player.gameObject, target.gameObject);
    }


    public string GetActionName()
    {
        return puncture.ActionName;
    }

    public int GetPVValue()
    {
        return puncture.PriorityValue;
    }

    public CharacterBaseClasses GetTarget()
    {
        return player;
    }

    public int GetAPValue()
    {
        return puncture.APCost;
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
        return ActionType;
    }
    int checkOrder()
    {
        return TurnManager.instance.players.IndexOf(target.GetComponent<PlayerTurn>()) - TurnManager.instance.players.IndexOf(player.GetComponent<PlayerTurn>());

    }
}
