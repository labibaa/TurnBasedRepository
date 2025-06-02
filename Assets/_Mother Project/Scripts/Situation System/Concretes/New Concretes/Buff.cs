using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UIElements;

public class Buff : ICommand
{
    CharacterBaseClasses player;
    CharacterBaseClasses target;
    TemporaryStats playerTempStats;
    TemporaryStats targetTempStats;
    ImprovedActionStat buffAttack;


    public Buff(CharacterBaseClasses playerAttacker, CharacterBaseClasses targetDefender, TemporaryStats currentStatPlayer, TemporaryStats currentStatTarget, ImprovedActionStat BuffScriptable)
    {
        player = playerAttacker;
        target = targetDefender;
        playerTempStats = currentStatPlayer;
        targetTempStats = currentStatTarget;
        buffAttack = BuffScriptable;

    }



    public async UniTask Execute()
    {

        int attackOrder = checkOrder();

        float actionAccuracy = buffAttack.ActionAccuracy;

        if (ActionResolver.instance.ActionAccuracyCalculation(actionAccuracy))
        {
            await HandleAnimation();
            targetTempStats.CurrentDamageMultiplier = targetTempStats.CurrentDamageMultiplier * 2;
            UI.instance.SendNotification(target.name +"'s Attack Buffed by " + targetTempStats.CurrentDamageMultiplier.ToString());
            await HealthManager.instance.PlayerMortality(targetTempStats, attackOrder,playerTempStats);



        }
    }

    async UniTask HandleAnimation()
    {
        Transform closestTarget = TurnManager.instance.FindClosestTarget(TurnManager.instance.target, player.GetComponent<CharacterBaseClasses>());
        await TempManager.instance.CharacterRotation(closestTarget.GetComponent<CharacterBaseClasses>(), player, 2f);

        player.GetComponent<SpawnVFX>().SetTargetAnimator(target.gameObject);
        player.GetComponent<SpawnVFX>().SetTargetVFXPosition(target.gameObject);
        player.GetComponent<SpawnVFX>().SetOwnVFXPosition(player.gameObject.GetComponent<VFXSpawnPosition>().MidBody);
        player.GetComponent<SpawnVFX>().SetVFXPrefab(buffAttack.PlayerActionVFX);
        player.GetComponent<SpawnVFX>().SetTargetHitVFXPrefab(buffAttack.TargetHitVFX);
        player.GetComponent<SpawnVFX>().SetParticle(buffAttack.particle);
        player.GetComponent<SpawnVFX>().SetVFXSound(buffAttack.actionSound);
        player.GetComponent<SpawnVFX>().SetTargetAnimation(buffAttack.TargetHurtAnimation);

        await CutsceneManager.instance.PlayAnimationForCharacter(player.gameObject, GetActionName());

        //player.GetComponent<ArrowSpawner>().SpawnArrow(player.gameObject, target.gameObject);
    }


    public string GetActionName()
    {
        return buffAttack.ActionName;
    }

    public int GetPVValue()
    {
        return buffAttack.PriorityValue;
    }

    public CharacterBaseClasses GetTarget()
    {
        return target;
    }

    public int GetAPValue()
    {
        return buffAttack.APCost;
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
