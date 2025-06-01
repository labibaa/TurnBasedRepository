using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Cysharp.Threading.Tasks;
using Unity.VisualScripting;


public class Dash : ICommand
{

    List<GameObject> Path;
    NavMeshAgent Agent;
    bool Automove;
    string ActionType;
    Vector3 SpeedAttributes;
    ActionStat dashScriptable;


    public Dash(List<GameObject> path, NavMeshAgent agent, bool autoMove, string actionType, ActionStat dashActionScriptable)
    {
        Path = new List<GameObject>();
        Path.AddRange(path);
        Agent = agent;
        Automove = autoMove;
        ActionType = actionType;
        dashScriptable = dashActionScriptable;
        SpeedAttributes = new Vector3(50, 1000, 150);
        foreach (GameObject gm in Path)
        {
            Debug.Log($"{"Current path: " + gm.name}");
        }

    }


    public async UniTask Execute()
    {
       /* Agent.GetComponent<SpawnVFX>().SetParticle(dashScriptable.particle);
        Agent.GetComponent<SpawnVFX>().SetTargetVFXPosition(Agent.GetComponent<VFXSpawnPosition>().CharacterBodyPosition[dashScriptable.CharacterBodyLocation]);
        Agent.GetComponent<SpawnVFX>().PlayParticles();*/

        Agent.GetComponent<SpawnVFX>().SetVFXPrefab(dashScriptable.PlayerActionVFX);
        Agent.GetComponent<SpawnVFX>().SetOwnVFXPosition(Agent.GetComponent<VFXSpawnPosition>().CharacterBodyPosition[dashScriptable.CharacterBodyLocation]);
        Agent.GetComponent<SpawnVFX>().PlayerVFX();
        Agent.GetComponent<SpawnVFX>().HitVfx();
        Agent.gameObject.GetComponent<TemporaryStats>().AutoMove = Automove;

       // await CutsceneManager.instance.PlayAnimationForCharacter(Agent.gameObject,"Dash");

        //Agent.gameObject.transform.position = Path[Path.Count-1].transform.position;
        await GridMovement.instance.MoveCharacterGrid(Path, Agent, SpeedAttributes, "Dash");
        Cursor.lockState = CursorLockMode.None;
        HandleAnimation();

        //Agent.Stop();

    }
    public async void HandleAnimation()
    {
        Transform closestTarget = TurnManager.instance.FindClosestTarget(TurnManager.instance.target, Agent.GetComponent<CharacterBaseClasses>());
        await TempManager.instance.CharacterRotation(closestTarget.GetComponent<CharacterBaseClasses>(), Agent.GetComponent<CharacterBaseClasses>(), 2f);

    }

    public string GetActionName()
    {
        return "Dash";
    }

    public int GetPVValue()
    {
        return 0;
    }

    public CharacterBaseClasses GetTarget()
    {
        return null;
    }
    public int GetAPValue()
    {
        return dashScriptable.APCost;
    }

    public NavMeshAgent GetAgent()
    {
        return Agent;
    }
    public List<GameObject> GetPaths()
    {
        return Path;
    }
    public string GetActionType()
    {
        return ActionType;
    }
}


