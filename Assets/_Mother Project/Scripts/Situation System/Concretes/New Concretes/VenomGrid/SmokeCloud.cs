using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Cysharp.Threading.Tasks;

public class SmokeCloud : ICommand
{
    CharacterBaseClasses player;
    CharacterBaseClasses target;
    TemporaryStats playerTempStats;
    TemporaryStats targetTempStats;
    ImprovedActionStat smokeCloud;
    int gridSizeX;
    int gridSizeY;

    public SmokeCloud(CharacterBaseClasses playerAttacker, CharacterBaseClasses targetDefender, TemporaryStats currentStatPlayer, TemporaryStats currentStatTarget, ImprovedActionStat smokeScriptable)
    {

        player = playerAttacker;
        target = targetDefender;
        playerTempStats = currentStatPlayer;
        targetTempStats = currentStatTarget;
        smokeCloud = smokeScriptable;
        gridSizeX = GridSystem.instance._gridArray.GetLength(0);
        gridSizeY = GridSystem.instance._gridArray.GetLength(1);
    }
    public async UniTask Execute()
    {
        Vector2 playerCoordinate = GridSystem.instance.WorldToGrid(player.transform.position);
        await HandleAnimation();
        List<Vector2> TilesToDeployGas = GridMovement.instance.GetAdjacentNeighbors(playerCoordinate, smokeCloud.ActionRange);
        Debug.Log("Smoke Count" + TilesToDeployGas.Count);
        for (int i = 0; i < TilesToDeployGas.Count; i++)
        {

            if ((TilesToDeployGas[i].x >= 0 && TilesToDeployGas[i].x < gridSizeX) && (TilesToDeployGas[i].y >= 0 && TilesToDeployGas[i].y < gridSizeY))
            {
                Debug.Log("Executed");
                GridSystem.instance._gridArray[(int)TilesToDeployGas[i].x, (int)TilesToDeployGas[i].y].GetComponent<SmokeEffector>().HasEffect = true;
                GridSystem.instance._gridArray[(int)TilesToDeployGas[i].x, (int)TilesToDeployGas[i].y].GetComponent<SmokeEffector>().EffectOwner = playerTempStats;
                GridSystem.instance._gridArray[(int)TilesToDeployGas[i].x, (int)TilesToDeployGas[i].y].GetComponent<SmokeEffector>().Smoke = smokeCloud;
                GridSystem.instance._gridArray[(int)TilesToDeployGas[i].x, (int)TilesToDeployGas[i].y].GetComponent<SmokeEffector>().TurnCount = smokeCloud.PriorityValue;//(actually DOT round)
                GridSystem.instance._gridArray[(int)TilesToDeployGas[i].x, (int)TilesToDeployGas[i].y].GetComponent<SmokeEffector>().GridPosition = TilesToDeployGas[i];
                GridSystem.instance._gridArray[(int)TilesToDeployGas[i].x, (int)TilesToDeployGas[i].y].GetComponent<SmokeEffector>().SmokeObject = OrbSpawner.instance.SpawnSmoke(GridSystem.instance._gridArray[(int)TilesToDeployGas[i].x, (int)TilesToDeployGas[i].y].transform);
            }
        }

    }
    public async UniTask HandleAnimation()
    {
        player.GetComponent<SpawnVFX>().SetOwnVFXPosition(player.GetComponent<VFXSpawnPosition>().CharacterBodyPosition[smokeCloud.CharacterBodyLocation]);
        player.GetComponent<SpawnVFX>().SetVFXPrefab(smokeCloud.PlayerActionVFX);
        player.GetComponent<SpawnVFX>().SetTargetHitVFXPrefab(smokeCloud.TargetHitVFX);
        player.GetComponent<SpawnVFX>().SetParticle(smokeCloud.particle);
        player.GetComponent<SpawnVFX>().SetVFXSound(smokeCloud.actionSound);
        player.GetComponent<SpawnVFX>().SetTargetAnimation(smokeCloud.TargetHurtAnimation);

        await CutsceneManager.instance.PlayAnimationForCharacter(player.gameObject, GetActionName());
    }
    public string GetActionName()
    {
        return smokeCloud.ActionName;
    }

    public int GetPVValue()
    {
        return smokeCloud.PriorityValue;
    }

    public CharacterBaseClasses GetTarget()
    {
        return target;
    }

    public int GetAPValue()
    {
        return smokeCloud.APCost;
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
