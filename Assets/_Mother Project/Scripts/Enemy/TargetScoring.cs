using UnityEngine;

/// <summary>
/// TargetScoring — static utility that replaces ad-hoc "first/lowest-HP target in range"
/// logic with a unified scoring model.
///
/// Offensive score factors (higher = better target to attack):
///   • Kill potential   — large bonus when this action can finish the target, or when
///                        committed squad damage already pushes them to the brink.
///   • HP% pressure     — prefer softer targets (score rises as HP falls).
///   • Overcommit guard — steep penalty when squad damage already exceeds remaining HP
///                        so allies retarget rather than overkill.
///   • Attacker cap     — escalating penalty for each attacker beyond AttackerSoftCap
///                        to spread pressure across multiple party members.
///   • Threat response  — occasional bonus for the target hoarding the most AP
///                        (same 30 % roll as before, kept as a flat score modifier).
///
/// Support score factors (higher = better ally to heal/buff):
///   • Missing HP %     — the more HP missing, the higher the priority.
///   • Claim penalty    — reduces score if another healer already claimed this ally,
///                        strongly discouraging pile-on healing while a third ally bleeds.
/// </summary>
public static class TargetScoring
{
    // ── Scoring weights ─────────────────────────────────────────────────
    private const float KillBonus               = 200f;  // target can be finished this round
    private const float NearDeathBonus          = 80f;   // target ≤ 20 % max HP
    private const float HPPercentWeight         = 100f;  // (1 - hp%) * this value
    private const float OvercommitPenalty       = 120f;  // committed dmg ≥ remaining HP
    private const float AttackerCapPenalty      = 50f;   // per attacker over soft cap
    private const float ThreatResponseBonus     = 60f;   // banked-AP target (30 % roll)
    private const float HealClaimPenalty        = 80f;   // ally already claimed by another healer

    // ─────────────────────────────────────────────────────────────────────
    // OFFENSIVE SCORING
    // ─────────────────────────────────────────────────────────────────────

    /// <summary>
    /// Score a single offensive candidate.
    /// Higher is better — caller picks the candidate with the highest score.
    /// </summary>
    /// <param name="candidate">The potential target character.</param>
    /// <param name="estimatedDamage">
    ///   Damage this unit's chosen action is expected to deal (computed by
    ///   EnemyAIController.EstimateActionDamage before calling here).
    /// </param>
    /// <param name="rollThreatResponse">
    ///   Pass the result of a (Random.value &lt;= 0.30f) roll so the caller controls
    ///   the randomness; keeps scoring deterministic for a given roll.
    /// </param>
    public static float ScoreOffensiveTarget(
        CharacterBaseClasses candidate,
        int estimatedDamage,
        bool rollThreatResponse = false)
    {
        TemporaryStats ts = candidate.GetComponent<TemporaryStats>();
        if (ts == null) return float.MinValue;

        float score = 0f;
        float maxHP  = Mathf.Max(ts.PlayerHealth, 1f);
        float hpFrac = Mathf.Clamp01(ts.CurrentHealth / maxHP);

        // ── Squad coordination data ──────────────────────────────────────
        int committed    = 0;
        int attackers    = 0;
        int softCap      = 2;

        if (SquadCoordinator.instance != null)
        {
            committed = SquadCoordinator.instance.GetCommittedDamage(candidate.gameObject);
            attackers = SquadCoordinator.instance.GetAttackerCount(candidate.gameObject);
            softCap   = SquadCoordinator.instance.AttackerSoftCap;
        }

        // 1. Kill potential — can this action or committed squad damage finish the target?
        float remainingHP = ts.CurrentHealth - committed;
        bool canKillNow   = ts.CurrentHealth <= estimatedDamage;
        bool committedKill = remainingHP <= estimatedDamage && remainingHP > 0f;
        if (canKillNow || committedKill)
            score += KillBonus;

        // 2. Near-death bonus — target is below 20 % max HP
        if (hpFrac <= 0.20f)
            score += NearDeathBonus;

        // 3. HP% pressure — weaker targets score higher
        score += (1f - hpFrac) * HPPercentWeight;

        // 4. Overcommit guard — squad damage already covers this target's HP
        if (committed >= ts.CurrentHealth)
            score -= OvercommitPenalty;

        // 5. Attacker soft cap — penalise pile-on
        if (attackers >= softCap)
            score -= AttackerCapPenalty * (attackers - softCap + 1);

        // 6. Threat response — 30 % chance to prefer the target hoarding most AP
        //    (caller supplies the roll; pass rollThreatResponse = true when the roll succeeded)
        if (rollThreatResponse)
            score += ThreatResponseBonus;

        return score;
    }

    // ─────────────────────────────────────────────────────────────────────
    // SUPPORT / HEAL SCORING
    // ─────────────────────────────────────────────────────────────────────

    /// <summary>
    /// Score a single ally candidate for support/heal targeting.
    /// Higher is better — caller picks the candidate with the highest score.
    /// </summary>
    /// <param name="candidate">The ally to potentially heal/buff.</param>
    /// <param name="healer">The acting healer/support unit (used for claim lookup).</param>
    public static float ScoreSupportTarget(
        CharacterBaseClasses candidate,
        GameObject healer = null)
    {
        TemporaryStats ts = candidate.GetComponent<TemporaryStats>();
        if (ts == null) return float.MinValue;

        float maxHP          = Mathf.Max(ts.PlayerHealth, 1f);
        float missingHPFrac  = 1f - Mathf.Clamp01(ts.CurrentHealth / maxHP);

        float score = missingHPFrac * 100f;

        // Penalise if another healer already claimed this ally
        if (SquadCoordinator.instance != null &&
            SquadCoordinator.instance.IsHealTargetClaimed(candidate.gameObject, healer))
        {
            score -= HealClaimPenalty;
        }

        return score;
    }
}
