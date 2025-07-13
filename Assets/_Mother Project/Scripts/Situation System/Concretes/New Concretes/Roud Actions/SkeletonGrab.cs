using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Cysharp.Threading.Tasks;

public class SkeletonGrab : ICommand
{
    CharacterBaseClasses player;
    CharacterBaseClasses target;
    TemporaryStats playerTempStats;
    TemporaryStats targetTempStats;
    ImprovedActionStat skeletonGrab;

    public SkeletonGrab(CharacterBaseClasses playerAttacker, CharacterBaseClasses targetDefender, TemporaryStats currentStatPlayer, TemporaryStats currentStatTarget, ImprovedActionStat skeletonGrabScriptable)
    {

        player = playerAttacker;
        target = targetDefender;
        playerTempStats = currentStatPlayer;
        targetTempStats = currentStatTarget;
        skeletonGrab = skeletonGrabScriptable;
    }
    public async UniTask Execute()
    {
        int attackOrder = checkOrder();
        Debug.Log("SkeletonGrabExecuted");
        /*  EffectorSkeletonjGrab.Instance.grabbedTarget = targetTempStats;
          EffectorSkeletonjGrab.Instance.HasEffect = true;
          EffectorSkeletonjGrab.Instance.EffectOwner = playerTempStats;
          EffectorSkeletonjGrab.Instance.SkeletonGrab_IAS = skeletonGrab;
          EffectorSkeletonjGrab.Instance.TurnCount = skeletonGrab.PriorityValue;
          EffectorSkeletonjGrab.Instance.SkeletonObject = OrbSpawner.instance.SpawnSmoke(target.transform); */
        GameObject effectObj = new GameObject("SkeletonGrabDOT");
        var handler = effectObj.AddComponent<SkeletonGrabDOTHandler>();

        GameObject vfxObj = OrbSpawner.instance.SpawnDotVFX(skeletonGrab.PlayerActionVFX , target.transform);

        handler.SetSkeletonGrabIAS(skeletonGrab);
        handler.Initialize(playerTempStats, targetTempStats, skeletonGrab.PriorityValue, attackOrder, vfxObj);


        await HandleAnimation();

    }
    public async UniTask HandleAnimation()
    {
        await TempManager.instance.CharacterRotation(target, player, 2f);

        player.GetComponent<SpawnVFX>().SetTargetAnimator(target.gameObject);
        player.GetComponent<SpawnVFX>().SetOwnVFXPosition(player.GetComponent<VFXSpawnPosition>().CharacterBodyPosition[skeletonGrab.CharacterBodyLocation]);
        player.GetComponent<SpawnVFX>().SetTargetVFXPosition(target.GetComponent<VFXSpawnPosition>().CharacterBodyPosition[skeletonGrab.TargetCharacterBodyLocation]);
        player.GetComponent<SpawnVFX>().SetVFXPrefab(skeletonGrab.PlayerActionVFX);
        player.GetComponent<SpawnVFX>().SetTargetHitVFXPrefab(skeletonGrab.TargetHitVFX);
        player.GetComponent<SpawnVFX>().SetParticle(skeletonGrab.particle);
        player.GetComponent<SpawnVFX>().SetVFXSound(skeletonGrab.actionSound);
        player.GetComponent<SpawnVFX>().SetTargetAnimation(skeletonGrab.TargetHurtAnimation);

        await CutsceneManager.instance.PlayAnimationForCharacter(player.gameObject, GetActionName());
    }
    public string GetActionName()
    {
        return skeletonGrab.ActionName;
    }

    public int GetPVValue()
    {
        return skeletonGrab.PriorityValue;
    }

    public CharacterBaseClasses GetTarget()
    {
        return target;
    }

    public int GetAPValue()
    {
        return skeletonGrab.APCost;
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
    int checkOrder()
    {
        return TurnManager.instance.players.IndexOf(target.GetComponent<PlayerTurn>()) - TurnManager.instance.players.IndexOf(player.GetComponent<PlayerTurn>());

    }
}
