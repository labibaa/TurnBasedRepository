using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class SoulSteal : ICommand
{
    CharacterBaseClasses player;
    CharacterBaseClasses target;
    TemporaryStats playerTempStats;
    TemporaryStats targetTempStats;
    ImprovedActionStat rangedAttack;


    public SoulSteal(CharacterBaseClasses playerAttacker, CharacterBaseClasses targetDefender, TemporaryStats currentStatPlayer, TemporaryStats currentStatTarget, ImprovedActionStat rangedScriptable)
    {
        player = playerAttacker;
        target = targetDefender;
        playerTempStats = currentStatPlayer;
        targetTempStats = currentStatTarget;
        rangedAttack = rangedScriptable;

    }
    public async UniTask Execute()
    {

        int attackOrder = checkOrder();

        float actionAccuracy = rangedAttack.ActionAccuracy;

        if (ActionResolver.instance.ActionAccuracyCalculation(actionAccuracy))
        {
            int diceValue = DiceNumberGenerator.instance.GetDiceValue(rangedAttack.FirstPercentage, rangedAttack.SecondPercentage, rangedAttack.LastPercentage);

            int damage = Mathf.RoundToInt(ActionResolver.instance.CalculateNewDamage(diceValue, rangedAttack) * playerTempStats.CurrentDamageMultiplier);
            UI.instance.SendNotification(diceValue.ToString());
            await HandleAnimation();
            targetTempStats.CurrentHealth = HealthManager.instance.HealthCalculation(damage, targetTempStats.CurrentHealth);
            playerTempStats.CurrentHealth = HealthManager.instance.HealthCap(playerTempStats.PlayerHealth, HealthManager.instance.HealthCalculation(( damage * -1), playerTempStats.CurrentHealth));
            UI.instance.ShowFlyingText((damage * -1).ToString(), target.GetComponent<TemporaryStats>().FlyingTextParent, Color.red);
            UI.instance.ShowFlyingText((damage).ToString(), player.GetComponent<TemporaryStats>().FlyingTextParent, Color.green);
            await HealthManager.instance.PlayerMortality(targetTempStats, targetTempStats);

        }
    }
    async UniTask HandleAnimation()
    {
        await TempManager.instance.CharacterRotation(target, player, 2f);

        player.GetComponent<SpawnVFX>().SetTargetAnimator(target.gameObject);
        player.GetComponent<SpawnVFX>().SetOwnVFXPosition(player.GetComponent<VFXSpawnPosition>().CharacterBodyPosition[rangedAttack.CharacterBodyLocation]);
        player.GetComponent<SpawnVFX>().SetTargetVFXPosition(target.GetComponent<VFXSpawnPosition>().CharacterBodyPosition[rangedAttack.TargetCharacterBodyLocation]);
        player.GetComponent<SpawnVFX>().SetVFXPrefab(rangedAttack.PlayerActionVFX);
        player.GetComponent<SpawnVFX>().SetTargetHitVFXPrefab(rangedAttack.TargetHitVFX);
        player.GetComponent<SpawnVFX>().SetParticle(rangedAttack.particle);
        player.GetComponent<SpawnVFX>().SetVFXSound(rangedAttack.actionSound);
        player.GetComponent<SpawnVFX>().SetTargetAnimation(rangedAttack.TargetHurtAnimation);



        // CutsceneManager.instance.virtualCamera.Priority = 15;

        await CutsceneManager.instance.PlayAnimationForCharacter(player.gameObject, GetActionName());

        //player.GetComponent<ArrowSpawner>().SpawnArrow(player.gameObject, target.gameObject);
    }


    public string GetActionName()
    {
        return rangedAttack.ActionName;
    }

    public int GetPVValue()
    {
        return rangedAttack.PriorityValue;
    }

    public CharacterBaseClasses GetTarget()
    {
        return target;
    }

    public int GetAPValue()
    {
        return rangedAttack.APCost;
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
