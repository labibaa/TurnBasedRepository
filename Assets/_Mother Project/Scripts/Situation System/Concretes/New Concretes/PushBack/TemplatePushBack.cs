using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using static TMPro.Examples.ObjectSpin;

public class TemplatePushBack : ICommand
{
    CharacterBaseClasses player;
    CharacterBaseClasses target;
    TemporaryStats playerTempStats;
    TemporaryStats targetTempStats;
    string ActionType;

    public TemplatePushBack(CharacterBaseClasses playerAttacker, CharacterBaseClasses targetDefender, TemporaryStats currentStatPlayer, TemporaryStats currentStatTarget, string actionType, string ActionName)//wip
    {
        player = playerAttacker;
        target = targetDefender;
        playerTempStats = currentStatPlayer;
        targetTempStats = currentStatTarget;
        ActionType = actionType;
    }
    public async UniTask Execute()
    {
        ObjectToBePushed pushScript = target.GetComponent<ObjectToBePushed>();

        Vector2 targetEnemyTile = GridSystem.instance.WorldToGrid(target.transform.position);
        Debug.Log("Position of target: " + targetEnemyTile);
        GameObject targetGridTile = GridSystem.instance._gridArray[(int)(targetEnemyTile.x), (int)(targetEnemyTile.y)];
        //List<Vector2> targetNeigbours = GridMovement.instance.GetAdjacentNeighbors(targetGridPosition, 2);// targetGridTile.GetComponent<GridStat>().neighborCoordinates;
        target.GetComponent<PushDetector>().currentPlayer = player;

        Vector3 directionOfPushBackPosition = (target.transform.position - player.transform.position).normalized;
        Vector3 twoStepDirection = directionOfPushBackPosition * 1.6f * 1f;

        Vector3 approximateTargetPosition = twoStepDirection + target.transform.position;
        Vector2 targetGridPosition = GridSystem.instance.WorldToGrid(approximateTargetPosition);

        Vector3 directionToTarget = target.transform.position - player.transform.position;
        directionToTarget.y = 0; // Zeroing out the y-component to prevent tilting up or down

   
        if (GridMovement.instance.InGridBounds(targetGridPosition))
        {
            Vector3 targetPosition = GridSystem.instance._gridArray[(int)(targetGridPosition.x), (int)(targetGridPosition.y)].transform.position;


            // Set the position of the primitive object to the desired location
            //primitiveObject.transform.position = targetPosition;

            pushScript.Target = targetPosition;
            pushScript.IsShoot = true;
            pushScript.PlayerPushScript = player.GetComponent<ObjectToBePushed>();
            // await HandleAnimation();
            int attackOrder = checkOrder();


                int diceValue = DiceNumberGenerator.instance.GetDiceValue(pushBack.FirstPercentage, pushBack.SecondPercentage, pushBack.LastPercentage);

                int damage = Mathf.RoundToInt(ActionResolver.instance.CalculateNewDamage(diceValue, pushBack) * playerTempStats.CurrentDamageMultiplier);
                Debug.Log("Dice: " + diceValue + " Damage: " + damage);

                await HandleAnimation();
                targetTempStats.CurrentHealth = HealthManager.instance.HealthCalculation(damage, targetTempStats.CurrentHealth);
                UI.instance.ShowFlyingText((damage * -1).ToString(), target.GetComponent<TemporaryStats>().FlyingTextParent, Color.red);
                await HealthManager.instance.PlayerMortality(targetTempStats, attackOrder, playerTempStats);

            
            //play animation

            //await pushScript.WaituntillPushFinished();
        }
        else
        {
            Debug.LogWarning("Calculated push position is out of grid bounds. Trying adjacent positions.");

            List<Vector2> potentialPositions = targetGridTile.GetComponent<GridStat>().neighborCoordinates;

            bool foundValidPosition = false;

            // Check each potential position
            foreach (Vector2 pos in potentialPositions)
            {
                if (GridMovement.instance.InGridBounds(pos))
                {
                    if (CheckTargetPosition(player.transform.position, target.transform.position, GridSystem.instance._gridArray[(int)pos.x, (int)pos.y].transform.position))
                    {
                        GameObject gridTile = GridSystem.instance._gridArray[(int)pos.x, (int)pos.y];
                        Vector3 newPosition = gridTile.transform.position;
                        pushScript.PlayerPushScript = player.GetComponent<ObjectToBePushed>();
                        pushScript.IsShoot = true;
                        pushScript.Target = newPosition;
                        await HandleAnimation();


                        player.GetComponent<ObjectToBePushed>().OwnShoot = false;
                        foundValidPosition = true;
                        //play Animation

                        await pushScript.WaituntillPushFinished();
                        Debug.Log("Valid push target found at position " + pos);
                        break;
                    }
                }
            }
        }
    }

    async UniTask HandleAnimation()
    {
        await TempManager.instance.CharacterRotation(target, player, 2f);

       /* player.GetComponent<SpawnVFX>().SetTargetAnimator(target.gameObject);
        player.GetComponent<SpawnVFX>().SetOwnVFXPosition(player.GetComponent<VFXSpawnPosition>().CharacterBodyPosition[pushBack.CharacterBodyLocation]);
        player.GetComponent<SpawnVFX>().SetTargetVFXPosition(target.GetComponent<VFXSpawnPosition>().CharacterBodyPosition[pushBack.TargetCharacterBodyLocation]);
        player.GetComponent<SpawnVFX>().SetVFXPrefab(pushBack.PlayerActionVFX);
        player.GetComponent<SpawnVFX>().SetTargetHitVFXPrefab(pushBack.TargetHitVFX);
        player.GetComponent<SpawnVFX>().SetParticle(pushBack.particle);
        player.GetComponent<SpawnVFX>().SetVFXSound(pushBack.actionSound);
        player.GetComponent<SpawnVFX>().SetTargetAnimation(pushBack.TargetHurtAnimation);*/

        //CutsceneManager.instance.virtualCamera.Priority = 15;
        await CutsceneManager.instance.PlayAnimationForCharacter(player.gameObject, GetActionName());


    }



    bool CheckTargetPosition(Vector3 A, Vector3 B, Vector3 target)
    {
        Vector3 AB = B - A;
        Vector3 AT = target - A;
        Vector3 BT = target - B;

        if (Vector3.Dot(AT, AB) < 0)
        {
            return false;// Debug.Log("Target is behind A");
        }
        else if (Vector3.Dot(BT, AB) > 0)
        {
            return true;// Debug.Log("Target is behind B");
        }
        else
        {
            return false; //Debug.Log("Target is between A and B");
        }
    }
    public string GetActionName()
    {
        throw new System.NotImplementedException();
    }

    public string GetActionType()
    {
        return null;
    }

    public NavMeshAgent GetAgent()
    {
        return null;
    }

    public int GetAPValue()
    {
        return 0;
    }

    public List<GameObject> GetPaths()
    {
        return null;
    }

    public int GetPVValue()
    {
        return 0;
    }

    public CharacterBaseClasses GetTarget()
    {
        return target;
    }
    int checkOrder()
    {
        return TurnManager.instance.players.IndexOf(target.GetComponent<PlayerTurn>()) - TurnManager.instance.players.IndexOf(player.GetComponent<PlayerTurn>());

    }
}
