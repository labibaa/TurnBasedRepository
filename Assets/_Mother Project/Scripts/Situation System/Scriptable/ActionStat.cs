using System;
using UnityEngine;
using UnityEngine.VFX;

[CreateAssetMenu(fileName = "MoveStats", menuName = "ScriptableObjects/MoveStats", order = 1)]
public class ActionStat : ScriptableObject
{
    public Prerequisite prerequiiste;
    public ActionBranch actionBranch;
    public MoveType moveType;
    public MoveType moveType2;
    public MoveType moveType3;
    public MoveType moveType4;

    public Condition conditionType;
    public Mood moodType;
    
    public float condition;
    public int APCost;
    public int CritChance;
    
    public OffenseModifier offenseModifier;
    public DefenseModifier defenseModifier;
    public TargetType targetType;

    public float ActionAccuracy;
    public int BasePower;
    public int PriorityValue; // needs work
    //public float ConditionPercentage;
    public int ActionRange;
    public int PowerPoints;
    public bool selectTarget; //ina
    public bool selectLoadout; //ina

    public string Description;
    public bool HasTargets;
    public AudioClip actionSound;
    public GameObject actionIcon;
    public GameObject actionButton;
    public string moveName;

    public VisualEffect PlayerActionVFX;
    public VisualEffect TargetHitVFX;
    public ParticleSystem particle;
    public string CharacterBodyLocation;
    public string TargetCharacterBodyLocation;
   

    public ActionStance actionStance;
    public ActionType actionType;

}
