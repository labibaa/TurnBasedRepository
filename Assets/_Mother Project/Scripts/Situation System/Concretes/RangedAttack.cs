using Cinemachine;
using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class RangedAttack : ICommand
{
    CharacterBaseClasses player;
    CharacterBaseClasses target;
    TemporaryStats playerTempStats;
    TemporaryStats targetTempStats;
    ImprovedActionStat rangedAttack;


    public RangedAttack(CharacterBaseClasses playerAttacker, CharacterBaseClasses targetDefender, TemporaryStats currentStatPlayer, TemporaryStats currentStatTarget, ImprovedActionStat rangedScriptable)
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
        if (targetTempStats.IsDodgeActive)
        {
            actionAccuracy = actionAccuracy - 50;
        }
        //if (targetTempStats.IsBlockActive)
        //{
        //    greaterStrike.BasePower = greaterStrike.BasePower - 30;
        //}
        int fp = rangedAttack.FirstPercentage;
        int sp = rangedAttack.SecondPercentage;
        int lp = rangedAttack.LastPercentage;

        if (playerTempStats.CurrentHealth <= player.HealthPoints * 0.2)
        {
            Debug.Log(playerTempStats.CurrentHealth + " >= " + player.HealthPoints * 0.2);
            rangedAttack.FirstPercentage = 0;
            rangedAttack.SecondPercentage = 20;
            rangedAttack.LastPercentage = 80;
        }

        if (ActionResolver.instance.ActionAccuracyCalculation(actionAccuracy))
        {
            //rangedAttack.FirstPercentage ==> this percentage can be used as crit chance logic
            int diceValue = DiceNumberGenerator.instance.GetDiceValue(rangedAttack.FirstPercentage, rangedAttack.SecondPercentage, rangedAttack.LastPercentage);    

            int damage = Mathf.RoundToInt(( ActionResolver.instance.CalculateNewDamage(diceValue, rangedAttack) * playerTempStats.CurrentDamageMultiplier )+ (Mathf.FloorToInt((playerTempStats.CurrentDex + 3) / 2f) + 1));//(dexterity dmg effect formula )
            UI.instance.SendNotification(diceValue.ToString());
            if (targetTempStats.IsBlockActive)
            {
                damage = damage / 2;
                targetTempStats.IsBlockActive= false;
            }

            if (targetTempStats.IsCounterActive)
            {
                await HandleAnimation();
                //playerTempStats.CurrentHealth = Mathf.Max(HealthManager.instance.HealthCalculation(damage / 2, playerTempStats.CurrentHealth), 1); 
                targetTempStats.CurrentHealth = HealthManager.instance.HealthCalculation(damage, targetTempStats.CurrentHealth);
               UI.instance.ShowFlyingText((damage * -1).ToString(), player.GetComponent<TemporaryStats>().FlyingTextParent, Color.red);
               await HealthManager.instance.PlayerMortality(playerTempStats, playerTempStats);
               await HealthManager.instance.PlayerMortality(targetTempStats, playerTempStats);
               targetTempStats.IsCounterActive= false;

                //ImprovedActionStat meleeScriptable = DAOScriptableObject.instance.GetImprovedActionData(StringData.directory, "Punch");
                //ICommand meleeAction = new MeleeAttack(target, player, targetTempStats, playerTempStats, meleeScriptable, "SingleMelee");
                //meleeAction.Execute();
                //targetTempStats.IsCounterActive = false;
            }
            else
            {

                await HandleAnimation();
                targetTempStats.CurrentHealth = HealthManager.instance.HealthCalculation(damage, targetTempStats.CurrentHealth);
                UI.instance.ShowFlyingText((damage * -1).ToString(), target.GetComponent<TemporaryStats>().FlyingTextParent, Color.red);
                await HealthManager.instance.PlayerMortality(targetTempStats, playerTempStats);
            }


        }
        rangedAttack.FirstPercentage = fp;
        rangedAttack.SecondPercentage = sp;
        rangedAttack.LastPercentage = lp;
    }


   async UniTask  HandleAnimation()
   {
        await TempManager.instance.CharacterRotation(target, player, 2f);

      /*  player.GetComponent<PlayParticle>().target = target.gameObject;
        player.GetComponent<PlayParticle>().actionSound = rangedAttack.actionSound;
        //player.GetComponent<PlayParticle>().InstantiateParticleEffect(rangedAttack.ParticleSystem);
        player.GetComponent<PlayParticle>().particlePrefab = rangedAttack.ParticleSystem;
        player.GetComponent<PlayParticle>().particlePrefabHit = rangedAttack.HitParticleSystem;
        target.GetComponent<PlayParticle>().particlePrefabHurt = rangedAttack.HurtParticleSystem;
        Debug.Log("RangedD");
*/
        //CutsceneManager.instance.virtualCamera.Follow = player.gameObject.transform;
        //CutsceneManager.instance.virtualCamera.LookAt = player.gameObject.transform;

        //CutsceneManager.instance.targetGroup.m_Targets = new CinemachineTargetGroup.Target[0];
        //CutsceneManager.instance.targetGroup.AddMember(target.gameObject.transform, 3, 4);
        //CutsceneManager.instance.targetGroup.AddMember(player.gameObject.transform, 3, 4);

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
