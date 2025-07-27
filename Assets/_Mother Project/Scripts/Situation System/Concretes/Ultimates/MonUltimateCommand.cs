using Cysharp.Threading.Tasks;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UIElements;

public class MonUltimateCommand : IUltimate, ICommand
{
    
    CharacterBaseClasses playerCharacter;
    CharacterBaseClasses targetCharacter;
    TemporaryStats playerTempStats;
    TemporaryStats targetTempStats;
    UltimateActionsFactory ultimateScriptable;
     

    public MonUltimateCommand(UltimateActionsFactory ultimateScritableObject)
    {
        ultimateScriptable = ultimateScritableObject;
    }
    public void setValues(CharacterBaseClasses playerCh, TemporaryStats playerTemp, CharacterBaseClasses targetCh, TemporaryStats targetTemp)
    {
        playerCharacter = playerCh;
        playerTempStats = playerTemp;
        targetCharacter = targetCh;
        targetTempStats = targetTemp;
    }
    public async void ExecuteUltimate()
    {
        if (!ultimateScriptable.isUltimateSingleTarget)
        {
            List<CharacterBaseClasses> targets = GridMovement.instance.InAdjacentMatrix(playerTempStats.currentPlayerGridPosition, playerTempStats.CharacterTeam, ultimateScriptable.ultimateRange, Color.clear);
            GridMovement.instance.ResetHighlightedPath();
            playerCharacter.GetComponent<SpawnVFX>().SetVFXSound(ultimateScriptable.actionSound);

            foreach (CharacterBaseClasses target in targets)
            {
                TemporaryStats targetTempStats = target.GetComponent<TemporaryStats>();

                targetTempStats.CurrentHealth = HealthManager.instance.HealthCalculation(targetTempStats.CurrentHealth / 2, targetTempStats.CurrentHealth);

                playerCharacter.GetComponent<SpawnVFX>().SetTargetAnimator(target.gameObject);
                playerCharacter.GetComponent<SpawnVFX>().SetTargetVFXPosition(target.GetComponent<VFXSpawnPosition>().CharacterBodyPosition[ultimateScriptable.TargetCharacterBodyLocation]);

                UI.instance.ShowFlyingText((targetTempStats.CurrentHealth / 2).ToString(), targetTempStats.FlyingTextParent, Color.red);
                //CutsceneManager.instance.PlayAnimationForCharacter(target.gameObject, "Hurt");
            }
          
        }
        else
        {
           // GridMovement.instance.ResetHighlightedPath();
           /* playerCharacter.GetComponent<SpawnVFX>().SetTargetAnimator(targetCharacter.gameObject);
            playerCharacter.GetComponent<SpawnVFX>().SetTargetVFXPosition(targetCharacter.GetComponent<VFXSpawnPosition>().CharacterBodyPosition[ultimateScriptable.TargetCharacterBodyLocation]);*/
         //   PlayerStatUI.instance.UpdateSummaryHUDUI();
            Debug.Log("Ultimate Single executed");
            UI.instance.SendNotification("ulti single target");
           // await HandleAnimation();
            UltimateSystem._instance.IsUltimate = false;


/*            GridMovement.instance.ResetHighlightedPath();
            TurnManager.instance.ResetTargetHIghlightVisual();
            TurnManager.instance.targetsInRange.Clear();
            TurnManager.instance.nonCharacterTargetsInRange.Clear();
            TempManager.instance.ChangeGameState(GameStates.Simulation);
            playerCharacter.GetComponent<PlayerTurn>().isMoveOn = true;
            TurnManager.instance.StartTurn();*/
            //TurnManager.instance.EndTurn();
        }
    }
    async UniTask HandleAnimation()
    {
        TempManager.instance.CharacterRotation(targetCharacter, playerCharacter, 2f);

        playerCharacter.GetComponent<SpawnVFX>().SetOwnVFXPosition(playerCharacter.GetComponent<VFXSpawnPosition>().CharacterBodyPosition[ultimateScriptable.CharacterBodyLocation]);
        playerCharacter.GetComponent<SpawnVFX>().SetVFXPrefab(ultimateScriptable.PlayerActionVFX);
        playerCharacter.GetComponent<SpawnVFX>().SetTargetHitVFXPrefab(ultimateScriptable.TargetHitVFX);
        playerCharacter.GetComponent<SpawnVFX>().SetParticle(ultimateScriptable.particle);
        playerCharacter.GetComponent<SpawnVFX>().SetVFXSound(ultimateScriptable.actionSound);
        playerCharacter.GetComponent<SpawnVFX>().SetTargetAnimation(ultimateScriptable.TargetHurtAnimation);

        playerCharacter.GetComponent<SpawnVFX>().SetTargetAnimator(targetCharacter.gameObject);
        playerCharacter.GetComponent<SpawnVFX>().SetTargetVFXPosition(targetCharacter.GetComponent<VFXSpawnPosition>().CharacterBodyPosition[ultimateScriptable.TargetCharacterBodyLocation]);
        await CutsceneManager.instance.PlayAnimationForCharacter(playerCharacter.gameObject, GetUltimateActionName());
    }

    public int GetultimateThreshold()
    {
       
        return ultimateScriptable.actionThreshold;
    }
    public string GetUltimateActionName()
    {
        return ultimateScriptable.UltimateName;
    }

    public bool IsSingleTarget()
    {
        return ultimateScriptable.isUltimateSingleTarget;
    }
    // interface functions of ICommand
    public async UniTask Execute()
    {
         ExecuteUltimate();
         await HandleAnimation();
        // PlayerStatUI.instance.UpdateSummaryHUDUI();
        Debug.Log("Ultimate executed");
    }

    public int GetPVValue()
    {
        return 0;
    }

    public int GetAPValue()
    {
        return 0 ;
    }

    public string GetActionName()
    {
        return ultimateScriptable.name ;
    }

    public CharacterBaseClasses GetTarget()
    {
        return targetCharacter;
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
        return "Ultimate";
    }
}
