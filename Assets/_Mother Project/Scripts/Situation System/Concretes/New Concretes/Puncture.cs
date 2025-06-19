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
    int TurnCount;

    public Puncture(CharacterBaseClasses playerAttacker, CharacterBaseClasses targetDefender, TemporaryStats currentStatPlayer, TemporaryStats currentStatTarget, ImprovedActionStat PuntureScriptable)
    {
        player = playerAttacker;
        target = targetDefender;
        playerTempStats = currentStatPlayer;
        targetTempStats = currentStatTarget;
        puncture = PuntureScriptable;

    }

    private void OnEnable()
    {
        HandleTurnNew.OnTurnEnd += DamageCurrentTarget;
    }
    private void OnDisable()
    {
        HandleTurnNew.OnTurnEnd -= DamageCurrentTarget;
    }

    private void ResetEffectState()
    {
        TurnCount = 0;
    }
    void DamageCurrentTarget()
    {
        ExecuteDOT();
    }

    async UniTask ExecuteDOT()
    {
        if (TurnCount > 0)
        {
            int attackOrder = checkOrder();
            float actionAccuracy = puncture.ActionAccuracy;
            if (ActionResolver.instance.ActionAccuracyCalculation(actionAccuracy))
            {
                int diceValue = DiceNumberGenerator.instance.GetDiceValue(puncture.FirstPercentage, puncture.SecondPercentage, puncture.LastPercentage);
                UI.instance.SendNotification(diceValue.ToString());
                int damage = Mathf.RoundToInt(ActionResolver.instance.CalculateNewDamage(diceValue, puncture) * playerTempStats.CurrentDamageMultiplier);
                Debug.Log("Dice: " + diceValue + " Damage: " + damage);
                if (targetTempStats.IsBlockActive)
                {
                    damage = damage / 2;
                    targetTempStats.IsBlockActive = false;
                }
                else
                {

                    targetTempStats.CurrentHealth = HealthManager.instance.HealthCalculation(damage, targetTempStats.CurrentHealth);
                    CutsceneManager.instance.PlayAnimationForCharacter(targetTempStats.gameObject, puncture.TargetHurtAnimation);
                    UI.instance.ShowFlyingText((damage * -1).ToString(), target.GetComponent<TemporaryStats>().FlyingTextParent, Color.red);
                    await HealthManager.instance.PlayerMortality(targetTempStats, attackOrder, playerTempStats);

                }
            }
            TurnCount--;
        }
        else
        {
            ResetEffectState();
        }
    }

    public async UniTask Execute()
    {
        TurnCount = puncture.PriorityValue;
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
        return "Melee";
    }
    int checkOrder()
    {
        return TurnManager.instance.players.IndexOf(target.GetComponent<PlayerTurn>()) - TurnManager.instance.players.IndexOf(player.GetComponent<PlayerTurn>());

    }
}
