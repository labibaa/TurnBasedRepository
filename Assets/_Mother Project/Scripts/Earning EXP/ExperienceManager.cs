using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExperienceManager : MonoBehaviour
{
    public static ExperienceManager instance;

    public delegate void ExperienceChanged(int exp);
    public event ExperienceChanged OnExperienceChanged;

    private int _pendingBroadcastExp;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
    }

    public void AddExperiencePoints(int exp, GameObject earner = null)
    {
        if (earner != null)
        {
            var stats = earner.GetComponent<TemporaryStats>();
            if (stats != null)
            {
                stats.CurrentExp += exp;
            }
            else
            {
                Debug.LogWarning($"[ExperienceManager] Earner '{earner.name}' has no TemporaryStats; {exp} XP dropped.");
            }
            return;
        }

        if (OnExperienceChanged == null)
        {
            _pendingBroadcastExp += exp;
            return;
        }

        int total = exp + _pendingBroadcastExp;
        _pendingBroadcastExp = 0;
        OnExperienceChanged.Invoke(total);
    }

}
