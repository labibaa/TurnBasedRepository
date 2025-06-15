using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class HangingTargetFall : ICommand
{
    GameObject hangingObject;
    ImprovedActionStat rangedAttack;


    public HangingTargetFall(GameObject hangingObj, ImprovedActionStat rangedScriptable)
    {
        hangingObject = hangingObj;
        rangedAttack = rangedScriptable;
    }
    public async UniTask Execute()
    {
        await HandleAnimation();
        Debug.Log("00Fall");
        hangingObject.transform.parent= null;
        hangingObject.GetComponent<Rigidbody>().useGravity = true;
        //fall logic
    }

    async UniTask HandleAnimation()
    {
        GameObject player = TurnManager.instance.players[TurnManager.instance.currentPlayerIndex].gameObject;
       
        player.GetComponent<SpawnVFX>().SetOwnVFXPosition(player.GetComponent<VFXSpawnPosition>().CharacterBodyPosition[rangedAttack.CharacterBodyLocation]);
        player.GetComponent<SpawnVFX>().SetTargetVFXPosition(hangingObject.GetComponent<VFXSpawnPosition>().CharacterBodyPosition[rangedAttack.TargetCharacterBodyLocation]);
        player.GetComponent<SpawnVFX>().SetVFXPrefab(rangedAttack.PlayerActionVFX);
        player.GetComponent<SpawnVFX>().SetTargetHitVFXPrefab(rangedAttack.TargetHitVFX);
        player.GetComponent<SpawnVFX>().SetParticle(rangedAttack.particle);
        player.GetComponent<SpawnVFX>().SetVFXSound(rangedAttack.actionSound);
        player.GetComponent<SpawnVFX>().SetTargetAnimation(rangedAttack.TargetHurtAnimation);

        await CutsceneManager.instance.PlayAnimationForCharacter(TurnManager.instance.players[TurnManager.instance.currentPlayerIndex].gameObject, "Dagger Throw");
    }

    public string GetActionName()
    {
        return rangedAttack.ActionName ;
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
        return rangedAttack.APCost;
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
        return null;
    }

   
}
