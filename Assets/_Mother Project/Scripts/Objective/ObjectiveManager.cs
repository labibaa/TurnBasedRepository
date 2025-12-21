using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ObjectiveManager : MonoBehaviour
{
    public static ObjectiveManager Instance;

    public List<Objective> Objectives = new List<Objective>();

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    public Objective GetObjective(string objectiveID)
    {
        return Objectives.FirstOrDefault(o => o.ObjectiveID == objectiveID);
    }


    // Call when something happens that MAY affect objective progress or completion
    public void TriggerEvaluation(Objective obj)
    {
            obj.Evaluate();

            // If finished, try activating dependent objectives
            if (obj.State == ObjectiveState.Completed)
                ActivateDependents();
    }

    private void ActivateDependents()
    {
        foreach (Objective obj in Objectives)
        {
            if (obj.State != ObjectiveState.Locked)
                continue;

            bool dependenciesMet = true;

            foreach (var req in obj.RequiredObjectives)
            {
                if (req == null || req.State != ObjectiveState.Completed)
                {
                    dependenciesMet = false;
                    break;
                }
            }

            if (dependenciesMet)
                obj.SetState(ObjectiveState.Active);
        }
    }

}
