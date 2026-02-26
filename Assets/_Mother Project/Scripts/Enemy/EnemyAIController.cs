using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

/// <summary>
/// AP Budget Planning Enemy AI Controller
///
/// CORRECT DECISION ORDER:
///   1. Pick the best action from available pool (most expensive affordable, ranged preferred)
///   2. Find a valid target within THAT action's specific ActionRange
///   3. If no target in range → use free Move to close distance, then retry
///   4. Queue the action with the found target
///   5. Repeat until AP is exhausted
///
/// This ensures range checks are always done against the chosen action's
/// ActionRange rather than guessing range before knowing the action.
/// </summary>
public class EnemyAIController : MonoBehaviour
{
    [Header("AI Behaviour Settings")]
    [Tooltip("HP % (0-1) below which enemy prioritises a defensive action first")]
    [SerializeField] private float lowHPThreshold = 0.4f;

    [Tooltip("Delay between steps for visual readability")]
    [SerializeField] private float stepDelay = 0.6f;

    private TemporaryStats myStats;
    private CharacterBaseClasses myBase;
    private PlayerTurn myPlayerTurn;

    private void Awake()
    {
        myStats = GetComponent<TemporaryStats>();
        myBase = GetComponent<CharacterBaseClasses>();
        myPlayerTurn = GetComponent<PlayerTurn>();
    }

    // ─────────────────────────────────────────────────────────────
    // ENTRY POINT — called by TurnManager.StartTurn()
    // ─────────────────────────────────────────────────────────────
    public async UniTask StartEnemyTurn()
    {
        await UniTask.Delay((int)(stepDelay * 1000));

        TempManager.instance.attacker = gameObject;

        await ExecutePlan();

        await UniTask.Delay((int)(stepDelay * 1000));
        TurnManager.instance.EndTurn();
    }

    // ─────────────────────────────────────────────────────────────
    // MAIN PLAN EXECUTOR
    // ─────────────────────────────────────────────────────────────
    private async UniTask ExecutePlan()
    {
        List<ImprovedActionStat> allActions = myBase.GetAvailableActions();
        List<ImprovedActionStat> offence = ActionArchive.instance.GetOffenseActions(allActions);
        List<ImprovedActionStat> rangedPool = ActionArchive.instance.GetRangedActions(offence);
        List<ImprovedActionStat> meleePool = ActionArchive.instance.GetMeleeActions(offence);
        List<ImprovedActionStat> defencePool = ActionArchive.instance.GetDefenceActions(allActions);

        int simulatedAP = myStats.CurrentAP;
        bool isLowHP = myStats.PlayerHealth > 0
                           && ((float)myStats.CurrentHealth / myStats.PlayerHealth) <= lowHPThreshold;
        bool hasMoved = !myPlayerTurn.isMoveOn; // already moved before this turn if false

        // ── Phase 1: Defensive action when low HP ──────────────────
        if (isLowHP)
        {
            // Defence actions target self — no range check needed
            ImprovedActionStat defence = FindMostExpensiveAffordable(defencePool, simulatedAP);
            if (defence != null)
            {
                // Defender = self for defensive actions
                TempManager.instance.defender = gameObject;
                DictionaryManager.instance.GiveAction(GetActionName(defence));
                simulatedAP -= defence.APCost;
                await UniTask.Delay((int)(stepDelay * 500));
            }
        }

        // ── Phase 2 onwards: Spend remaining AP on offence ─────────
        // Each iteration:
        //   a) Pick best action (ranged first, then melee) by AP cost
        //   b) Find a target within that action's ActionRange
        //   c) If no target in range and Move available → move then retry once
        //   d) Queue the action, subtract AP, repeat

        List<ImprovedActionStat> usedActions = new List<ImprovedActionStat>();

        while (simulatedAP > 0)
        {
            // a) Pick the best affordable action not yet used this turn
            //    Ranged pool is tried first since ranged actions have larger range
            ImprovedActionStat chosenAction = FindMostExpensiveAffordableExcluding(
                                                rangedPool, simulatedAP, usedActions);

            // If no ranged action fits budget, try melee
            if (chosenAction == null)
                chosenAction = FindMostExpensiveAffordableExcluding(
                                   meleePool, simulatedAP, usedActions);

            // Nothing affordable at all — stop spending
            if (chosenAction == null) break;

            // b) Find a target within THIS action's specific range
            CharacterBaseClasses target = FindTargetInRange(chosenAction);

            if (target == null)
            {
                // c) No target in range for this action
                //    If free Move is still available, move toward closest enemy and retry
                if (!hasMoved && myPlayerTurn.isMoveOn)
                {
                    CharacterBaseClasses closestEnemy = ChooseClosestTarget();
                    if (closestEnemy != null)
                    {
                        QueueMoveTowardTarget(closestEnemy, chosenAction.ActionRange);
                        hasMoved = true;
                        myPlayerTurn.isMoveOn = false;
                        await UniTask.Delay((int)(stepDelay * 1000));

                        // Retry range check after moving
                        target = FindTargetInRange(chosenAction);
                    }
                }

                // Still no target after moving — skip this action, try next cheapest
                if (target == null)
                {
                    usedActions.Add(chosenAction); // mark so we don't retry it
                    continue;
                }
            }

            // d) Target found — queue the action
            TempManager.instance.attacker = gameObject;
            TempManager.instance.defender = target.gameObject;
            DictionaryManager.instance.GiveAction(GetActionName(chosenAction));

            usedActions.Add(chosenAction);
            simulatedAP -= chosenAction.APCost;

            await UniTask.Delay((int)(stepDelay * 500));
        }
    }

