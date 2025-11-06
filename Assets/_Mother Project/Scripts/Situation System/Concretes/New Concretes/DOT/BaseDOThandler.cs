using System;
using UnityEngine;
using Cysharp.Threading.Tasks;

public abstract class BaseDOThandler : MonoBehaviour
{
    public static event Action OnAnyEffectFinished;

    public bool HasEffect { get; protected set; }
    public TemporaryStats EffectOwner { get; protected set; }
    public TemporaryStats Target { get; protected set; }
    public int TurnCount { get; protected set; }
    public GameObject EffectVisualObject { get; protected set; }
    public int AttackOrder { get; protected set; }

    protected virtual void OnEnable()
    {
        HandleTurnNew.OnTurnEnd += ApplyEffect;
    }

    protected virtual void OnDisable()
    {
        HandleTurnNew.OnTurnEnd -= ApplyEffect;
    }

    public virtual void Initialize(TemporaryStats owner, TemporaryStats target, int turnDuration,int attackOrder, GameObject vfxObj)
    {
        HasEffect = true;
        EffectOwner = owner;
        Target = target;
        TurnCount = turnDuration;
        AttackOrder = attackOrder;
        EffectVisualObject = vfxObj;
    }

    private void ApplyEffect()
    {
        HandleEffect().Forget();
    }

    private async UniTask HandleEffect()
    {
        if (TurnCount > 0)
        {
            await OnEffectTick();
            TurnCount--;
        }
        else
        {
            ResetEffect();
        }
    }

    protected virtual void ResetEffect()
    {
        HasEffect = false;
        Target.playerVisiblity = 1;
        Target = null;
        OnAnyEffectFinished?.Invoke();

        if (EffectVisualObject)
            Destroy(EffectVisualObject);

        Destroy(this.gameObject); // Clean up handler after effect ends
    }

    protected abstract UniTask OnEffectTick();
}
