using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.AI;

public struct CombatContext
{
    public CharacterBaseClasses attacker;
    public CharacterBaseClasses defender;
    public TemporaryStats attackerStats;
    public TemporaryStats defenderStats;
}

public class ActionArchive : MonoBehaviour
{

    //have to implement all playable actions here

    public static ActionArchive instance;

    [SerializeField] List<GridInput> gridInput = new List<GridInput>();
    [SerializeField] PlayerStatUI playerStateUI;

    [Header("Action Feedback SFX")]
    public AudioClip noTargetSfx;
    public AudioClip actionFailedSfx;

    public bool isTurnAdded = false;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
    }

    private void Start()
    {
        //DictionaryManager.instance.action += GreaterStrike;
        //DictionaryManager.instance.action += CaptivatingPerformance;
    }

    public CombatContext GetCombatContext()
    {
        CombatContext ctx = new CombatContext();
        ctx.attacker = TempManager.instance.attacker.GetComponent<CharacterBaseClasses>();
        ctx.attackerStats = ctx.attacker.GetComponent<TemporaryStats>();
        if (TempManager.instance.defender)
        {
            ctx.defender = TempManager.instance.defender.GetComponent<CharacterBaseClasses>();
            ctx.defenderStats = ctx.defender.GetComponent<TemporaryStats>();
        }
        return ctx;
    }

    // ==========================================
    // DATA LOOKUP HELPERS (CallerMemberName auto-fills action name)
    // ==========================================

    private ImprovedActionStat GetImprovedAction([CallerMemberName] string actionName = "")
        => DAOScriptableObject.instance.GetImprovedActionData(StringData.directory, actionName);

    private ActionStat GetLegacyAction([CallerMemberName] string actionName = "")
        => DAOScriptableObject.instance.GetActionData(StringData.directory, actionName);

    // ==========================================
    // GENERIC ACTION EXECUTORS
    // ==========================================

    public void ExecuteStandardMelee([CallerMemberName] string actionName = "")
    {
        CombatContext ctx = GetCombatContext();
        ImprovedActionStat meleeScriptable = DAOScriptableObject.instance.GetImprovedActionData(StringData.directory, actionName);
        bool isMoveAdded = MeleeMoveTemplate(meleeScriptable, ctx);
        string meleeType = isMoveAdded ? "Melee" : "SingleMelee";
        ICommand meleeAction = new MeleeAttack(ctx.attacker, ctx.defender, ctx.attackerStats, ctx.defenderStats, meleeScriptable, meleeType);
        ActionTemplate(meleeScriptable, meleeAction, ctx);
    }

    public void ExecuteStandardRanged([CallerMemberName] string actionName = "")
    {
        CombatContext ctx = GetCombatContext();
        ImprovedActionStat rangedScriptable = DAOScriptableObject.instance.GetImprovedActionData(StringData.directory, actionName);
        ICommand rangedAction = new RangedAttack(ctx.attacker, ctx.defender, ctx.attackerStats, ctx.defenderStats, rangedScriptable);
        ActionTemplate(rangedScriptable, rangedAction, ctx);
    }

    // ==========================================
    // LEGACY ACTIONS (ActionStat-based)
    // ==========================================

    public void WitchesBolt()
    {
        CombatContext ctx = GetCombatContext();
        ActionStat scriptable = GetLegacyAction();
        ICommand action = new WitchsBolt(ctx.attacker, ctx.defender, ctx.attackerStats, ctx.defenderStats, scriptable);
        ActionTemplate(scriptable, action, ctx);
    }

    public void Choke(CharacterBaseClasses playerAttacker, CharacterBaseClasses targetDefender, TemporaryStats currentStat)
    {
        ActionStat chokeScriptable = Resources.Load<ActionStat>("ActionMoves/Choke"); // will have to change later, and make it a global variable. cz as for now everytime this function is being called, the variable is getting loaded .
        float damageValue = ActionResolver.instance.CalculateKillDamage(playerAttacker, targetDefender, chokeScriptable);
        currentStat.CurrentHealth = HealthManager.instance.HealthCalculation(damageValue, currentStat.CurrentHealth);
        TemporaryStats attackerStats = playerAttacker.gameObject.GetComponent<TemporaryStats>();

        attackerStats.CurrentAP = ActionResolver.instance.APResolver(attackerStats.CurrentAP, chokeScriptable.APCost);

        Debug.Log("Damage :" + damageValue);
    }

    public void Devour()
    {
        CombatContext ctx = GetCombatContext();
        ActionStat scriptable = GetLegacyAction();
        ICommand action = new Devour(ctx.attacker, ctx.defender, ctx.attackerStats, ctx.defenderStats, scriptable);
        ActionTemplate(scriptable, action, ctx);
    }

    public void GreaterStrike()
    {
        CombatContext ctx = GetCombatContext();
        ActionStat scriptable = GetLegacyAction();
        ICommand action = new GreaterStrike(ctx.attacker, ctx.defender, ctx.attackerStats, ctx.defenderStats, scriptable);
        ActionTemplate(scriptable, action, ctx);
    }

    // ==========================================
    // STANDARD MELEE ACTIONS (all use MeleeAttack command)
    // ==========================================

    public void MeleeAttack() => ExecuteStandardMelee();
  /*  public void CosmicCatastrophe() => ExecuteStandardMelee();
    public void RavenousRoast() => ExecuteStandardMelee();
    public void PhantomFury() => ExecuteStandardMelee();
    public void AstralAnnihilation() => ExecuteStandardMelee();*/
    public void SwordSlash() => ExecuteStandardMelee();
    public void SpoonSmack() => ExecuteStandardMelee();
    public void ButcherSmack() => ExecuteStandardMelee();
    public void ButcherSmack1() => ExecuteStandardMelee();
    public void ButcherSmack2() => ExecuteStandardMelee();
    public void HammerGroundAttack() => ExecuteStandardMelee();
    public void TwoHitCombo() => ExecuteStandardMelee();
    public void BossSpinningAttack() => ExecuteStandardMelee();
    public void FrontSlash() => ExecuteStandardMelee();
    public void ThreeHitComboOverhead() => ExecuteStandardMelee();
    public void ThreeHitComboSpinning() => ExecuteStandardMelee();
    public void AxeAttackOverhead() => ExecuteStandardMelee();
    public void DeathWheel() => ExecuteStandardMelee();
    public void FourStabCombo() => ExecuteStandardMelee();
    public void SingleAttack() => ExecuteStandardMelee();
    public void SpearAttackPlace4High() => ExecuteStandardMelee();
    public void SpinningAttack() => ExecuteStandardMelee();
    public void SideAttack() => ExecuteStandardMelee();
    public void Stab() => ExecuteStandardMelee();
    public void Pierce() => ExecuteStandardMelee();
    public void Punch() => ExecuteStandardMelee();

    // ==========================================
    // STANDARD RANGED ACTIONS (all use RangedAttack command)
    // ==========================================

    public void PigAttackOne() => ExecuteStandardRanged();
    public void BoneSpear() => ExecuteStandardRanged();
    public void MirrorMayhem() => ExecuteStandardRanged();
    public void DaggerThrow() => ExecuteStandardRanged();
    public void GrenadeThrow() => ExecuteStandardRanged();
    public void PigThrow() => ExecuteStandardRanged();
    public void CrystalCascade() => ExecuteStandardRanged();
    public void LunarLullaby() => ExecuteStandardRanged();
    public void HexedHavoc() => ExecuteStandardRanged();
    public void CrossbowShoot() => ExecuteStandardRanged();

    // ==========================================
    // CUSTOM MELEE ACTIONS (unique command types)
    // ==========================================

    public void Assassinate()
    {
        CombatContext ctx = GetCombatContext();
        ImprovedActionStat scriptable = GetImprovedAction();
        bool isMoveAdded = MeleeMoveTemplate(scriptable, ctx);
        string meleeType = isMoveAdded ? "Melee" : "SingleMelee";
        ICommand action = new Assassinate(ctx.attacker, ctx.defender, ctx.attackerStats, ctx.defenderStats, scriptable, meleeType);
        ActionTemplate(scriptable, action, ctx);
    }

    public void Puncture()
    {
        CombatContext ctx = GetCombatContext();
        ImprovedActionStat scriptable = GetImprovedAction();
        bool isMoveAdded = MeleeMoveTemplate(scriptable, ctx);
        string meleeType = isMoveAdded ? "Melee" : "SingleMelee";
        ICommand action = new Puncture(ctx.attacker, ctx.defender, ctx.attackerStats, ctx.defenderStats, scriptable, meleeType);
        ActionTemplate(scriptable, action, ctx);
    }

    public void DaggerRising()
    {
        CombatContext ctx = GetCombatContext();
        ImprovedActionStat scriptable = GetImprovedAction();
        bool isMoveAdded = MeleeMoveTemplate(scriptable, ctx);
        string meleeType = isMoveAdded ? "Melee" : "SingleMelee";
        ICommand action = new PushBack(ctx.attacker, ctx.defender, ctx.attackerStats, ctx.defenderStats, scriptable, meleeType);
        ActionTemplate(scriptable, action, ctx);
    }

    public void PushBack()
    {
        CombatContext ctx = GetCombatContext();
        ImprovedActionStat scriptable = GetImprovedAction();
        bool isMoveAdded = MeleeMoveTemplate(scriptable, ctx);
        string meleeType = isMoveAdded ? "Melee" : "SingleMelee";
        ICommand action = new PushBack(ctx.attacker, ctx.defender, ctx.attackerStats, ctx.defenderStats, scriptable, meleeType);
        ActionTemplate(scriptable, action, ctx);
    }

    // ==========================================
    // CUSTOM RANGED / SUPPORT ACTIONS (unique command types)
    // ==========================================

    public void BoneShield()
    {
        CombatContext ctx = GetCombatContext();
        ImprovedActionStat scriptable = GetImprovedAction();
        ICommand action = new BoneShield(ctx.attacker, ctx.defender, ctx.attackerStats, ctx.defenderStats, scriptable);
        ActionTemplate(scriptable, action, ctx);
    }

    public void Imbuement()
    {
        CombatContext ctx = GetCombatContext();
        ImprovedActionStat scriptable = GetImprovedAction();
        ICommand action = new Imbuement(ctx.attacker, ctx.defender, ctx.attackerStats, ctx.defenderStats, scriptable);
        ActionTemplate(scriptable, action, ctx);
    }

    public void SkeletonGrabRoud()
    {
        CombatContext ctx = GetCombatContext();
        ImprovedActionStat scriptable = GetImprovedAction();
        ICommand action = new SkeletonGrab(ctx.attacker, ctx.defender, ctx.attackerStats, ctx.defenderStats, scriptable);
        ActionTemplate(scriptable, action, ctx);
        GridMovement.instance.InAdjacentMatrix(ctx.attackerStats.currentPlayerGridPosition, TeamName.NullTeam, scriptable.ActionRange, Color.red);
    }

    public void SoulSteal()
    {
        CombatContext ctx = GetCombatContext();
        ImprovedActionStat scriptable = GetImprovedAction();
        ICommand action = new SoulSteal(ctx.attacker, ctx.defender, ctx.attackerStats, ctx.defenderStats, scriptable);
        ActionTemplate(scriptable, action, ctx);
    }

    public void MagicSiphon()
    {
        CombatContext ctx = GetCombatContext();
        ImprovedActionStat scriptable = GetImprovedAction();
        ICommand action = new MagicSiphon(ctx.attacker, ctx.defender, ctx.attackerStats, ctx.defenderStats, scriptable);
        ActionTemplate(scriptable, action, ctx);
    }

    public void SoulTransfer()
    {
        CombatContext ctx = GetCombatContext();
        ImprovedActionStat scriptable = GetImprovedAction();
        ICommand action = new SoulTransfer(ctx.attacker, ctx.defender, ctx.attackerStats, ctx.defenderStats, scriptable);
        ActionTemplate(scriptable, action, ctx);
    }

    public void Heal()
    {
        CombatContext ctx = GetCombatContext();
        ImprovedActionStat scriptable = GetImprovedAction();
        ICommand action = new Heal(ctx.attacker, ctx.defender, ctx.attackerStats, ctx.defenderStats, scriptable);
        ActionTemplate(scriptable, action, ctx);
    }

    public void Buff()
    {
        CombatContext ctx = GetCombatContext();
        ImprovedActionStat scriptable = GetImprovedAction();
        ICommand action = new Buff(ctx.attacker, ctx.defender, ctx.attackerStats, ctx.defenderStats, scriptable);
        ActionTemplate(scriptable, action, ctx);
    }

    public void Debuff()
    {
        CombatContext ctx = GetCombatContext();
        ImprovedActionStat scriptable = GetImprovedAction();
        ICommand action = new Debuff(ctx.attacker, ctx.defender, ctx.attackerStats, ctx.defenderStats, scriptable);
        ActionTemplate(scriptable, action, ctx);
    }

    public void Counter()
    {
        CombatContext ctx = GetCombatContext();
        ImprovedActionStat scriptable = GetImprovedAction();
        ICommand action = new Counter(ctx.attacker, ctx.defender, ctx.attackerStats, ctx.defenderStats, scriptable);
        ActionTemplate(scriptable, action, ctx);
    }

    public void Block()
    {
        Debug.Log("block done");
        CombatContext ctx = GetCombatContext();
        ImprovedActionStat scriptable = GetImprovedAction();
        ICommand action = new Block(ctx.attacker, ctx.defender, ctx.attackerStats, ctx.defenderStats, scriptable);
        ActionTemplate(scriptable, action, ctx);
    }

    public void Dodge()
    {
        CombatContext ctx = GetCombatContext();
        ActionStat scriptable = GetLegacyAction();
        ICommand action = new Dodge(ctx.attacker, ctx.defender, ctx.attackerStats, ctx.defenderStats, scriptable);
        ActionTemplate(scriptable, action, ctx);
    }

    // ==========================================
    // AOE ACTIONS (with visual cue logic)
    // ==========================================

    public void VenomCloud()
    {
        CombatContext ctx = GetCombatContext();
        ImprovedActionStat scriptable = GetImprovedAction();
        ICommand action = new VenomCloud(ctx.attacker, ctx.defender, ctx.attackerStats, ctx.defenderStats, scriptable);
        ActionTemplate(scriptable, action, ctx);
        GridMovement.instance.InAdjacentMatrix(ctx.attackerStats.currentPlayerGridPosition, TeamName.NullTeam, scriptable.ActionRange, Color.red);
    }

    public void SmokeCloud()
    {
        CombatContext ctx = GetCombatContext();
        ImprovedActionStat scriptable = GetImprovedAction();
        ICommand action = new SmokeCloud(ctx.attacker, ctx.defender, ctx.attackerStats, ctx.defenderStats, scriptable);
        ActionTemplate(scriptable, action, ctx);
        GridMovement.instance.InAdjacentMatrix(ctx.attackerStats.currentPlayerGridPosition, TeamName.NullTeam, scriptable.ActionRange, Color.red);
    }

    public async void Impale()
    {
        CombatContext ctx = GetCombatContext();
        ImprovedActionStat scriptable = GetImprovedAction();
        List<CharacterBaseClasses> targetsInRange = GridMovement.instance.InAdjacentMatrix(ctx.attackerStats.currentPlayerGridPosition, ctx.attackerStats.CharacterTeam, scriptable.ActionRange, Color.red);
        if (targetsInRange.Count <= 0)
        {
            NoTargetVisual_AOE();
        }
        else
        {
            for (int i = 0; i < targetsInRange.Count; i++)          //visual cue
            {
                targetsInRange[i].GetComponent<TemporaryStats>().EnemyTargetSelectionParticle.SetActive(true);
            }
            Transform ct = TurnManager.instance.FindClosestTarget(TurnManager.instance.target, ctx.attacker);

            ICommand action = new Impale(scriptable, ctx.attackerStats, ctx.attacker, ct.GetComponent<CharacterBaseClasses>());
            ActionTemplate(scriptable, action, ctx);         //visual cue
            await UniTask.Delay(1500);
            for (int i = 0; i < targetsInRange.Count; i++)
            {
                targetsInRange[i].GetComponent<TemporaryStats>().EnemyTargetSelectionParticle.SetActive(false);
            }
            TargetReset_AoE();
        }
    }

    public async void DaggerSweep()
    {
        CombatContext ctx = GetCombatContext();
        ImprovedActionStat scriptable = GetImprovedAction();
        List<CharacterBaseClasses> targetsInRange = GridMovement.instance.InAdjacentMatrix(ctx.attackerStats.currentPlayerGridPosition, ctx.attackerStats.CharacterTeam, scriptable.ActionRange, Color.red);
        if (targetsInRange.Count <= 0)
        {
            NoTargetVisual_AOE();
        }
        else
        {
            for (int i = 0; i < targetsInRange.Count; i++)          //visual cue
            {
                targetsInRange[i].GetComponent<TemporaryStats>().EnemyTargetSelectionParticle.SetActive(true);
            }
            Transform ct = TurnManager.instance.FindClosestTarget(TurnManager.instance.target, ctx.attacker);

            ICommand action = new DaggerSweep(scriptable, ctx.attackerStats, ctx.attacker, ct.GetComponent<CharacterBaseClasses>());
            ActionTemplate(scriptable, action, ctx);         //visual cue
            await UniTask.Delay(1500);
            for (int i = 0; i < targetsInRange.Count; i++)
            {
                targetsInRange[i].GetComponent<TemporaryStats>().EnemyTargetSelectionParticle.SetActive(false);
            }
            TargetReset_AoE();
        }
    }

    // ==========================================
    // MELEE MOVE TEMPLATE
    // ==========================================

    public bool MeleeMoveTemplate(ImprovedActionStat actionScriptable, CombatContext ctx)
    {
        if (ActionResolver.instance.APResolver(ctx.attackerStats.CurrentAP, actionScriptable.APCost) < 0) {
            return false;
        }
        Vector2 playerPosition = GridSystem.instance.WorldToGrid(ctx.attackerStats.currentPlayerGridPosition);
        List<GameObject> path = new List<GameObject>();
        List<GameObject> pathToGo = new List<GameObject>();
        Vector2 targetPosition;

        if (!GridMovement.instance.InAdjacentMatrix(ctx.attackerStats.currentPlayerGridPosition, ctx.defender.transform.position, 1))
        {

            targetPosition = AutoGridMovement.instance.CheckClosestAdjacent(ctx.attackerStats.currentPlayerGridPosition, ctx.defender.transform.position);


            path = AutoGridMovement.instance.FindPath(playerPosition, targetPosition);

            pathToGo.Add(path[0]);
            pathToGo.Add(path[path.Count - 1]);
        }

        if (path.Count > 1)
        {
            ctx.attackerStats.AutoMove = true;
            ctx.attackerStats.currentPlayerGridPosition = path[path.Count - 1].transform.position;

            ICommand MovementConrete = new Move(pathToGo, ctx.attacker.GetComponent<NavMeshAgent>(), true,"MeleeMove");
            //await GameManager.instance.AddCommand(MovementConrete);
            Turn turn = new Turn(ctx.attacker, MovementConrete, 20);
            HandleTurnNew.instance.AddTurn(turn);

            return true;
        }
        else
        {
            return false;
        }
    }

    // ==========================================
    // ACTION TEMPLATES (turn creation & AP validation)
    // ==========================================

    public void ActionTemplate(ActionStat actionScriptable, ICommand tobePerformedAction, CombatContext ctx)
    {
        TemporaryStats playerInfo = ctx.attackerStats;

        if (ActionResolver.instance.APResolver(playerInfo.CurrentAP, actionScriptable.APCost) >= 0)
        {

            playerInfo.CurrentAP = ActionResolver.instance.APResolver(playerInfo.CurrentAP, actionScriptable.APCost);
            int previousPV = 20;
            Turn turn = new Turn(ctx.attacker, tobePerformedAction, previousPV + actionScriptable.PriorityValue);
            int rpOfCurrentPlayer;
            rpOfCurrentPlayer = playerInfo.CurrentResolve;
            if (rpOfCurrentPlayer == 0)
            {
                int randomChanceOfAction = Random.Range(0, 2);
                if (randomChanceOfAction == 0)
                {
                    // notification of rp 0
                    UI.instance.SendNotification("Player Rp is Zero");
                    Debug.Log("rp is zero");
                }
                else
                {
                    HandleTurnNew.instance.AddTurn(turn);
                    Debug.Log("else 1");
                }
            }
            else
            {
                HandleTurnNew.instance.AddTurn(turn);
                Debug.Log("else 2");
            }

        }
        else
        {
            PlayActionFailedSfx();
            UI.instance.SendNotification("No AP left");
        }
    }

    public void ActionTemplate(ImprovedActionStat actionScriptable, ICommand tobePerformedAction, CombatContext ctx)
    {
        TemporaryStats playerInfo = ctx.attackerStats;

        if (ActionResolver.instance.APResolver(playerInfo.CurrentAP, actionScriptable.APCost) >= 0)
        {
            ButtonStackManager.instance.OnButtonPressed(actionScriptable.actionIcon);

            playerInfo.CurrentAP = ActionResolver.instance.APResolver(playerInfo.CurrentAP, actionScriptable.APCost);
            int previousPV =25;

            Turn turn = new Turn(ctx.attacker, tobePerformedAction, previousPV + actionScriptable.PriorityValue);
           // int rpOfCurrentPlayer;//rp not in use ===Date 23.09.24===
           /* rpOfCurrentPlayer = playerInfo.CurrentResolve;
            if (rpOfCurrentPlayer == 0)
            {*/
               /* int randomChanceOfAction = Random.Range(0, 2);
                if (randomChanceOfAction == 0)
                {
                    // notification of rp 0
                    UI.instance.SendNotification("Player Rp is Zero");
                    Debug.Log("rp is zero");
                    isTurnAdded = false;
                }
                else
                {*/
                    HandleTurnNew.instance.AddTurn(turn);
                    Debug.Log("else 1");
                    isTurnAdded = true;

               // }
            //} //rp not in use ===Date 23.09.24===
         /*   else
            {
                HandleTurnNew.instance.AddTurn(turn);
                isTurnAdded  = true;

            }*/

        }
        else
        {
            PlayActionFailedSfx();
            UI.instance.SendNotification("No AP left");
        }
    }

    // ==========================================
    // ULTIMATE
    // ==========================================

    public async void Ultimate()
    {
        CombatContext ctx = GetCombatContext();
        await UltimateSystem._instance.useUltimate(ctx.attacker, ctx.attackerStats, ctx.defender, ctx.defenderStats);
        ctx.attackerStats.playerUltimateBarCount = 0;
        ctx.attackerStats.PlayerUltimateBar.GetComponent<UltimateUI>().ResetUltimateBar();
    }

    // ==========================================
    // MOVEMENT ACTIONS
    // ==========================================

    public async void Move()
    {
        Debug.Log("ASE");
        TurnManager.instance.ResetTargetHIghlightVisual();
        GridMovement.instance.ResetHighlightedPath();
        TurnManager.instance.targetsInRange.Clear();
        TurnManager.instance.nonCharacterTargetsInRange.Clear();
        CombatContext ctx = GetCombatContext();
        if (ctx.attackerStats.gameObject.GetComponent<PlayerTurn>().isMoveOn == true)
        {

            ctx.attackerStats.AutoMove = false;
            ctx.attackerStats.gameObject.GetComponent<PlayerTurn>().isMoveOn = false;
            Cursor.lockState = CursorLockMode.Locked;

            ActionStat moveScriptable = GetLegacyAction();
            ButtonStackManager.instance.OnButtonPressed(moveScriptable.actionIcon);
            foreach(var gridIp in gridInput)
            {
                gridIp.enabled = true;
            }

            GridMovement.instance.InAdjacentMatrix(ctx.attackerStats.currentPlayerGridPosition, TeamName.NullTeam, moveScriptable.ActionRange, Color.green);
            GridMovement.instance.setMoveParam(moveScriptable, moveScriptable.ActionRange, ctx.attackerStats.currentPlayerGridPosition, ctx.attacker.gameObject.GetComponent<NavMeshAgent>());

            TempManager.instance.ChangeGameState(GameStates.MovementGridSelectionTurn);
        }
        else
        {
            PlayActionFailedSfx();
            UI.instance.SendNotification("Can't Move Now");
            TempManager.instance.ChangeGameState(GameStates.MidTurn);
        }
    }

    public async void Dash()
    {
        Debug.Log("Dash");
        TurnManager.instance.ResetTargetHIghlightVisual();
        GridMovement.instance.ResetHighlightedPath();
        TurnManager.instance.targetsInRange.Clear();
        TurnManager.instance.nonCharacterTargetsInRange.Clear();
        CombatContext ctx = GetCombatContext();
        if (ctx.attackerStats.gameObject.GetComponent<PlayerTurn>().isMoveOn == true)
        {
            ActionStat dashScriptable = GetLegacyAction();

            if (ActionResolver.instance.APResolver(ctx.attackerStats.CurrentAP, dashScriptable.APCost) >= 0)
            {

                ctx.attackerStats.AutoMove = false;
                ctx.attackerStats.gameObject.GetComponent<PlayerTurn>().isMoveOn = false;
                Cursor.lockState = CursorLockMode.Locked;

                ctx.attackerStats.CurrentAP = ActionResolver.instance.APResolver(ctx.attackerStats.CurrentAP, dashScriptable.APCost);//ap cost
                ButtonStackManager.instance.OnButtonPressed(dashScriptable.actionIcon);
                foreach (var gridIp in gridInput)
                {
                    gridIp.enabled = true;
                }
                GridMovement.instance.InAdjacentMatrix(ctx.attackerStats.currentPlayerGridPosition, TeamName.NullTeam, dashScriptable.ActionRange, Color.green);
                GridMovement.instance.setMoveParam(dashScriptable, dashScriptable.ActionRange, ctx.attackerStats.currentPlayerGridPosition, ctx.attacker.gameObject.GetComponent<NavMeshAgent>());

                TempManager.instance.ChangeGameState(GameStates.MovementGridSelectionTurn);
            }
            else
            {
                PlayActionFailedSfx();
                UI.instance.SendNotification("No AP!!Can't Dash Now");
            }

        }
        else
        {
            PlayActionFailedSfx();
            UI.instance.SendNotification("Can't Dash Now");
            TempManager.instance.ChangeGameState(GameStates.MidTurn);
        }
    }

    public async void WarpSurge()
    {
        Debug.Log("Ap lagbe");
        TurnManager.instance.ResetTargetHIghlightVisual();
        GridMovement.instance.ResetHighlightedPath();
        TurnManager.instance.targetsInRange.Clear();
        TurnManager.instance.nonCharacterTargetsInRange.Clear();
        CombatContext ctx = GetCombatContext();
        if (ctx.attackerStats.gameObject.GetComponent<PlayerTurn>().isMoveOn == true)
        {

            ctx.attackerStats.AutoMove = false;
            ctx.attackerStats.gameObject.GetComponent<PlayerTurn>().isMoveOn = false;
            Cursor.lockState = CursorLockMode.Locked;

            ActionStat moveScriptable = GetLegacyAction();
            ButtonStackManager.instance.OnButtonPressed(moveScriptable.actionIcon);
            foreach (var gridIp in gridInput)
            {
                gridIp.enabled = true;
            }
            GridMovement.instance.InAdjacentMatrix(ctx.attackerStats.currentPlayerGridPosition, TeamName.NullTeam, moveScriptable.ActionRange, Color.green);
            GridMovement.instance.setMoveParam(moveScriptable, moveScriptable.ActionRange, ctx.attackerStats.currentPlayerGridPosition, ctx.attacker.gameObject.GetComponent<NavMeshAgent>());

            TempManager.instance.ChangeGameState(GameStates.MovementGridSelectionTurn);
        }
        else
        {
            PlayActionFailedSfx();
            UI.instance.SendNotification("Can't Warp Now");
            TempManager.instance.ChangeGameState(GameStates.MidTurn);
        }
    }


    public async void GroundBlast()
    {
        Debug.Log("Grpund Blast in");
        TurnManager.instance.ResetTargetHIghlightVisual();
        GridMovement.instance.ResetHighlightedPath();
        TurnManager.instance.targetsInRange.Clear();
        TurnManager.instance.nonCharacterTargetsInRange.Clear();
        CombatContext ctx = GetCombatContext();
        if (ctx.attackerStats.gameObject.GetComponent<PlayerTurn>().isMoveOn == true)
        {
            Debug.Log("Ground Ap lagbe");
            ctx.attackerStats.AutoMove = false;
            ctx.attackerStats.gameObject.GetComponent<PlayerTurn>().isMoveOn = false;
            Cursor.lockState = CursorLockMode.Locked;

            ActionStat moveScriptable = GetLegacyAction();
            ButtonStackManager.instance.OnButtonPressed(moveScriptable.actionIcon);
            foreach (var gridIp in gridInput)
            {
                gridIp.enabled = true;
            }
            GridMovement.instance.InAdjacentMatrix(ctx.attackerStats.currentPlayerGridPosition, TeamName.NullTeam, ctx.attackerStats.CurrentDex, Color.green);
            GridMovement.instance.setMoveParam(moveScriptable, ctx.attackerStats.CurrentDex, ctx.attackerStats.currentPlayerGridPosition, ctx.attacker.gameObject.GetComponent<NavMeshAgent>());

            TempManager.instance.ChangeGameState(GameStates.MovementGridSelectionTurn);
        }
        else
        {
            PlayActionFailedSfx();
            UI.instance.SendNotification("Can't Warp Now");
            TempManager.instance.ChangeGameState(GameStates.MidTurn);
        }
    }

    // ==========================================
    // UTILITY / HELPER
    // ==========================================

    public void AddTurnToAllTurns()
    {
        HandleTurnNew.instance.AddTurn(TurnManager.currentTurn);
    }

    void SendNotification(string action)
    {
        //actionNotification.StartImageAnimation(action);
    }

    // Plays the shared action-failed SFX (used for No AP / Can't Move / Can't Dash / Can't Warp notifications).
    private void PlayActionFailedSfx()
    {
        if (SoundManager.Instance == null || actionFailedSfx == null) return;
        SoundManager.Instance.PlaySound(actionFailedSfx);
    }

    public List<ImprovedActionStat> GetMeleeActions(List<ImprovedActionStat> actions)
    {
        List<ImprovedActionStat> availableActions = new List<ImprovedActionStat>();
        foreach (ImprovedActionStat action in actions)
        {
            if (action.actionType == ActionType.Melee)
            {
                availableActions.Add(action);
            }
        }
        return availableActions;
    }

    public List<ImprovedActionStat> GetRangedActions(List<ImprovedActionStat> actions)
    {
        List<ImprovedActionStat> availableActions = new List<ImprovedActionStat>();
        foreach (ImprovedActionStat action in actions)
        {
            if (action.actionType == ActionType.Ranged)
            {
                availableActions.Add(action);
            }
        }
        return availableActions;
    }

    public List<ImprovedActionStat> GetDefenceActions(List<ImprovedActionStat> actions)
    {
        List<ImprovedActionStat> availableActions = new List<ImprovedActionStat>();
        foreach (ImprovedActionStat action in actions)
        {
            if (action.actionStance == ActionStance.Defense)
            {
                availableActions.Add(action);
            }
        }
        return availableActions;
    }
    public List<ImprovedActionStat> GetOffenseActions(List<ImprovedActionStat> actions)
    {
        List<ImprovedActionStat> availableActions = new List<ImprovedActionStat>();
        foreach (ImprovedActionStat action in actions)
        {
            if (action.actionStance == ActionStance.Offense)
            {
                availableActions.Add(action);
            }
        }
        return availableActions;
    }

    public List<ImprovedActionStat> GetSupportActions(List<ImprovedActionStat> actions)
    {
        List<ImprovedActionStat> availableActions = new List<ImprovedActionStat>();
        foreach (ImprovedActionStat action in actions)
        {
            if (action.actionStance == ActionStance.Support)
            {
                availableActions.Add(action);
            }
        }
        return availableActions;
    }

    public List<ImprovedActionStat> GetActionsWithinAP(List<ImprovedActionStat> actions, TemporaryStats playerStats)
    {
        List<ImprovedActionStat> availableActions = new List<ImprovedActionStat>();
        foreach (ImprovedActionStat action in actions)
        {
            if (action.APCost <= playerStats.CurrentAP)
            {
                availableActions.Add(action);
            }
        }
        return availableActions;
    }

    public async void NoTargetVisual_AOE()
    {
        TempManager.instance.SituationUIPanel.SetActive(false);
        TempManager.instance.UlimateUIPanel.SetActive(false);

        if (SoundManager.Instance != null && noTargetSfx != null)
        {
            SoundManager.Instance.PlaySound(noTargetSfx);
        }

        UI.instance.SendNotification("No target in your range");


        await UniTask.Delay(1500);
        TempManager.instance.ChangeGameState(GameStates.MidTurn);
        GridMovement.instance.ResetHighlightedPath();
        TempManager.instance.SituationUIPanel.SetActive(true);
        TempManager.instance.UlimateUIPanel.SetActive(true);
    }
    public void TargetReset_AoE()
    {
        GridMovement.instance.ResetHighlightedPath();
        TurnManager.instance.ResetTargetHIghlightVisual();
        TurnManager.instance.targetsInRange.Clear();
        TurnManager.instance.nonCharacterTargetsInRange.Clear();
        TempManager.instance.ChangeGameState(GameStates.MidTurn);
    }
}
