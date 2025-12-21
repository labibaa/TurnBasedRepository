using System;
using System.Collections.Generic;

[Serializable]
public class Objective
{
    public string ObjectiveID;
    public string Title;
    public string Description;

    public ObjectiveState State { get; private set; } = ObjectiveState.Locked;

    // Dependencies
    public Objective[] RequiredObjectives ;

    public Func<float> ProgressGetter;        // Optional – returns 0 to 1 value
    public Func<bool> Condition;              // Completion condition

    public event Action<ObjectiveState> OnStateChanged;
    public event Action<float> OnProgressChanged;

    public void SetState(ObjectiveState newState)
    {
        if (State == newState) return;

        State = newState;
        OnStateChanged?.Invoke(newState);
    }

    public void Evaluate()
    {
        if (State != ObjectiveState.Active) return;

        // Update progress if available
        if (ProgressGetter != null)
            OnProgressChanged?.Invoke(ProgressGetter.Invoke());

        // If requirements exist, stop until dependencies complete
        if (RequiredObjectives != null && RequiredObjectives.Length > 0)
        {
            foreach (var req in RequiredObjectives)
                if (req.State != ObjectiveState.Completed)
                    return; // Dependency not ready yet
        }

        // Completion condition
        if (Condition?.Invoke() ?? false)
            SetState(ObjectiveState.Completed);
    }
}