    // ─────────────────────────────────────────────────────────────
    // FIND TARGET IN RANGE FOR A SPECIFIC ACTION
    // Checks all opposing living players against the chosen action's
    // ActionRange * playerVisiblity — same multiplier used in
    // TurnManager.PopulateTargetList()
    // Returns the lowest-HP target in range, or null if none.
    // ─────────────────────────────────────────────────────────────
    private CharacterBaseClasses FindTargetInRange(ImprovedActionStat action)
    {
        int effectiveRange = action.ActionRange * myStats.playerVisiblity;

        CharacterBaseClasses best = null;
        float lowestHP = Mathf.Infinity;

        foreach (PlayerTurn pt in TurnManager.instance.target)
        {
            if (pt == null || !pt.gameObject.activeInHierarchy) continue;

            TemporaryStats ts = pt.GetComponent<TemporaryStats>();
            if (ts == null) continue;
            if (ts.CharacterTeam == myStats.CharacterTeam) continue;
            if (ts.CurrentHealth <= 0) continue;

            bool inRange = GridMovement.instance.InAdjacentMatrix(
                myStats.currentPlayerGridPosition,
                pt.transform.position,
                effectiveRange
            );

            if (inRange && ts.CurrentHealth < lowestHP)
            {
                lowestHP = ts.CurrentHealth;
                best = pt.GetComponent<CharacterBaseClasses>();
            }
        }

        return best;
    }

    // ─────────────────────────────────────────────────────────────
    // TARGET SELECTION HELPERS
    // ─────────────────────────────────────────────────────────────

    /// Lowest-HP living opponent — used when no range constraint matters
    private CharacterBaseClasses ChooseBestTarget()
    {
        CharacterBaseClasses best = null;
        float lowestHP = Mathf.Infinity;

        foreach (PlayerTurn pt in TurnManager.instance.target)
        {
            if (pt == null || !pt.gameObject.activeInHierarchy) continue;
            TemporaryStats ts = pt.GetComponent<TemporaryStats>();
            if (ts == null || ts.CharacterTeam == myStats.CharacterTeam) continue;
            if (ts.CurrentHealth <= 0) continue;

            if (ts.CurrentHealth < lowestHP)
            {
                lowestHP = ts.CurrentHealth;
                best = pt.GetComponent<CharacterBaseClasses>();
            }
        }

        return best;
    }

    /// Physically closest opponent — used to decide move direction
    private CharacterBaseClasses ChooseClosestTarget()
    {
        Transform closest = TurnManager.instance.FindClosestTarget(
            TurnManager.instance.target,
            myBase
        );
        return closest != null ? closest.GetComponent<CharacterBaseClasses>() : null;
    }

