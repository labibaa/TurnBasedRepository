using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using TrailsFX.Demos;
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

        //if (targetTempStats.IsBlockActive)
        //{
        //    greaterStrike.BasePower = greaterStrike.BasePower - 30;
        //}

        if (ActionResolver.instance.ActionAccuracyCalculation(actionAccuracy))
        {
            targetTempStats.CurrentDamageMultiplier = targetTempStats.CurrentDamageMultiplier * 2;
            UI.instance.SendNotification(target.name +"'s Attack Buffed by " + targetTempStats.CurrentDamageMultiplier.ToString());
            await HandleAnimation();
            await HealthManager.instance.PlayerMortality(targetTempStats, attackOrder,playerTempStats);



        }
    }

    /*    private void OnEnable()
        {
            HandleTurnNew.OnTurnEnd += DamageCurrentTarget;
        }
        private void OnDisable()
        {
            HandleTurnNew.OnTurnEnd -= DamageCurrentTarget;
        }


        void DamageCurrentTarget()
        {
            ExecuteDamageCurrentTarget();
        }

        async UniTask ExecuteDamageCurrentTarget()
        {
            if (buffAttack.PriorityValue > 0)
            {
                if (target != null)
                {
                    target.DamageMultiplier = target.DamageMultiplier * 2;
                    CutsceneManager.instance.PlayAnimationForCharacter(target.gameObject, buffAttack.TargetHurtAnimation);

                    buffAttack.PriorityValue--;
                }
                else
                {
                    if (HasEffect)
                    {
                        ResetEffectState();
                    }

                }


            }
        }

        private void ResetEffectState()
        {
            //target.playerVisiblity = 1;
            HasEffect = false;
            EffectOwner = null;
            GridPosition = Vector2.zero;
            buffAttack.PriorityValue = 0;
            Smoke = null;
            Destroy(SmokeObject);
            foreach (PlayerTurn pturn in TurnManager.instance.players)
            {
                pturn.GetComponent<TemporaryStats>().playerVisiblity = 1;
            }


        }*/

    async UniTask HandleAnimation()
    {
        Transform closestTarget = TurnManager.instance.FindClosestTarget(TurnManager.instance.target, player.GetComponent<CharacterBaseClasses>());
        TempManager.instance.CharacterRotation(closestTarget.GetComponent<CharacterBaseClasses>(), player, 2f);

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
