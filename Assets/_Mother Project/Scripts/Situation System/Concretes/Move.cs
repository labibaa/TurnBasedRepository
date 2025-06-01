using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Move :ICommand
{

    List<GameObject> Path;
    NavMeshAgent Agent;
    bool Automove;
    string ActionType;
    Vector3 SpeedAttributes;


    public Move(List<GameObject> path, NavMeshAgent agent,bool autoMove,string actionType)
    {
        Path = new List<GameObject>();
        Path.AddRange(path);
        Agent = agent;
        Automove =autoMove;
        ActionType = actionType;
        SpeedAttributes = new Vector3(5,360,30);

        foreach (GameObject gm in Path)
        {
           
        }

    }


    public async UniTask Execute()
    {
        Agent.gameObject.GetComponent<TemporaryStats>().AutoMove = Automove;
        Debug.Log($"<color=blue>{Agent.gameObject.name}</color>");
        if (Agent.gameObject.activeInHierarchy)
        {
            await GridMovement.instance.MoveCharacterGrid(Path, Agent, SpeedAttributes, "Move");

            Cursor.lockState = CursorLockMode.None;

            //Agent.Stop();
            //HandleAnimation();
        }

    }

    public async void HandleAnimation()
    {
        Transform closestTarget = TurnManager.instance.FindClosestTarget(TurnManager.instance.target, Agent.GetComponent<CharacterBaseClasses>());
        await TempManager.instance.CharacterRotation(closestTarget.GetComponent<CharacterBaseClasses>(), Agent.GetComponent<CharacterBaseClasses>(), 2f);
    }

    public string GetActionName()
    {
        return "Move";
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
        return 0;
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