    // ─────────────────────────────────────────────────────────────
    // MOVE TOWARD TARGET (free action, once per turn)
    // Mirrors MeleeMoveTemplate's internal logic:
    //   - Uses AutoGridMovement to find a path
    //   - Tries to reach within desiredRange tiles of the target
    //   - Caps movement at CurrentDex steps
    //   - Queues a Move Turn directly into HandleTurnNew (no AP cost)
    // ─────────────────────────────────────────────────────────────
    private void QueueMoveTowardTarget(CharacterBaseClasses target, int desiredRange)
    {
        Vector2 myGridPos = GridSystem.instance.WorldToGrid(myStats.currentPlayerGridPosition);

        // Find the closest tile adjacent to the target that puts us within desiredRange
        Vector2 destinationGrid = AutoGridMovement.instance.CheckClosestAdjacent(
            myStats.currentPlayerGridPosition,
            target.transform.position
        );

        List<GameObject> fullPath = AutoGridMovement.instance.FindPath(myGridPos, destinationGrid);
        if (fullPath == null || fullPath.Count < 2) return;

        // Cap movement to dexterity — same rule as player Move action
        int maxSteps = myStats.CurrentDex;
        List<GameObject> pathToGo = new List<GameObject>();
        pathToGo.Add(fullPath[0]);
        pathToGo.Add(fullPath[Mathf.Min(maxSteps, fullPath.Count - 1)]);

        // Update logical grid position so subsequent range checks are accurate
        myStats.currentPlayerGridPosition = pathToGo[pathToGo.Count - 1].transform.position;
        myStats.AutoMove = true;

        // Queue Move turn — same pattern as MeleeMoveTemplate, priority 20, no AP cost
        ICommand moveCommand = new Move(pathToGo, GetComponent<NavMeshAgent>(), true, "MeleeMove");
        Turn moveTurn = new Turn(myBase, moveCommand, 20);
        HandleTurnNew.instance.AddTurn(moveTurn);
    }

    // ─────────────────────────────────────────────────────────────
    // ACTION SELECTION HELPERS
    // ─────────────────────────────────────────────────────────────

    /// Most expensive affordable action not already used this turn.
    /// Higher AP cost = more impactful — maximises damage per turn.
    private ImprovedActionStat FindMostExpensiveAffordable(
        List<ImprovedActionStat> pool, int availableAP)
    {
        ImprovedActionStat best = null;
        int bestCost = -1;

        foreach (ImprovedActionStat action in pool)
        {
            if (action == null) continue;
            if (action.APCost > availableAP) continue;
            if (action.APCost > bestCost)
            {
                bestCost = action.APCost;
                best = action;
            }
        }

        return best;
    }

    private ImprovedActionStat FindMostExpensiveAffordableExcluding(
        List<ImprovedActionStat> pool, int availableAP,
        List<ImprovedActionStat> exclude)
    {
        ImprovedActionStat best = null;
        int bestCost = -1;

        foreach (ImprovedActionStat action in pool)
        {
            if (action == null) continue;
            if (exclude.Contains(action)) continue;
            if (action.APCost > availableAP) continue;
            if (action.APCost > bestCost)
            {
                bestCost = action.APCost;
                best = action;
            }
        }

        return best;
    }

    /// Cheapest affordable — used for defensive picks to save AP for attacks.
    private ImprovedActionStat FindCheapestAffordable(
        List<ImprovedActionStat> pool, int availableAP)
    {
        ImprovedActionStat best = null;
        int bestCost = int.MaxValue;

        foreach (ImprovedActionStat action in pool)
        {
            if (action == null) continue;
            if (action.APCost > availableAP) continue;
            if (action.APCost < bestCost)
            {
                bestCost = action.APCost;
                best = action;
            }
        }

        return best;
    }

    /// Resolves the string key for DictionaryManager from the action scriptable.
    private string GetActionName(ImprovedActionStat action)
    {
        return action.actionButton != null ? action.actionButton.name : action.ActionName;
    }
}