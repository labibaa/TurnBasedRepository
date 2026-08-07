using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// SquadCoordinator — singleton (same pattern as TurnManager / TempManager / ActionArchive).
///
/// Tracks, per enemy round:
///   • How much damage is already committed to each target so allies don't overkill.
///   • How many attackers are focusing the same target (soft cap prevents pile-ons).
///   • Which ally each support/healer unit has claimed, preventing duplicate heal targeting.
///
/// WIRING REQUIRED:
///   Call  SquadCoordinator.instance.BeginRound()  from TurnManager (or wherever your
///   enemy-turn cycle begins) so state is cleared before the first enemy acts each round.
///
/// Usage in EnemyAIController:
///   • After queuing an offensive action → RegisterAttack(attacker, target, estimatedDmg)
///   • After picking a support target    → TryClaimHealTarget(healer, ally)
///   • TargetScoring reads GetCommittedDamage / GetAttackerCount / IsHealTargetClaimed
///     when scoring candidates.
/// </summary>
public class SquadCoordinator : MonoBehaviour
{
    public static SquadCoordinator instance;

    [Header("Coordination Settings")]
    [Tooltip("Number of attackers allowed to focus one target before a soft score penalty kicks in.")]
    [SerializeField] private int attackerSoftCap = 2;

    // ── Internal state (cleared each round) ──────────────────────────────
    private readonly Dictionary<GameObject, int> _committedDamage  = new Dictionary<GameObject, int>();
    private readonly Dictionary<GameObject, int> _attackerCount    = new Dictionary<GameObject, int>();
    private readonly Dictionary<GameObject, GameObject> _claimedHealTargets = new Dictionary<GameObject, GameObject>(); // healer → ally

    // ── Public read-only properties ──────────────────────────────────────
    public int AttackerSoftCap => attackerSoftCap;

    // ─────────────────────────────────────────────────────────────────────
    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }
        instance = this;
    }

    // ─────────────────────────────────────────────────────────────────────
    // ROUND LIFECYCLE
    // ─────────────────────────────────────────────────────────────────────

    /// <summary>
    /// Call once at the start of each enemy round (before any enemy acts).
    /// Clears all committed-damage, attacker-count, and heal-claim state.
    /// </summary>
    public void BeginRound()
    {
        _committedDamage.Clear();
        _attackerCount.Clear();
        _claimedHealTargets.Clear();
    }

    // ─────────────────────────────────────────────────────────────────────
    // OFFENSIVE COORDINATION
    // ─────────────────────────────────────────────────────────────────────

    /// <summary>
    /// Register that <paramref name="attacker"/> intends to deal
    /// <paramref name="estimatedDamage"/> to <paramref name="target"/> this round.
    /// Call after queuing an offensive action in EnemyAIController.
    /// </summary>
    public void RegisterAttack(GameObject attacker, GameObject target, int estimatedDamage)
    {
        if (target == null) return;

        if (!_committedDamage.ContainsKey(target))
            _committedDamage[target] = 0;
        _committedDamage[target] += estimatedDamage;

        if (!_attackerCount.ContainsKey(target))
            _attackerCount[target] = 0;
        _attackerCount[target]++;
    }

    /// <summary>
    /// Total damage already committed to <paramref name="target"/> by other units this round.
    /// </summary>
    public int GetCommittedDamage(GameObject target)
    {
        return (target != null && _committedDamage.TryGetValue(target, out int dmg)) ? dmg : 0;
    }

    /// <summary>
    /// Number of attackers already assigned to <paramref name="target"/> this round.
    /// </summary>
    public int GetAttackerCount(GameObject target)
    {
        return (target != null && _attackerCount.TryGetValue(target, out int count)) ? count : 0;
    }

    // ─────────────────────────────────────────────────────────────────────
    // SUPPORT / HEAL COORDINATION
    // ─────────────────────────────────────────────────────────────────────

    /// <summary>
    /// Try to claim <paramref name="ally"/> as the heal target for <paramref name="healer"/>.
    /// Returns true if the claim succeeds (ally not yet claimed by a different healer).
    /// Returns false if another healer already claimed this ally — the caller should
    /// still heal them but TargetScoring will penalise the score to encourage variety.
    /// </summary>
    public bool TryClaimHealTarget(GameObject healer, GameObject ally)
    {
        if (ally == null) return false;

        // Allow re-claiming own slot
        if (_claimedHealTargets.TryGetValue(healer, out GameObject current) && current == ally)
            return true;

        // Check if another healer already holds this ally
        foreach (var kv in _claimedHealTargets)
        {
            if (kv.Key != healer && kv.Value == ally)
                return false; // another healer claimed them
        }

        _claimedHealTargets[healer] = ally;
        return true;
    }

    /// <summary>
    /// True if any healer (other than <paramref name="askingHealer"/>) has claimed this ally.
    /// </summary>
    public bool IsHealTargetClaimed(GameObject ally, GameObject askingHealer = null)
    {
        if (ally == null) return false;
        foreach (var kv in _claimedHealTargets)
        {
            if (kv.Value == ally && kv.Key != askingHealer)
                return true;
        }
        return false;
    }

    /// <summary>
    /// Release this healer's claim (e.g. when the unit dies or changes targets).
    /// Called automatically by BeginRound — manual release is optional.
    /// </summary>
    public void ReleaseHealClaim(GameObject healer)
    {
        _claimedHealTargets.Remove(healer);
    }
}
