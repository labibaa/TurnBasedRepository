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
public enum EnemyAITrait
{
    Aggressive,
    Defensive,
    SaveAP
}

public class EnemyAIController : MonoBehaviour
{
    [Header("AI Behaviour Settings")]
    [Tooltip("HP % (0-1) below which enemy prioritises a defensive action first")]
    [SerializeField] private float lowHPThreshold = 0.4f;

    [Tooltip("Delay between steps for visual readability")]
    [SerializeField] private float stepDelay = 0.6f;

    [Header("AI Trait Settings")]
    [SerializeField] private EnemyAITrait myTrait;
    [SerializeField] private bool assignRandomTraitOnStart = true;

    private TemporaryStats myStats;
    private CharacterBaseClasses myBase;
    private PlayerTurn myPlayerTurn;

    private void Awake()
    {
        myStats = GetComponent<TemporaryStats>();
        myBase = GetComponent<CharacterBaseClasses>();
        myPlayerTurn = GetComponent<PlayerTurn>();
    }

    private void Start()
    {
        if (assignRandomTraitOnStart)
        {
            myTrait = (EnemyAITrait)UnityEngine.Random.Range(0, 3);
            Debug.Log($"[EnemyAIController] {gameObject.name} assigned trait: {myTrait}");
        }
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
        // Blinded / zero-visibility — cannot act or move, end turn immediately
        if (myStats.playerVisiblity == 0)
            return;

        List<ImprovedActionStat> allActions = myBase.GetAvailableActions();
        List<ImprovedActionStat> offence = ActionArchive.instance.GetOffenseActions(allActions);
        List<ImprovedActionStat> rangedPool = ActionArchive.instance.GetRangedActions(offence);
        List<ImprovedActionStat> meleePool = ActionArchive.instance.GetMeleeActions(offence);
        List<ImprovedActionStat> defencePool = ActionArchive.instance.GetDefenceActions(allActions);
        List<ImprovedActionStat> supportPool = ActionArchive.instance.GetSupportActions(allActions);

        int simulatedAP = myStats.CurrentAP;
        bool isLowHP = false;

        if (myTrait == EnemyAITrait.Defensive)
        {
            // Defensive units prioritize survival at a higher health threshold (70% probability per turn once under threshold)
            isLowHP = myStats.PlayerHealth > 0
                           && ((float)myStats.CurrentHealth / myStats.PlayerHealth) <= 0.75f
                           && UnityEngine.Random.value <= 0.70f;
        }
        else if (myTrait == EnemyAITrait.SaveAP)
        {
            isLowHP = myStats.PlayerHealth > 0
                           && ((float)myStats.CurrentHealth / myStats.PlayerHealth) <= lowHPThreshold
                           && UnityEngine.Random.value <= 0.70f;
        }
        // Aggressive units never defend in Phase 1 (isLowHP remains false)

        bool hasMoved = !myPlayerTurn.isMoveOn; // already moved before this turn if false

        // ── Phase 0: Tactical Repositioning (Free move) ──────────────────
        if (!hasMoved && myPlayerTurn.isMoveOn)
        {
            Vector3 targetPosition = Vector3.zero;
            bool shouldReposition = false;

            bool isRanged = rangedPool.Count > 0;
            bool isCasterOrSupport = gameObject.name.Contains("Acolyte") || gameObject.name.Contains("Sorcerer") || supportPool.Count > 0;

            CharacterBaseClasses closestEnemy = ChooseClosestTarget();
            if (closestEnemy != null)
            {
                float distToClosestEnemy = GetGridDistance(myStats.currentPlayerGridPosition, closestEnemy.transform.position);

                if (isRanged && distToClosestEnemy <= 2f)
                {
                    Vector3 bestTile = FindKiteTile(closestEnemy, rangedPool);
                    if (bestTile != Vector3.zero && bestTile != myStats.currentPlayerGridPosition)
                    {
                        targetPosition = bestTile;
                        shouldReposition = true;
                    }
                }
                else if (isCasterOrSupport)
                {
                    Vector3 bestTile = FindSupportPosition(closestEnemy);
                    if (bestTile != Vector3.zero && bestTile != myStats.currentPlayerGridPosition)
                    {
                        targetPosition = bestTile;
                        shouldReposition = true;
                    }
                }
            }

            if (shouldReposition)
            {
                QueueMoveToPosition(targetPosition);
                hasMoved = true;
                myPlayerTurn.isMoveOn = false;
                await UniTask.Delay((int)(stepDelay * 1000));
            }
        }

        // Shared weighted-selection state — used in Phase 1 AND Phase 2+
        // so the same 60/40 weighted selector applies to every action pick.
        List<ImprovedActionStat> usedActions = new List<ImprovedActionStat>();
        List<ImprovedActionStat> strictExclude = new List<ImprovedActionStat>();

        // ── Phase 1: Defensive action when low HP ──────────────────
        if (isLowHP)
        {
            // Uses shared weighted selector: 60% most-expensive, 40% weighted-random cheaper.
            // Defence actions target self — no range check needed.
            ImprovedActionStat defence = ChooseActionFromPool(defencePool, simulatedAP, usedActions, strictExclude);
            if (defence != null)
            {
                TempManager.instance.defender = gameObject;
                DictionaryManager.instance.GiveAction(GetActionName(defence));
                usedActions.Add(defence);
                simulatedAP -= defence.APCost;
                await UniTask.Delay((int)(stepDelay * 500));
            }
        }

        // ── Phase 2 onwards: Spend remaining AP ──────────────────
        // Each iteration:
        //   a) Pick best action based on Trait priority  (weighted 60/40 — shared)
        //   b) Find a target within that action's ActionRange (or self for defensive)
        //   c) If no target in range and Move available → move then retry once
        //   d) Queue the action, subtract AP, repeat

        // For SaveAP trait: find the highest AP cost among available offensive actions (heavy attack cost)
        int heavyAttackCost = 0;
        if (myTrait == EnemyAITrait.SaveAP)
        {
            foreach (ImprovedActionStat action in offence)
            {
                if (action != null && action.APCost > heavyAttackCost)
                {
                    heavyAttackCost = action.APCost;
                }
            }
        }
        int safetyLoopCount = 0;

        while (simulatedAP > 0)
        {
            safetyLoopCount++;
            if (safetyLoopCount > 100)
            {
                Debug.LogWarning($"[EnemyAIController] Infinite loop safety break triggered on {gameObject.name}. AP left: {simulatedAP}");
                break;
            }

            // If SaveAP trait and cannot afford the heavy attack, we save AP for the next turn
            if (myTrait == EnemyAITrait.SaveAP && heavyAttackCost > 0 && simulatedAP < heavyAttackCost)
            {
                int maxAP = ActionResolver.instance != null ? ActionResolver.instance.MaxAP : 10;
                bool hasSpentAP = simulatedAP < myStats.CurrentAP;
                bool belowMaxAP = myStats.CurrentAP < maxAP;

                if (belowMaxAP || hasSpentAP)
                {
                    break; // Save remaining AP and end turn
                }
            }

            // a) Pick the best affordable action not yet used this turn based on trait
            ImprovedActionStat chosenAction = null;

            if (myTrait == EnemyAITrait.Defensive)
            {
                // Defensive: Buff/heal allies -> Self defense -> Ranged -> Melee
                chosenAction = ChooseActionFromPool(supportPool, simulatedAP, usedActions, strictExclude);

                if (chosenAction == null)
                    chosenAction = ChooseActionFromPool(defencePool, simulatedAP, usedActions, strictExclude);

                if (chosenAction == null)
                    chosenAction = ChooseActionFromPool(rangedPool, simulatedAP, usedActions, strictExclude);

                if (chosenAction == null)
                    chosenAction = ChooseActionFromPool(meleePool, simulatedAP, usedActions, strictExclude);
            }
            else if (myTrait == EnemyAITrait.Aggressive)
            {
                // Aggressive: Ranged -> Melee (No support or self-defense in main loop)
                chosenAction = ChooseActionFromPool(rangedPool, simulatedAP, usedActions, strictExclude);

                if (chosenAction == null)
                    chosenAction = ChooseActionFromPool(meleePool, simulatedAP, usedActions, strictExclude);
            }
            else // SaveAP / Standard
            {
                // Normal/SaveAP: Ranged -> Melee -> Support
                chosenAction = ChooseActionFromPool(rangedPool, simulatedAP, usedActions, strictExclude);

                if (chosenAction == null)
                    chosenAction = ChooseActionFromPool(meleePool, simulatedAP, usedActions, strictExclude);

                if (chosenAction == null)
                    chosenAction = ChooseActionFromPool(supportPool, simulatedAP, usedActions, strictExclude);
            }

            // Nothing affordable at all — stop spending
            if (chosenAction == null) break;

            bool isSupportAction = chosenAction.actionStance == ActionStance.Support;
            bool isDefenseAction = chosenAction.actionStance == ActionStance.Defense;

            // b) Find a target within THIS action's specific range
            CharacterBaseClasses target = null;
            if (isDefenseAction)
            {
                target = myBase; // Self targeting, always in range
            }
            else if (isSupportAction)
            {
                target = FindAllyTargetInRange(chosenAction);
            }
            else
            {
                target = FindTargetInRange(chosenAction);
            }

            if (target == null)
            {
                // c) No target in range for offensive actions only — move and retry
                if (!isSupportAction && !isDefenseAction && !hasMoved && myPlayerTurn.isMoveOn)
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
                    strictExclude.Add(chosenAction); // strictly exclude it so it doesn't get picked again in ChooseActionFromPool
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

        List<CharacterBaseClasses> validTargets = new List<CharacterBaseClasses>();

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

            if (inRange)
            {
                validTargets.Add(pt.GetComponent<CharacterBaseClasses>());
            }
        }

        if (validTargets.Count == 0) return null;

        // 1. Kill-Priority Override: If any target is below lethal threshold, prefer finishing them
        List<CharacterBaseClasses> killableTargets = new List<CharacterBaseClasses>();
        foreach (var targetChar in validTargets)
        {
            TemporaryStats ts = targetChar.GetComponent<TemporaryStats>();
            int estDamage = EstimateActionDamage(action, ts);
            bool isLethal = ts.CurrentHealth <= estDamage || ts.CurrentHealth <= (ts.PlayerHealth * 0.20f);
            if (isLethal)
            {
                killableTargets.Add(targetChar);
            }
        }

        if (killableTargets.Count > 0)
        {
            CharacterBaseClasses best = null;
            float lowestHP = Mathf.Infinity;
            foreach (var targetChar in killableTargets)
            {
                float hp = targetChar.GetComponent<TemporaryStats>().CurrentHealth;
                if (hp < lowestHP)
                {
                    lowestHP = hp;
                    best = targetChar;
                }
            }
            return best;
        }

        // 2. Threat Response (Banked AP): 30% chance to target character with most AP
        if (UnityEngine.Random.value <= 0.30f)
        {
            CharacterBaseClasses best = null;
            int maxAP = -1;
            float lowestHP = Mathf.Infinity;

            foreach (var targetChar in validTargets)
            {
                TemporaryStats ts = targetChar.GetComponent<TemporaryStats>();
                if (ts.CurrentAP > maxAP)
                {
                    maxAP = ts.CurrentAP;
                    best = targetChar;
                    lowestHP = ts.CurrentHealth;
                }
                else if (ts.CurrentAP == maxAP)
                {
                    if (ts.CurrentHealth < lowestHP)
                    {
                        lowestHP = ts.CurrentHealth;
                        best = targetChar;
                    }
                }
            }
            if (best != null) return best;
        }

        // 3. Default: Target lowest current HP
        {
            CharacterBaseClasses best = null;
            float lowestHP = Mathf.Infinity;
            foreach (var targetChar in validTargets)
            {
                float hp = targetChar.GetComponent<TemporaryStats>().CurrentHealth;
                if (hp < lowestHP)
                {
                    lowestHP = hp;
                    best = targetChar;
                }
            }
            return best;
        }
    }

    private int EstimateActionDamage(ImprovedActionStat action, TemporaryStats targetStats)
    {
        if (action == null || action.RangeMappings == null || action.RangeMappings.Length == 0)
            return 0;

        float total = 0f;
        foreach (var mapping in action.RangeMappings)
        {
            total += mapping.MappedValue;
        }
        float avgBaseDamage = total / action.RangeMappings.Length;

        float damage = avgBaseDamage * myStats.CurrentDamageMultiplier;

        if (targetStats.IsBlockActive)
        {
            damage /= 2f;
        }

        int finalDamage = ActionResolver.instance.ApplyPhysicalDefense(Mathf.RoundToInt(damage), targetStats.CurrentEndurance);
        return finalDamage;
    }

    private ImprovedActionStat ChooseActionFromPool(
        List<ImprovedActionStat> pool, int availableAP,
        List<ImprovedActionStat> exclude,
        List<ImprovedActionStat> strictExclude)
    {
        List<ImprovedActionStat> affordable = new List<ImprovedActionStat>();
        foreach (ImprovedActionStat action in pool)
        {
            if (action == null) continue;

            if (strictExclude != null && strictExclude.Contains(action))
                continue;

            if (exclude.Contains(action))
            {
                bool isRepeatable = action.APCost <= 2;
                if (!isRepeatable)
                    continue;
            }

            if (action.APCost <= availableAP)
            {
                affordable.Add(action);
            }
        }

        if (affordable.Count == 0)
            return null;

        if (affordable.Count == 1)
            return affordable[0];

        // Sort by APCost descending
        affordable.Sort((a, b) => b.APCost.CompareTo(a.APCost));

        // 60% chance to choose the most expensive affordable one
        if (UnityEngine.Random.value <= 0.60f)
        {
            return affordable[0];
        }
        else
        {
            // 40% chance to choose weighted-random cheaper action
            List<ImprovedActionStat> cheaper = new List<ImprovedActionStat>();
            int maxCost = affordable[0].APCost;
            foreach (var action in affordable)
            {
                if (action.APCost < maxCost)
                {
                    cheaper.Add(action);
                }
            }

            if (cheaper.Count > 0)
            {
                int totalWeight = 0;
                foreach (var action in cheaper)
                {
                    totalWeight += action.APCost;
                }

                if (totalWeight > 0)
                {
                    int roll = UnityEngine.Random.Range(0, totalWeight);
                    int cumulative = 0;
                    foreach (var action in cheaper)
                    {
                        cumulative += action.APCost;
                        if (roll < cumulative)
                        {
                            return action;
                        }
                    }
                }
                return cheaper[UnityEngine.Random.Range(0, cheaper.Count)];
            }
            else
            {
                return affordable[UnityEngine.Random.Range(0, affordable.Count)];
            }
        }
    }

    private Vector3 FindKiteTile(CharacterBaseClasses threat, List<ImprovedActionStat> rangedPool)
    {
        Vector2 myGridPos = GridSystem.instance.WorldToGrid(myStats.currentPlayerGridPosition);
        Vector2 enemyGrid = GridSystem.instance.WorldToGrid(threat.transform.position);

        int maxRange = 3;
        foreach (var action in rangedPool)
        {
            if (action.ActionRange > maxRange)
                maxRange = action.ActionRange;
        }

        float bestScore = CalculateKiteScore(myGridPos, enemyGrid, maxRange);
        Vector3 bestPos = myStats.currentPlayerGridPosition;

        List<Vector2> neighbors = GridMovement.instance.GetAdjacentNeighbors(myGridPos, myStats.CurrentDex);
        foreach (Vector2 coord in neighbors)
        {
            if (!GridMovement.instance.InGridBounds(coord)) continue;

            GameObject gridCell = GridSystem.instance._gridArray[(int)coord.x, (int)coord.y];
            if (gridCell == null) continue;

            GridStat stat = gridCell.GetComponent<GridStat>();
            if (stat == null || !stat.IsWalkable || (stat.IsOccupied && stat.OccupiedGameObject != gameObject))
                continue;

            float score = CalculateKiteScore(coord, enemyGrid, maxRange);

            List<GameObject> path = AutoGridMovement.instance.FindPath(myGridPos, coord);
            if (path == null || path.Count == 0) continue;

            if (score > bestScore)
            {
                bestScore = score;
                bestPos = gridCell.transform.position;
            }
        }

        return bestPos;
    }

    private Vector3 FindSupportPosition(CharacterBaseClasses closestEnemy)
    {
        Vector2 myGridPos = GridSystem.instance.WorldToGrid(myStats.currentPlayerGridPosition);
        Vector2 enemyGrid = GridSystem.instance.WorldToGrid(closestEnemy.transform.position);

        CharacterBaseClasses closestAlly = null;
        float minAllyDist = Mathf.Infinity;

        foreach (PlayerTurn pt in TurnManager.instance.target)
        {
            if (pt == null || !pt.gameObject.activeInHierarchy || pt.gameObject == gameObject) continue;
            TemporaryStats ts = pt.GetComponent<TemporaryStats>();
            if (ts == null || ts.CharacterTeam != myStats.CharacterTeam || ts.CurrentHealth <= 0) continue;

            float dist = GetGridDistance(myStats.currentPlayerGridPosition, pt.transform.position);
            if (dist < minAllyDist)
            {
                minAllyDist = dist;
                closestAlly = pt.GetComponent<CharacterBaseClasses>();
            }
        }

        if (closestAlly == null) return myStats.currentPlayerGridPosition;

        Vector2 allyGrid = GridSystem.instance.WorldToGrid(closestAlly.transform.position);
        float allyDistToEnemy = Vector2.Distance(allyGrid, enemyGrid);

        float bestScore = CalculateSupportScore(myGridPos, enemyGrid, allyGrid, allyDistToEnemy);
        Vector3 bestPos = myStats.currentPlayerGridPosition;

        List<Vector2> neighbors = GridMovement.instance.GetAdjacentNeighbors(myGridPos, myStats.CurrentDex);
        foreach (Vector2 coord in neighbors)
        {
            if (!GridMovement.instance.InGridBounds(coord)) continue;

            GameObject gridCell = GridSystem.instance._gridArray[(int)coord.x, (int)coord.y];
            if (gridCell == null) continue;

            GridStat stat = gridCell.GetComponent<GridStat>();
            if (stat == null || !stat.IsWalkable || (stat.IsOccupied && stat.OccupiedGameObject != gameObject))
                continue;

            float score = CalculateSupportScore(coord, enemyGrid, allyGrid, allyDistToEnemy);

            List<GameObject> path = AutoGridMovement.instance.FindPath(myGridPos, coord);
            if (path == null || path.Count == 0) continue;

            if (score > bestScore)
            {
                bestScore = score;
                bestPos = gridCell.transform.position;
            }
        }

        return bestPos;
    }

    private float CalculateKiteScore(Vector2 tileGrid, Vector2 enemyGrid, int maxRange)
    {
        float distToThreat = Vector2.Distance(tileGrid, enemyGrid);

        if (distToThreat >= 2f && distToThreat <= maxRange)
        {
            return 100f + distToThreat;
        }
        else if (distToThreat > maxRange)
        {
            return 50f - (distToThreat - maxRange);
        }
        else
        {
            return distToThreat;
        }
    }

    private float CalculateSupportScore(Vector2 tileGrid, Vector2 enemyGrid, Vector2 allyGrid, float allyDistToEnemy)
    {
        float distToEnemy = Vector2.Distance(tileGrid, enemyGrid);
        float distToAlly = Vector2.Distance(tileGrid, allyGrid);

        float score = -distToAlly * 2f + distToEnemy * 1f;

        if (distToEnemy > allyDistToEnemy)
        {
            score += 15f;
        }

        if (distToEnemy < 2f)
        {
            score -= 20f;
        }

        return score;
    }

    private void QueueMoveToPosition(Vector3 destinationPos)
    {
        Vector2 myGridPos = GridSystem.instance.WorldToGrid(myStats.currentPlayerGridPosition);
        Vector2 targetGridPos = GridSystem.instance.WorldToGrid(destinationPos);

        List<GameObject> fullPath = AutoGridMovement.instance.FindPath(myGridPos, targetGridPos);
        if (fullPath == null || fullPath.Count < 2) return;

        int maxSteps = myStats.CurrentDex;
        List<GameObject> pathToGo = new List<GameObject>();
        pathToGo.Add(fullPath[0]);
        pathToGo.Add(fullPath[Mathf.Min(maxSteps, fullPath.Count - 1)]);

        myStats.currentPlayerGridPosition = pathToGo[pathToGo.Count - 1].transform.position;
        myStats.AutoMove = true;

        ICommand moveCommand = new Move(pathToGo, GetComponent<NavMeshAgent>(), true, "MeleeMove");
        Turn moveTurn = new Turn(myBase, moveCommand, 20);
        HandleTurnNew.instance.AddTurn(moveTurn);
    }

    private float GetGridDistance(Vector3 posA, Vector3 posB)
    {
        Vector2 gridA = GridSystem.instance.WorldToGrid(posA);
        Vector2 gridB = GridSystem.instance.WorldToGrid(posB);
        return Vector2.Distance(gridA, gridB);
    }

    // ─────────────────────────────────────────────────────────────
    // FIND ALLY TARGET IN RANGE FOR SUPPORT ACTIONS (Buff, SoulTransfer, etc.)
    // Targets the lowest-HP living ally (same team, not self) within range.
    // ─────────────────────────────────────────────────────────────
    private CharacterBaseClasses FindAllyTargetInRange(ImprovedActionStat action)
    {
        int effectiveRange = action.ActionRange * myStats.playerVisiblity;

        CharacterBaseClasses best = null;
        float lowestHP = Mathf.Infinity;

        foreach (PlayerTurn pt in TurnManager.instance.target)
        {
            if (pt == null || !pt.gameObject.activeInHierarchy) continue;
            if (pt.gameObject == gameObject) continue; // self-targeting belongs to Defense stance

            TemporaryStats ts = pt.GetComponent<TemporaryStats>();
            if (ts == null) continue;
            if (ts.CharacterTeam != myStats.CharacterTeam) continue; // allies only
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