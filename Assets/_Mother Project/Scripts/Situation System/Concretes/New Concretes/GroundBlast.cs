using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Cysharp.Threading.Tasks;

public class GroundBlast : ICommand
{
    List<GameObject> Path;
    NavMeshAgent Agent;
    bool Automove;
    string ActionType;
    ActionStat groundBlastScriptable;
    Vector3 SpeedAttributes;

    public GroundBlast(List<GameObject> path, NavMeshAgent agent, bool autoMove, string actionType,ActionStat groundScriptable)
    {
        Path = new List<GameObject>();
        Path.AddRange(path);
        Agent = agent;
        Automove = autoMove; 
        ActionType = actionType;
        groundBlastScriptable = groundScriptable;
        SpeedAttributes = new Vector3(2, 5, 8);

        foreach (GameObject gm in Path)
        {
            Debug.Log($"{"Current path: " + gm.name}");
        }

    }


    public async UniTask Execute()
    {
        Agent.gameObject.GetComponent<TemporaryStats>().AutoMove = Automove;
        await GridMovement.instance.MoveCharacterGrid(Path, Agent, SpeedAttributes, groundBlastScriptable.moveName);
        Cursor.lockState = CursorLockMode.None;
       
        await CutsceneManager.instance.PlayAnimationForCharacter(Agent.gameObject,groundBlastScriptable.moveName);
        List<CharacterBaseClasses> targetsInRange = GridMovement.instance.InAdjacentMatrix(Agent.GetComponent<TemporaryStats>().currentPlayerGridPosition, Agent.GetComponent<TemporaryStats>().CharacterTeam, groundBlastScriptable.ActionRange, Color.red);
        GridMovement.instance.ResetPathSelection();
        GridMovement.instance.ResetHighlightedPath();
        ImprovedActionStat pushBackScriptable = DAOScriptableObject.instance.GetImprovedActionData(StringData.directory, "PushBack");
        foreach (CharacterBaseClasses target in targetsInRange)
        {
            ICommand pushBack = new PushBack(Agent.GetComponent<CharacterBaseClasses>(), target.GetComponent<CharacterBaseClasses>(), Agent.GetComponent<TemporaryStats>(), target.GetComponent<TemporaryStats>(), pushBackScriptable, "SingleMelee");
            await pushBack.Execute();
        }
        //Agent.Stop();

    }

    public string GetActionName()
    {
        return groundBlastScriptable.moveName;
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
        return groundBlastScriptable.APCost ;
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
