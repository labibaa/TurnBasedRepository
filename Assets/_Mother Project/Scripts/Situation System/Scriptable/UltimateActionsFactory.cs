using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.VFX;

public abstract class UltimateActionsFactory : ScriptableObject
{
    public string UltimateName;
    public GameObject ultimateButton;
    public int actionThreshold;
    public int ultimateRange;
    public bool isUltimateSingleTarget;
    public GameObject particlePrefab;
    public VisualEffect PlayerActionVFX;
    public VisualEffect TargetHitVFX;
    public ParticleSystem particle;
    public string CharacterBodyLocation;
    public string TargetCharacterBodyLocation;
    public string TargetHurtAnimation;
    public AudioClip actionSound;
    public abstract IUltimate CreateUltimate();
    public abstract bool IsUltimateEnabled();
}
