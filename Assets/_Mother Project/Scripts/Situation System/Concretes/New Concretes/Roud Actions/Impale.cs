using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Impale : ICommand
{

    TemporaryStats playerTempStats;
    CharacterBaseClasses playerBaseClass;
    ImprovedActionStat actionScriptable;
    CharacterBaseClasses closestTarget;

    public Impale(ImprovedActionStat action, TemporaryStats PlayerTempStats, CharacterBaseClasses PlayerBaseClass, CharacterBaseClasses ClosestTarget)
    {
        actionScriptable = action;
        playerTempStats = PlayerTempStats;
        playerBaseClass = PlayerBaseClass;
        closestTarget = ClosestTarget;
    }


    public async UniTask Execute()
    {
        List<CharacterBaseClasses> targetsInRange = GridMovement.instance.InAdjacentMatrix(playerTempStats.currentPlayerGridPosition, playerTempStats.CharacterTeam, actionScriptable.ActionRange, Color.clear);
        GridMovement.instance.ResetHighlightedPath();
        int diceValue = DiceNumberGenerator.instance.GetDiceValue(actionScriptable.FirstPercentage, actionScriptable.SecondPercentage, actionScriptable.LastPercentage);
        UI.instance.SendNotification(diceValue.ToString());
        int damage = Mathf.RoundToInt(ActionResolver.instance.CalculateNewDamage(diceValue, actionScriptable) * playerTempStats.CurrentDamageMultiplier);
        Debug.Log("Dice: " + diceValue + " Damage: " + damage);
        await HandleAnimation();
        foreach (CharacterBaseClasses target in targetsInRange)
        {
            int attackOrder = checkOrder(target);
            TemporaryStats targetTempStat = target.GetComponent<TemporaryStats>();
            CutsceneManager.instance.PlayAnimationForCharacter(target.gameObject, actionScriptable.TargetHurtAnimation);
            targetTempStat.CurrentHealth = HealthManager.instance.HealthCalculation(damage, targetTempStat.CurrentHealth);
            UI.instance.ShowFlyingText((damage * -1).ToString(), targetTempStat.FlyingTextParent, Color.red);
            await HealthManager.instance.PlayerMortality(targetTempStat, attackOrder, playerTempStats);
        }

    }
    async UniTask HandleAnimation()
    {

          //  playerBaseClass.GetComponent<SpawnVFX>().SetTargetAnimator(target.gameObject);
        playerBaseClass.GetComponent<SpawnVFX>().SetOwnVFXPosition(playerBaseClass.GetComponent<VFXSpawnPosition>().CharacterBodyPosition[actionScriptable.CharacterBodyLocation]);
           // playerBaseClass.GetComponent<SpawnVFX>().SetTargetVFXPosition(target.GetComponent<VFXSpawnPosition>().CharacterBodyPosition[actionScriptable.TargetCharacterBodyLocation]);
        playerBaseClass.GetComponent<SpawnVFX>().SetVFXPrefab(actionScriptable.PlayerActionVFX);
        playerBaseClass.GetComponent<SpawnVFX>().SetTargetHitVFXPrefab(actionScriptable.TargetHitVFX);
        playerBaseClass.GetComponent<SpawnVFX>().SetParticle(actionScriptable.particle);
        playerBaseClass.GetComponent<SpawnVFX>().SetVFXSound(actionScriptable.actionSound);
        playerBaseClass.GetComponent<SpawnVFX>().SetTargetAnimation(actionScriptable.TargetHurtAnimation);
        await CutsceneManager.instance.PlayAnimationForCharacter(playerBaseClass.gameObject, GetActionName());
    }

    public string GetActionName()
    {
        return actionScriptable.ActionName;
    }

    public int GetPVValue()
    {
        return 0;
    }

    public CharacterBaseClasses GetTarget()
    {
        return closestTarget;
    }
    public int GetAPValue()
    {
        return actionScriptable.APCost;
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
    int checkOrder(CharacterBaseClasses target)
    {
        return TurnManager.instance.players.IndexOf(target.GetComponent<PlayerTurn>()) - TurnManager.instance.players.IndexOf(playerBaseClass.GetComponent<PlayerTurn>());

    }
}
