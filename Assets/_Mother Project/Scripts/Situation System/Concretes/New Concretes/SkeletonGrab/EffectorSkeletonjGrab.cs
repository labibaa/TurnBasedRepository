using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cysharp.Threading.Tasks;
using UnityEngine.VFX;
using System;

public class EffectorSkeletonjGrab : MonoBehaviour
{
    public static EffectorSkeletonjGrab Instance;

    public static event Action OnFinishSkeletonGrab;

    public bool HasEffect = false;
    public TemporaryStats EffectOwner;
    public int TurnCount;
    public ImprovedActionStat SkeletonGrab_IAS;
    public GameObject SkeletonObject;
    public TemporaryStats grabbedTarget;

    // Start is called before the first frame update
    private void OnEnable()
    {
        HandleTurnNew.OnTurnEnd += DamageCurrentTarget;
    }
    private void OnDisable()
    {
        HandleTurnNew.OnTurnEnd -= DamageCurrentTarget;
    }
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }

    }


    void DamageCurrentTarget()
    {
        ExecuteDamageCurrentTarget();
    }

    async UniTask ExecuteDamageCurrentTarget()
    {
        if (TurnCount > 0)
        {

           // GameObject gridObject = GridSystem.instance._gridArray[(int)GridPosition.x, (int)GridPosition.y].GetComponent<GridStat>().OccupiedGameObject;
            if (grabbedTarget != null)
            {
                if (grabbedTarget != null && grabbedTarget != EffectOwner) // have to include a teamcheck if you want your ally's to not get affected
                {
                    grabbedTarget.playerVisiblity = 0;
                    //target.playerVisiblity = 0;
                    //int diceValue = DiceNumberGenerator.instance.GetDiceValue(Smoke.FirstPercentage, Smoke.SecondPercentage, Smoke.LastPercentage);
                    //int damage = Mathf.RoundToInt(ActionResolver.instance.CalculateNewDamage(diceValue, Smoke) * EffectOwner.GetComponent<CharacterBaseClasses>().damageMultiplier);
                    //targetTempStatsComponent.CurrentHealth = HealthManager.instance.HealthCalculation(damage, targetTempStatsComponent.CurrentHealth);
                    //await HealthManager.instance.PlayerMortality(targetTempStatsComponent, 1);
                    grabbedTarget.gameObject.GetComponent<PlayerTurn>().isMoveOn = false;
                    CutsceneManager.instance.PlayAnimationForCharacter(grabbedTarget.gameObject, SkeletonGrab_IAS.TargetHurtAnimation);
                }
            }


            TurnCount--;
        }
        else
        {
            if (HasEffect)
            {
                ResetEffectState();
            }

        }


    }

    private void ResetEffectState()
    {
        //target.playerVisiblity = 1;
        HasEffect = false;
        EffectOwner = null;
        grabbedTarget = null;
        TurnCount = 0;
        SkeletonGrab_IAS = null;
        OnFinishSkeletonGrab?.Invoke();
        Destroy(SkeletonObject);
        foreach (PlayerTurn pturn in TurnManager.instance.players)
        {
            pturn.GetComponent<TemporaryStats>().playerVisiblity = 1;
            pturn.isMoveOn = true;
        }


    }


}

