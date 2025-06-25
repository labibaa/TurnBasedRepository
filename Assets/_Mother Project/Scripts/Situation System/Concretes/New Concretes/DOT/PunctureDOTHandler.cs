using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PunctureDOTHandler : BaseDOThandler
{
    private ImprovedActionStat punctureIAS;

    public void SetPunctureIAS(ImprovedActionStat stat)
    {
        punctureIAS = stat;
    }

    protected override async UniTask OnEffectTick()
    {
        if (Target != null)
        {
            Target.playerVisiblity = 0;
            int diceValue = DiceNumberGenerator.instance.GetDiceValue(punctureIAS.FirstPercentage, punctureIAS.SecondPercentage, punctureIAS.LastPercentage);
            UI.instance.SendNotification(diceValue.ToString());
            int damage = Mathf.RoundToInt(ActionResolver.instance.CalculateNewDamage(diceValue, punctureIAS) * EffectOwner.CurrentDamageMultiplier);
            Debug.Log("Dice: " + diceValue + " Damage: " + damage);
            Target.CurrentHealth = HealthManager.instance.HealthCalculation(damage, Target.CurrentHealth);

            if (punctureIAS != null)
            {
                await CutsceneManager.instance.PlayAnimationForCharacter(
                    Target.gameObject,
                    punctureIAS.TargetHurtAnimation
                );
            }
            UI.instance.ShowFlyingText((damage * -1).ToString(), Target.FlyingTextParent, Color.red);
           // await HealthManager.instance.PlayerMortality(Target, attackOrder, playerTempStats);
        }
    }
}
