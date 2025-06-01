using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Debuff : ICommand
{
    CharacterBaseClasses player;
    CharacterBaseClasses target;
    TemporaryStats playerTempStats;
    TemporaryStats targetTempStats;
    ImprovedActionStat debuffAttack;


    public Debuff(CharacterBaseClasses playerAttacker, CharacterBaseClasses targetDefender, TemporaryStats currentStatPlayer, TemporaryStats currentStatTarget, ImprovedActionStat DebuffScriptable)
    {
        player = playerAttacker;
        target = targetDefender;
        playerTempStats = currentStatPlayer;
        targetTempStats = currentStatTarget;
        debuffAttack = DebuffScriptable;

    }



    public async UniTask Execute()
    {

        int attackOrder = checkOrder();

        float actionAccuracy = debuffAttack.ActionAccuracy;

        //if (targetTempStats.IsBlockActive)
        //{
        //    greaterStrike.BasePower = greaterStrike.BasePower - 30;
        //}

        if (ActionResolver.instance.ActionAccuracyCalculation(actionAccuracy))
        {
            targetTempStats.CurrentDamageMultiplier = targetTempStats.CurrentDamageMultiplier * 0.5f;
            UI.instance.SendNotification(target.name + "'s Attack debuffed by " + targetTempStats.CurrentDamageMultiplier.ToString());
            await HandleAnimation();
            await HealthManager.instance.PlayerMortality(targetTempStats, attackOrder, playerTempStats);



        }
    }

    async UniTask HandleAnimation()
    {
        Transform closestTarget = TurnManager.instance.FindClosestTarget(TurnManager.instance.target, player.GetComponent<CharacterBaseClasses>());
        await TempManager.instance.CharacterRotation(closestTarget.GetComponent<CharacterBaseClasses>(), player, 2f);

        /*   player.GetComponent<PlayParticle>().target = target.gameObject;
           player.GetComponent<PlayParticle>().actionSound = rangedAttack.actionSound;
           //player.GetComponent<PlayParticle>().InstantiateParticleEffect(rangedAttack.ParticleSystem);
           player.GetComponent<PlayParticle>().particlePrefab = rangedAttack.ParticleSystem;
           player.GetComponent<PlayParticle>().particlePrefabHit = rangedAttack.HitParticleSystem;
           target.GetComponent<PlayParticle>().particlePrefabHurt = rangedAttack.HurtParticleSystem;
           Debug.Log("RangedD");*/

        //CutsceneManager.instance.virtualCamera.LookAt = player.gameObject.transform;
        //CutsceneManager.instance.virtualCamera.Follow = player.gameObject.transform;
        //CutsceneManager.instance.virtualCamera.Priority = 15;

        player.GetComponent<SpawnVFX>().SetTargetAnimator(target.gameObject);
        player.GetComponent<SpawnVFX>().SetTargetVFXPosition(target.gameObject);
        player.GetComponent<SpawnVFX>().SetOwnVFXPosition(player.gameObject.GetComponent<VFXSpawnPosition>().MidBody);
        player.GetComponent<SpawnVFX>().SetVFXPrefab(debuffAttack.PlayerActionVFX);
        player.GetComponent<SpawnVFX>().SetTargetHitVFXPrefab(debuffAttack.TargetHitVFX);
        player.GetComponent<SpawnVFX>().SetParticle(debuffAttack.particle);
        player.GetComponent<SpawnVFX>().SetVFXSound(debuffAttack.actionSound);
        player.GetComponent<SpawnVFX>().SetTargetAnimation(debuffAttack.TargetHurtAnimation);

        await CutsceneManager.instance.PlayAnimationForCharacter(player.gameObject, GetActionName());

        //player.GetComponent<ArrowSpawner>().SpawnArrow(player.gameObject, target.gameObject);
    }


    public string GetActionName()
    {
        return debuffAttack.ActionName;
    }

    public int GetPVValue()
    {
        return debuffAttack.PriorityValue;
    }

    public CharacterBaseClasses GetTarget()
    {
        return target;
    }

    public int GetAPValue()
    {
        return debuffAttack.APCost;
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
