using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExperienceManager : MonoBehaviour
{
    public static ExperienceManager instance;

    public delegate void ExperienceChanged(int exp);
    public event ExperienceChanged OnExperienceChanged;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this; 
        }
    }

    public void AddExperiencePoints(int exp)
    {
        OnExperienceChanged?.Invoke(exp);
    }

}
