using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using Cysharp.Threading.Tasks;

public class VenomEffector : MonoBehaviour
{

    public bool HasEffect= false;
    public TemporaryStats EffectOwner;
    public Vector2 GridPosition;
    public int TurnCount;
    public ImprovedActionStat Venom;
    // Start is called before the first frame update
    private void OnEnable()
    {
        HandleTurnNew.OnTurnEnd += DamageCurrentTarget;
    }
    private void OnDisable()
    {
        HandleTurnNew.OnTurnEnd -= DamageCurrentTarget;
    }


    void DamageCurrentTarget()
    {
        ExecuteDamageCurrentTarget();
    }

    async UniTask ExecuteDamageCurrentTarget()
    {
        if (TurnCount > 0)
        {

            GameObject gridObject = GridSystem.instance._gridArray[(int)GridPosition.x, (int)GridPosition.y].GetComponent<GridStat>().OccupiedGameObject;
            if (gridObject != null)
            {
                TemporaryStats targetTempStatsComponent = gridObject.GetComponent<TemporaryStats>();
                if (targetTempStatsComponent != null && targetTempStatsComponent!= EffectOwner && targetTempStatsComponent.CharacterTeam != EffectOwner.CharacterTeam) // have to include a teamcheck if you want your ally's to not get affected
                {
                    int diceValue = DiceNumberGenerator.instance.GetDiceValue(Venom.FirstPercentage, Venom.SecondPercentage, Venom.LastPercentage);
                    UI.instance.SendNotification(diceValue.ToString());
                    int damage = Mathf.RoundToInt(ActionResolver.instance.CalculateNewDamage(diceValue, Venom) * EffectOwner.GetComponent<TemporaryStats>().CurrentDamageMultiplier);

                   /* if (targetTempStatsComponent.IsBlockActive)
                    {
                        damage = damage / 2;
                        targetTempStatsComponent.IsBlockActive = false;
                    }

                    if (targetTempStatsComponent.IsCounterActive)
                    {

                        UI.instance.ShowFlyingText((damage * -1).ToString(), EffectOwner.GetComponent<TemporaryStats>().FlyingTextParent, Color.red);
                        //await HealthManager.instance.PlayerMortality(playerTempStats, attackOrder);
                       // await HealthManager.instance.PlayerMortality(targetTempStats, attackOrder);
                        targetTempStatsComponent.IsCounterActive = false;
                    }

                    else
                    {*/
                        targetTempStatsComponent.CurrentHealth = Mathf.Max(1, HealthManager.instance.HealthCalculation(damage, targetTempStatsComponent.CurrentHealth));
                        CutsceneManager.instance.PlayAnimationForCharacter(targetTempStatsComponent.gameObject, Venom.TargetHurtAnimation);

                        //await HealthManager.instance.PlayerMortality(targetTempStatsComponent, 1);
                        //await HealthManager.instance.PlayerMortality(targetTempStats, attackOrder);
                         UI.instance.ShowFlyingText((damage * -1).ToString(), targetTempStatsComponent.GetComponent<TemporaryStats>().FlyingTextParent, Color.red);

                    // }          

                }

            }

           
            TurnCount--;
        }
        else
        {
           ResetEffectState();
        }

        
    }

    private void ResetEffectState()
    {
        HasEffect = false;
        EffectOwner = null;
        GridPosition = Vector2.zero;
        TurnCount= 0;
        Venom = null;

    }

    
}
