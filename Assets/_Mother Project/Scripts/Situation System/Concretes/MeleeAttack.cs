

using Cinemachine;
using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.TextCore.Text;

public class MeleeAttack : ICommand
{
    CharacterBaseClasses player;
    CharacterBaseClasses target;
    TemporaryStats playerTempStats;
    TemporaryStats targetTempStats;
    ImprovedActionStat meleeAttack;
    string ActionType;
    


    public MeleeAttack(CharacterBaseClasses playerAttacker, CharacterBaseClasses targetDefender, TemporaryStats currentStatPlayer, TemporaryStats currentStatTarget, ImprovedActionStat meleeScriptable,string actionType)
    {
        player = playerAttacker;
        target = targetDefender;
        playerTempStats = currentStatPlayer;
        targetTempStats = currentStatTarget;
        meleeAttack = meleeScriptable;
        ActionType = actionType;

    }







    public async UniTask Execute()
    {
       
        int attackOrder = checkOrder();

        float actionAccuracy = meleeAttack.ActionAccuracy;
        if (targetTempStats.IsDodgeActive)
        {
            actionAccuracy = actionAccuracy - 50;
        }
        //if (targetTempStats.IsBlockActive)
        //{
        //    greaterStrike.BasePower = greaterStrike.BasePower - 30;
        //}

        if (ActionResolver.instance.ActionAccuracyCalculation(actionAccuracy) )
        {
            int diceValue = DiceNumberGenerator.instance.GetDiceValue(meleeAttack.FirstPercentage, meleeAttack.SecondPercentage, meleeAttack.LastPercentage);
            UI.instance.SendNotification(diceValue.ToString());
            int damage =Mathf.RoundToInt(ActionResolver.instance.CalculateNewDamage(diceValue, meleeAttack) * playerTempStats.CurrentDamageMultiplier);
            Debug.Log("Dice: " + diceValue + " Damage: " + damage);
            if (targetTempStats.IsBlockActive)
            {
                damage = damage / 2;
                targetTempStats.IsBlockActive = false;
            }

            if (targetTempStats.IsCounterActive)
            {
                //damages attacker is counter on
                playerTempStats.CurrentHealth = Mathf.Max(HealthManager.instance.HealthCalculation(damage / 2, playerTempStats.CurrentHealth), 1);
                targetTempStats.CurrentHealth = HealthManager.instance.HealthCalculation(damage, targetTempStats.CurrentHealth);
                await HandleAnimation();
                UI.instance.ShowFlyingText((damage * -1).ToString(), player.GetComponent<TemporaryStats>().FlyingTextParent, Color.red);
                await HealthManager.instance.PlayerMortality(playerTempStats,attackOrder, playerTempStats);
                await HealthManager.instance.PlayerMortality(targetTempStats, attackOrder, playerTempStats);

                //ImprovedActionStat meleeScriptable = DAOScriptableObject.instance.GetImprovedActionData(StringData.directory, "Punch");
                //ICommand meleeAction = new MeleeAttack(target, player, targetTempStats, playerTempStats, meleeScriptable, "SingleMelee");
                //meleeAction.Execute();
                //targetTempStats.IsCounterActive= false;
            }
            else
            {
                targetTempStats.CurrentHealth = HealthManager.instance.HealthCalculation(damage, targetTempStats.CurrentHealth);
                
                await HandleAnimation();

                UI.instance.ShowFlyingText((damage * -1).ToString(), target.GetComponent<TemporaryStats>().FlyingTextParent, Color.red);
                await HealthManager.instance.PlayerMortality(targetTempStats,attackOrder, playerTempStats);

            }


        }
    }


    async UniTask HandleAnimation()
    {
        TempManager.instance.CharacterRotation(target, player, 2f);
      /*  player.GetComponent<PlayParticle>().target = target.gameObject;
        player.GetComponent<PlayParticle>().actionSound = meleeAttack.actionSound;
        // player.GetComponent<PlayParticle>().InstantiateParticleEffect(meleeAttack.ParticleSystem);

        player.GetComponent<PlayParticle>().particlePrefab = meleeAttack.ParticleSystem;
        player.GetComponent<PlayParticle>().particlePrefabHit = meleeAttack.HitParticleSystem;
        target.GetComponent<PlayParticle>().particlePrefabHurt = meleeAttack.HurtParticleSystem;*/

        //CutsceneManager.instance.virtualCamera.Follow = player.gameObject.transform;
        //CutsceneManager.instance.virtualCamera.LookAt = player.gameObject.transform;

        //CutsceneManager.instance.targetGroup.m_Targets = new CinemachineTargetGroup.Target[0];
        //CutsceneManager.instance.targetGroup.AddMember(target.gameObject.transform, 3, 4);
        //CutsceneManager.instance.targetGroup.AddMember(player.gameObject.transform, 3, 4);

        player.GetComponent<SpawnVFX>().SetTargetAnimator(target.gameObject);
        player.GetComponent<SpawnVFX>().SetOwnVFXPosition(player.GetComponent<VFXSpawnPosition>().CharacterBodyPosition[meleeAttack.CharacterBodyLocation]);
        player.GetComponent<SpawnVFX>().SetTargetVFXPosition(target.GetComponent<VFXSpawnPosition>().CharacterBodyPosition[meleeAttack.TargetCharacterBodyLocation]);
        player.GetComponent<SpawnVFX>().SetVFXPrefab(meleeAttack.PlayerActionVFX);
        player.GetComponent<SpawnVFX>().SetTargetHitVFXPrefab(meleeAttack.TargetHitVFX);
        player.GetComponent<SpawnVFX>().SetParticle(meleeAttack.particle);
        player.GetComponent<SpawnVFX>().SetVFXSound(meleeAttack.actionSound);
        player.GetComponent<SpawnVFX>().SetTargetAnimation(meleeAttack.TargetHurtAnimation);

        //CutsceneManager.instance.virtualCamera.Priority = 15;
        await CutsceneManager.instance.PlayAnimationForCharacter(player.gameObject, GetActionName());
        


        // CutsceneManager.instance.PlayAnimationForCharacter(target.gameObject,"Hurt");

    }


    public string GetActionName()
    {
        return meleeAttack.ActionName;
    }

    public int GetPVValue()
    {
        return meleeAttack.PriorityValue;
    }

    public CharacterBaseClasses GetTarget()
    {
        return target;
    }
    public int GetAPValue()
    {
        return meleeAttack.APCost;
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
       return TurnManager.instance.players.IndexOf(target.GetComponent<PlayerTurn>())  - TurnManager.instance.players.IndexOf(player.GetComponent<PlayerTurn>());
       
    }
}
