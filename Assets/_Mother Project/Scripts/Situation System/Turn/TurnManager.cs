
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.PlayerLoop;
using static Unity.Burst.Intrinsics.Arm;
using static UnityEditor.Experimental.AssetDatabaseExperimental.AssetDatabaseCounters;
using static UnityEditor.Experimental.GraphView.GraphView;
using static UnityEngine.ParticleSystem;
using static UnityEngine.Rendering.DebugUI;
//using static UnityEditorInternal.ReorderableList;

public class TurnManager : MonoBehaviour
{

    public static TurnManager instance;
    [SerializeField] public int currentPlayerIndex = 0;
    public List<PlayerTurn> players;
    public List<PlayerTurn> target;
    public HashSet<PlayerTurn> TargetHashset;

    private TemporaryStats currentPlayer;

    [SerializeField]
    GameType gameMode;//this denotes the mode
    bool firstTurn = true;//this boolean is used to denote if it is the firstTurn of the situatui==ion. This helps to track for 1221 mode.
    int turnCount = 0;
    [SerializeField]
    TMP_Text roundNumber;
    [SerializeField]
    TMP_Text roundText;
    int round = 1;
    //public GameObject ActionUI;
    //public TMP_Text playerTxt;
    public List<CharacterBaseClasses> targetsInRange;
    public List<GameObject> AvailableNonCharacterTargets;
    public List<INonCharacterTarget> nonCharacterTargetsInRange = new List<INonCharacterTarget>();
    [SerializeField] InputHandlerForSaving inputHandlerForSaving;

