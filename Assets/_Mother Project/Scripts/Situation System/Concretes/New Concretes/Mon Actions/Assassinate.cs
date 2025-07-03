using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Assassinate : ICommand
{
    CharacterBaseClasses player;
    CharacterBaseClasses target;
    TemporaryStats playerTempStats;
    TemporaryStats targetTempStats;
    ImprovedActionStat assasinateAttack;
    string ActionType;



    public Assassinate(CharacterBaseClasses playerAttacker, CharacterBaseClasses targetDefender, TemporaryStats currentStatPlayer, TemporaryStats currentStatTarget, ImprovedActionStat assassinateScriptable, string actionType)
    {
        player = playerAttacker;
        target = targetDefender;
        playerTempStats = currentStatPlayer;
        targetTempStats = currentStatTarget;
        assasinateAttack = assassinateScriptable;
        ActionType = actionType;

    }
    public async UniTask Execute()
    {

        int attackOrder = checkOrder();

        float actionAccuracy = assasinateAttack.ActionAccuracy;

        if (ActionResolver.instance.ActionAccuracyCalculation(actionAccuracy))
        {
            if (target.characterClass == "Talker") 
            {
                await HandleAnimation();
                targetTempStats.CurrentHealth = HealthManager.instance.HealthCalculation(targetTempStats.CurrentHealth, targetTempStats.CurrentHealth);
                await HealthManager.instance.PlayerMortality(targetTempStats, attackOrder, playerTempStats);
            }
            else
            {
                int diceValue = DiceNumberGenerator.instance.GetDiceValue(assasinateAttack.FirstPercentage, assasinateAttack.SecondPercentage, assasinateAttack.LastPercentage);
                UI.instance.SendNotification(diceValue.ToString());
                int damage = Mathf.RoundToInt(ActionResolver.instance.CalculateNewDamage(diceValue, assasinateAttack) * playerTempStats.CurrentDamageMultiplier);
                Debug.Log("Dice: " + diceValue + " Damage: " + damage);
                await HandleAnimation();
                targetTempStats.CurrentHealth = HealthManager.instance.HealthCalculation(damage, targetTempStats.CurrentHealth);
                UI.instance.ShowFlyingText((damage * -1).ToString(), target.GetComponent<TemporaryStats>().FlyingTextParent, Color.red);
                await HealthManager.instance.PlayerMortality(targetTempStats, attackOrder, playerTempStats);
            }
        }
    }


    async UniTask HandleAnimation()
    {
        await TempManager.instance.CharacterRotation(target, player, 2f);

        player.GetComponent<SpawnVFX>().SetTargetAnimator(target.gameObject);
        player.GetComponent<SpawnVFX>().SetOwnVFXPosition(player.GetComponent<VFXSpawnPosition>().CharacterBodyPosition[assasinateAttack.CharacterBodyLocation]);
        player.GetComponent<SpawnVFX>().SetTargetVFXPosition(target.GetComponent<VFXSpawnPosition>().CharacterBodyPosition[assasinateAttack.TargetCharacterBodyLocation]);
        player.GetComponent<SpawnVFX>().SetVFXPrefab(assasinateAttack.PlayerActionVFX);
        player.GetComponent<SpawnVFX>().SetTargetHitVFXPrefab(assasinateAttack.TargetHitVFX);
        player.GetComponent<SpawnVFX>().SetParticle(assasinateAttack.particle);
        player.GetComponent<SpawnVFX>().SetVFXSound(assasinateAttack.actionSound);
        player.GetComponent<SpawnVFX>().SetTargetAnimation(assasinateAttack.TargetHurtAnimation);

        await CutsceneManager.instance.PlayAnimationForCharacter(player.gameObject, GetActionName());


    }


    public string GetActionName()
    {
        return assasinateAttack.ActionName;
    }

    public int GetPVValue()
    {
        return assasinateAttack.PriorityValue;
    }

    public CharacterBaseClasses GetTarget()
    {
        return target;
    }
    public int GetAPValue()
    {
        return assasinateAttack.APCost;
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
