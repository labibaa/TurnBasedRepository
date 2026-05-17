using UnityEngine;

public class ComboXPBonus : MonoBehaviour
{
    [SerializeField] int roundThreshold = 3;

    private const float BonusMultiplier = 0.25f;

    private void OnEnable()
    {
        HealthManager.OnGridDisable += EvaluateComboBonus;
    }

    private void OnDisable()
    {
        HealthManager.OnGridDisable -= EvaluateComboBonus;
    }

    private void EvaluateComboBonus()
    {
        if (TurnManager.instance == null || ExperienceManager.instance == null)
            return;

        if (WaveManager.instance?.currentWave == null)
            return;

        if (TurnManager.instance.round <= roundThreshold)
        {
            int baseExp = WaveManager.instance.currentWave.rewardExp;
            int bonus = Mathf.RoundToInt(baseExp * BonusMultiplier);
            ExperienceManager.instance.AddExperiencePoints(bonus);
            UI.instance.SendNotification($"Speed Bonus! +{bonus} XP (cleared in {TurnManager.instance.round} rounds)");
        }
    }
}
