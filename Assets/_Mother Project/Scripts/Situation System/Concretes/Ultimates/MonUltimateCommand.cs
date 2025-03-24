using Cysharp.Threading.Tasks;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class MonUltimateCommand : IUltimate
{
    
    CharacterBaseClasses playerCharacter;
    TemporaryStats playerTempStats;
    UltimateActionsFactory ultimateScriptable;
     

    public MonUltimateCommand(UltimateActionsFactory ultimateScritableObject)
    {
        ultimateScriptable = ultimateScritableObject;
    }
    public async void Execute()
    {
        if (!ultimateScriptable.isUltimateSingleTarget)
        {
            List<CharacterBaseClasses> targets = GridMovement.instance.InAdjacentMatrix(playerTempStats.currentPlayerGridPosition, playerTempStats.CharacterTeam, ultimateScriptable.ultimateRange, Color.clear);
            GridMovement.instance.ResetHighlightedPath();
            playerCharacter.GetComponent<SpawnVFX>().SetVFXSound(ultimateScriptable.actionSound);

            // CutsceneManager.instance.virtualCamera.LookAt = playerCharacter.gameObject.transform;
            // CutsceneManager.instance.virtualCamera.Follow = playerCharacter.gameObject.transform;

            //CutsceneManager.instance.virtualCamera.Priority = 15;
            //await CutsceneManager.instance.PlayAnimationForCharacter(playerCharacter.gameObject, "Ult1");
            foreach (CharacterBaseClasses target in targets)
            {
                TemporaryStats targetTempStats = target.GetComponent<TemporaryStats>();

                targetTempStats.CurrentHealth = HealthManager.instance.HealthCalculation(targetTempStats.CurrentHealth / 2, targetTempStats.CurrentHealth);

                playerCharacter.GetComponent<SpawnVFX>().SetTargetAnimator(target.gameObject);
                playerCharacter.GetComponent<SpawnVFX>().SetTargetVFXPosition(target.GetComponent<VFXSpawnPosition>().CharacterBodyPosition[ultimateScriptable.TargetCharacterBodyLocation]);

                UI.instance.ShowFlyingText((targetTempStats.CurrentHealth / 2).ToString(), targetTempStats.FlyingTextParent, Color.red);
                //CutsceneManager.instance.PlayAnimationForCharacter(target.gameObject, "Hurt");
            }
            await HandleAnimation();
            PlayerStatUI.instance.UpdateSummaryHUDUI();
            Debug.Log("Ultimate executed");
        }
        else
        {
            UltimateSystem._instance.IsUltimate = false;
            GridMovement.instance.ResetHighlightedPath();
            await HandleAnimation();
            PlayerStatUI.instance.UpdateSummaryHUDUI();
            Debug.Log("Ultimate ingle executed");
            UI.instance.SendNotification("ulti single target");
            GridMovement.instance.ResetHighlightedPath();
            TurnManager.instance.ResetTargetHIghlightVisual();
            TurnManager.instance.targetsInRange.Clear();
            TurnManager.instance.nonCharacterTargetsInRange.Clear();
            TempManager.instance.ChangeGameState(GameStates.Simulation);
            playerCharacter.GetComponent<PlayerTurn>().isMoveOn = true;
            TurnManager.instance.StartTurn();
            //TurnManager.instance.EndTurn();
        }
    }
    async UniTask HandleAnimation()
    {
       // TempManager.instance.CharacterRotation(target, player, 2f);

        playerCharacter.GetComponent<SpawnVFX>().SetOwnVFXPosition(playerCharacter.GetComponent<VFXSpawnPosition>().CharacterBodyPosition[ultimateScriptable.CharacterBodyLocation]);
        playerCharacter.GetComponent<SpawnVFX>().SetVFXPrefab(ultimateScriptable.PlayerActionVFX);
        playerCharacter.GetComponent<SpawnVFX>().SetTargetHitVFXPrefab(ultimateScriptable.TargetHitVFX);
        playerCharacter.GetComponent<SpawnVFX>().SetParticle(ultimateScriptable.particle);
        playerCharacter.GetComponent<SpawnVFX>().SetVFXSound(ultimateScriptable.actionSound);
        playerCharacter.GetComponent<SpawnVFX>().SetTargetAnimation(ultimateScriptable.TargetHurtAnimation);

        await CutsceneManager.instance.PlayAnimationForCharacter(playerCharacter.gameObject, GetUltimateActionName());
    }

    public void setValues(CharacterBaseClasses playerCh,TemporaryStats playerTemp)
    {
        playerCharacter= playerCh;
        playerTempStats= playerTemp;
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
}