    public static Turn currentTurn;
    [SerializeField] GridHover _gridHover;
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        TargetHashset = new HashSet<PlayerTurn>(players);

    }
    private void Start()
    {

        //ActionUI.SetActive(false);
        //StartTurn();
        // Initialize the array of players
        //players = FindObjectsOfType<CharacterBaseClasses>();

    }

    private void Update()
    {


        // Check for player input
        //if (Input.GetKeyDown(KeyCode.T))
        //{
        //    // End the player's turn when the spacebar is pressed
        //    EndTurn();

        //}
        if (Input.GetKeyDown(KeyCode.Space))
        {
            //if (currentPlayer.CompareTag("Player"))
            //{
                EndTurn();
            //}
            //else
            //{
            //    EnemyAIAttack();
            //}

        }

    }

    public async void EndTurn()
    {

        /* Stops turn timer

        Clears grid visuals

        Resets target highlights

        Triggers simulation state

        Executes queued actions

        Saves turn data

        Clears UI text*/
        TurnTimer.Instance.StopTImer();
        _gridHover.RestoreColor();
        if (GridSystem.instance.IsGridOn)
        {

            /// Turning of Simulation for Turn Base
            /*
            else
            {
                TempManager.instance.Simulate();
                TempManager.instance.ChangeGameState(GameStates.Simulation);
            }*/



            ResetTargetHIghlightVisual();
            GridMovement.instance.ResetHighlightedPath();
            targetsInRange.Clear();
            nonCharacterTargetsInRange.Clear();
            TempManager.instance.ChangeGameState(GameStates.Simulation);
            await HandleTurnNew.instance.PerformTurns();
            TextFadeInOut.instance.ClearText();
            inputHandlerForSaving.SaveTurnToJson();
   
        }

    }

    public void StartTurn()
    {
        /*Starts turn timer

        Updates round UI

        Activates current player's components

        Sets up UI panels

        Populates target lists

        Updates action UI*/
        
        TurnTimer.Instance.StartTimer();

        TeamManager.instance.PrintDictionary();

        roundNumber.text = "Round: " + round.ToString();
        if (currentPlayerIndex >= players.Count)
        {
            // Increment the current player index

            //ActionUI.SetActive(false);
            // Start the next player's turn
            Debug.Log("not performing turns");
            UI.instance.actionPanel = null; //because kill, survive, deal movelists overwrite fix

            ResetTurn();
        }
        

        UpdateTurnUI();//enabling current Turn Ui


        //   TempManager.instance.attacker = players[currentPlayerIndex].GetComponent<CharacterBaseClasses>().gameObject;
        // Turn On Action UI
        //ActionUI.SetActive(true);
        UI.instance.ResetPanels();
        UI.instance.ClearTargetList();

        //playerTxt.text = players[currentPlayerIndex].gameObject.name + "'s turn";

        // Activate the current player's turn
        //players[currentPlayerIndex].StartTurn();
        TempManager.instance.attacker = players[currentPlayerIndex].gameObject;
        TempManager.instance.ChangeGameState(GameStates.StartTurn);
        currentPlayer = players[currentPlayerIndex].GetComponent<TemporaryStats>();
        CharacterBaseClasses currentPlayerBaseClass = currentPlayer.GetComponent<CharacterBaseClasses>();
        players[currentPlayerIndex].GetComponent<NavMeshAgent>().enabled = true;
        currentPlayer.SelectionParticle.SetActive(true);
        currentPlayer.PlayerActionListPanel.SetActive(true);
        currentPlayer.PlayerUltimateBar.SetActive(true);
        if(currentPlayer.CharacterTeam == TeamName.TeamA)
        {
            currentPlayer.GetComponent<IsoMetricToTPS>().enabled = true;
        }

        TeamManager.instance.TeamMemberList(currentPlayer.CharacterTeam);

        //foreach (PlayerTurn pl in players)
        //{
        //    pl.GetComponent<TemporaryStats>().playerActionListPanel.SetActive(false);
        //}
        //currentPlayer.playerActionListPanel.SetActive(true);

        //players[currentPlayerIndex].GetComponent<NavMeshObstacle>().enabled = false;
        
        ActionActivator.instance.UpdateAvailableAction(currentPlayerBaseClass, currentPlayer);

        UI.instance.GetPlayerStats(players[currentPlayerIndex].GetComponent<CharacterBaseClasses>());

        TargetList();
        
        //if (players[currentPlayerIndex].GetComponent<TemporaryStats>().CharacterTeam == TeamName.TeamD)
        //{
        //    Debug.Log("aw");
        //    AutoRandomAttack();

        //}


    }



    public void PopulateTargetList(string actionName)
    {
        /*Gets action data from DAO

        Finds valid targets in range

        Handles special cases(e.g., Heal)

        Updates target UI buttons

        Manages target particles*/

        ActionStat temporaryScriptable = DAOScriptableObject.instance.GetActionData(StringData.directory, actionName);
        ImprovedActionStat temporaryImprovedScriptable = DAOScriptableObject.instance.GetImprovedActionData(StringData.directory, actionName);


        if (temporaryImprovedScriptable)
        {
            targetsInRange = GridMovement.instance.InAdjacentMatrix(players[currentPlayerIndex].GetComponent<TemporaryStats>().currentPlayerGridPosition, players[currentPlayerIndex].GetComponent<TemporaryStats>().CharacterTeam, temporaryImprovedScriptable.ActionRange * players[currentPlayerIndex].GetComponent<TemporaryStats>().playerVisiblity, Color.red);

            foreach (GameObject target in AvailableNonCharacterTargets)
            {
                if (GridMovement.instance.InAdjacentMatrix(players[currentPlayerIndex].GetComponent<TemporaryStats>().currentPlayerGridPosition, new Vector3(target.transform.position.x, 0f, target.transform.position.z), temporaryImprovedScriptable.ActionRange))
                {
                    nonCharacterTargetsInRange.Add(target.GetComponent<INonCharacterTarget>());
                }
            }




            if (actionName == "Heal" || actionName == "Buff")
            {
                targetsInRange = GridMovement.instance.InAdjacentMatrix(players[currentPlayerIndex].GetComponent<TemporaryStats>().currentPlayerGridPosition, players[currentPlayerIndex].GetComponent<TemporaryStats>().CharacterTeam, 10, Color.white);
                GridMovement.instance.ResetHighlightedPath();
                GridMovement.instance.ResetPathSelection();
                targetsInRange = GridMovement.instance.InAdjacentMatrix(players[currentPlayerIndex].GetComponent<TemporaryStats>().currentPlayerGridPosition, targetsInRange[0].GetComponent<TemporaryStats>().CharacterTeam, temporaryImprovedScriptable.ActionRange, Color.red);
                targetsInRange.Add(players[currentPlayerIndex].GetComponent<CharacterBaseClasses>());
            }
        }
        else
        {
            targetsInRange = GridMovement.instance.InAdjacentMatrix(players[currentPlayerIndex].GetComponent<TemporaryStats>().currentPlayerGridPosition, players[currentPlayerIndex].GetComponent<TemporaryStats>().CharacterTeam, temporaryScriptable.ActionRange, Color.red);
        }

        if (targetsInRange.Contains(players[currentPlayerIndex].GetComponent<CharacterBaseClasses>()))
        {
            targetsInRange.Remove(players[currentPlayerIndex].GetComponent<CharacterBaseClasses>());
        }


        if (targetsInRange.Count <= 0)
        {

            StartCoroutine(NoTargetFound(2f));// this is starting a coroutine, which shows there is no target available. then gives a time delay and rests the range

        }
        else
        {

            for (int i = 0; i < targetsInRange.Count; i++)
            {

                UI.instance.CreateTargetButton(targetsInRange[i].gameObject.GetComponent<CharacterBaseClasses>());
                targetsInRange[i].GetComponent<TemporaryStats>().EnemyTargetSelectionParticle.SetActive(true);


            }
            if (actionName == "DaggerThrow")
            {


                for (int i = 0; i < nonCharacterTargetsInRange.Count; i++)
                {


                    nonCharacterTargetsInRange[i].GetSelectionParticle().GetComponent<Renderer>().material.SetColor("_OutlineColor", Color.yellow);


                }
            }
        }

    }
    public void UltimateTargetList(UltimateActionsFactory ultimateScriptable)
    {
        TempManager.instance.ChangeGameState(GameStates.TargetSelectionTurn);
        targetsInRange = GridMovement.instance.InAdjacentMatrix(players[currentPlayerIndex].GetComponent<TemporaryStats>().currentPlayerGridPosition, players[currentPlayerIndex].GetComponent<TemporaryStats>().CharacterTeam, ultimateScriptable.ultimateRange * players[currentPlayerIndex].GetComponent<TemporaryStats>().playerVisiblity, Color.red);
        for (int i = 0; i < targetsInRange.Count; i++)
        {
            UI.instance.CreateTargetButton(targetsInRange[i].gameObject.GetComponent<CharacterBaseClasses>());
            targetsInRange[i].GetComponent<TemporaryStats>().EnemyTargetSelectionParticle.SetActive(true);
        }
    }
    public void TargetList()
    {
        /*  Clears previous targets

        Rebuilds from players list

        Disables non - active player navigation*/
        target.Clear();
        TargetHashset.Clear();
        int j = 0;

        foreach (PlayerTurn pt in TurnManager.instance.players)
        {
            TargetHashset.Add(pt);
        }

        foreach (PlayerTurn playerTurn in TargetHashset)
        {
            if (playerTurn != players[currentPlayerIndex])
            {
                target.Add(playerTurn);
                playerTurn.GetComponent<NavMeshAgent>().enabled = false;
                //players[i].GetComponent<NavMeshObstacle>().enabled = true;
                j++;
            }
        }
    }

    public void AutoRandomAttack()
    {
        /*        Selects random target

        Picks random action

        Triggers attack resolution

        Ends turn*/
        if (GridSystem.instance.IsGridOn)
        {



            //auto turn add ==> random target selection

            int randomTargetIndex = UnityEngine.Random.Range(0, target.Count);
            TempManager.instance.defender = target[randomTargetIndex].gameObject;
            //Debug.Log(target[randomTargetIndex]);

            DictionaryManager.instance.action = DictionaryManager.instance.GiveRandomAction();
            //Debug.Log (DictionaryManager.instance.action);

            DictionaryManager.instance.action();

            TempManager.instance.ChangeGameState(GameStates.MidTurn);


            EndTurn();
        }
    }

    public Transform FindClosestTarget(List<PlayerTurn> allTargets, CharacterBaseClasses player)
    {
        /*Distance - based target selection

        Returns nearest active enemy*/
        Transform closestTarget = null;
        float shortestDistance = Mathf.Infinity; // Initialize with a large value

        foreach (var target in allTargets)
        {
            float distanceToTarget = Vector3.Distance(player.transform.position, target.transform.position);
            if (distanceToTarget < shortestDistance && target.gameObject.activeInHierarchy && target.GetComponent<TemporaryStats>().CharacterTeam != player.GetComponent<TemporaryStats>().CharacterTeam)
            {
                shortestDistance = distanceToTarget;
                closestTarget = target.transform;
            }
        }

        return closestTarget;
    }


    public void EnemyAIAttack()
    {
         /*Gets target from EnemyAI component

        Selects AI - determined action

        Delays action with Invoke*/
        if (GridSystem.instance.IsGridOn && currentPlayer.tag != "Player")
        {



            //auto turn add ==> random target selection
            //int randomTargetIndex = UnityEngine.Random.Range(0, target.Count);
            TempManager.instance.defender = currentPlayer.GetComponent<EnemyAI>().target;
            //Debug.Log(target[randomTargetIndex]);

            currentPlayer.GetComponent<EnemyAI>().SelectAction();

            //DictionaryManager.instance.action = DictionaryManager.instance.GiveRandomAction();
            //Debug.Log (DictionaryManager.instance.action);

            Invoke("EndEnemyAction", 2f);
        }
    }
    public void EndEnemyAction()
    {
        // DictionaryManager.instance.action();
        currentPlayer.GetComponent<EnemyAI>().EndEnemyAction();

        TempManager.instance.ChangeGameState(GameStates.MidTurn);


        EndTurn();

    }



    void UpdateTurnUI()
    {
        players[currentPlayerIndex].myTurn = true;

        PlayerStatUI.instance.UpdateSummaryHUDUI();
    }

    public async void ResetTurn()
    {
         /*Increments round counter

        Resets player stats/ abilities

        Updates UI elements

        Restarts turn cycle*/
        TurnTimer.Instance.StopTImer();
        round++;
        roundNumber.text = "Round: " + round.ToString();
        roundText.text = roundNumber.text = "Round: " + round.ToString();




       // OrbSpawner.instance.SpawnObject();
        //GetComponent<TimelineManager>().enabled = false;
        currentPlayerIndex = 0;




        for (int i = 0; i < players.Count; i++)
        {
            TemporaryStats playerTempStat = players[i].GetComponent<TemporaryStats>();
            playerTempStat.IsBlockActive = false;
            playerTempStat.IsDodgeActive = false;
            playerTempStat.IsThirdRatePerformanceActive = false;
            playerTempStat.IsCounterActive = false;
            playerTempStat.CurrentDamageMultiplier = players[i].GetComponent<CharacterBaseClasses>().DamageMultiplier;
            if (playerTempStat.CompareTag("Player"))
            {
                playerTempStat.CurrentAP = ActionResolver.instance.APCarryOver(playerTempStat.CurrentAP, 2);
            }
            else
            {
                playerTempStat.CurrentAP = ActionResolver.instance.APCarryOver(playerTempStat.CurrentAP, 6);
            }

            players[i].GetComponent<PlayerTurn>().isMoveOn = true;
            PlayerStatUI.instance.GetPlayerStatSummary(players[i].GetComponent<CharacterBaseClasses>());
            PlayerStatUI.instance.GetPlayerStatDetails(players[i].GetComponent<CharacterBaseClasses>());

        }

        RemoveCue.instance.RemoveAllCues();



        //AssignTurnOrder(gameMode);
        StartTurn();
        await UI.instance.AnimatePanelAsync();
        //StartTurn();


    }

    void AssignTurnOrder(GameType gameType)
    {
        if (gameType == GameType.OneVOne)
        {
            /*Activates current player's UI elements

            Updates HUD display*/

            if (firstTurn)
            {
                firstTurn = false;
                // this is iterating to the next player;
                PlayerTurn tempFirstPlayer = players[0];
                players.RemoveAt(0);
                players.Add(tempFirstPlayer);
                PlayerStatUI.instance.GetPlayerStatSummary(players[0].GetComponent<CharacterBaseClasses>());
                PlayerStatUI.instance.GetPlayerStatDetails(players[0].GetComponent<CharacterBaseClasses>());
            }
            else
            {
                turnCount++;
                if (turnCount > 1)
                {
                    // this is iterating to the next player;
                    PlayerTurn tempFirstPlayer = players[0];
                    players.RemoveAt(0);
                    players.Add(tempFirstPlayer);
                    PlayerStatUI.instance.GetPlayerStatSummary(players[0].GetComponent<CharacterBaseClasses>());
                    PlayerStatUI.instance.GetPlayerStatDetails(players[0].GetComponent<CharacterBaseClasses>());
                    turnCount = 0;
                }

            }



        }

        else if (gameType == GameType.AllOut)
        {
            // this is iterating to the next player;
            PlayerTurn tempFirstPlayer = players[0];
            players.RemoveAt(0);
            players.Add(tempFirstPlayer);
            PlayerStatUI.instance.GetPlayerStatSummary(players[0].GetComponent<CharacterBaseClasses>());
            PlayerStatUI.instance.GetPlayerStatDetails(players[0].GetComponent<CharacterBaseClasses>());
        }
    }


    IEnumerator NoTargetFound(float TimeBetweenDelay)
    {
        TempManager.instance.SituationUIPanel.SetActive(false);
        TempManager.instance.UlimateUIPanel.SetActive(false);
        UI.instance.SendNotification("No target in your range");


        yield return new WaitForSecondsRealtime(TimeBetweenDelay);
        TempManager.instance.ChangeGameState(GameStates.MidTurn);
        GridMovement.instance.ResetHighlightedPath();
        TempManager.instance.SituationUIPanel.SetActive(true);
        TempManager.instance.UlimateUIPanel.SetActive(true);
    }


    public void ResetTargetHIghlightVisual()   // to Reset the visual of target if it is not selected 
    {
        foreach (CharacterBaseClasses targets in targetsInRange)
        {
            targets.GetComponent<TemporaryStats>().EnemyTargetSelectionParticle.SetActive(false);

        }
        if (nonCharacterTargetsInRange.Count > 0)
        {


            foreach (INonCharacterTarget targets in nonCharacterTargetsInRange)
            {
                targets.GetSelectionParticle().GetComponent<Renderer>().material.SetColor("_OutlineColor", Color.black);
            }
        }
    }

}
