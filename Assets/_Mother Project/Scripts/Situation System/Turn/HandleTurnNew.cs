using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;



public class HandleTurnNew : MonoBehaviour
{
    public static event Action OnTurnEnd;
    public static event Action OnNewAction;
    public delegate void UndoMechanics(bool isMove);

    public static event UndoMechanics IsPlayerUndo;
   
    public static HandleTurnNew instance;
    [SerializeField]
    InputHandlerForSaving inputHandlerForSaving;



    public List<Turn> allTurnsOfPlayer = new List<Turn>();
    public List<Turn> performedTurns = new List<Turn>();
    public List<Turn> turnsToBePerformed = new List<Turn>();

    public static event Action<Turn> OnActionExecution;

    public bool SituationEndCondition = false;


    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        turnsToBePerformed = allTurnsOfPlayer;
    }

    public List<Turn> GetAllTurns()
    {

        return allTurnsOfPlayer;
    }

    public async UniTask PerformTurns()
    {
        PlayerTurn currentPlayer = TurnManager.instance.players[TurnManager.instance.currentPlayerIndex];
        int totalActionthisTurn = turnsToBePerformed.Count;
        for (int i = 0; i < turnsToBePerformed.Count; i++)
        {
            await PerformTurn(turnsToBePerformed[i]);
        }
       
        if(TurnManager.instance.players.Count > TurnManager.instance.currentPlayerIndex)
        {
            TurnManager.instance.players[TurnManager.instance.players.IndexOf(currentPlayer)].myTurn = false;
        }
       
       // currentPlayer.GetComponent<ShadowOfPlayer>().j = 0;
        PlayerStatUI.instance.UpdateSummaryHUDUI();

        TurnManager.instance.currentPlayerIndex++;// =(TurnManager.instance.currentPlayerIndex + 1) % TurnManager.instance.players.Count; 

        allTurnsOfPlayer.Clear();
        TemporaryStats currentPlayerTemporaryStates = TurnManager.instance.players[TurnManager.instance.players.IndexOf(currentPlayer)].GetComponent<TemporaryStats>();
        TeamManager.instance.IsTeamEmpty(currentPlayerTemporaryStates.CharacterTeam);            // NO EFFECTS!!!
        //currentPlayerTemporaryStates.playerUltimateBarCount= currentPlayerTemporaryStates.playerUltimateBarCount+totalActionthisTurn;

        if (!SituationEndCondition)
        {

            if (currentPlayerTemporaryStates.tag == StringData.PlayerTag)
            {
                currentPlayerTemporaryStates.GetComponent<PlayerTurn>().isMoveOn = true;
                currentPlayerTemporaryStates.CurrentAP = ActionResolver.instance.APCarryOver(currentPlayerTemporaryStates.CurrentAP, 2);
            }

            TurnManager.instance.StartTurn();
            OnTurnEnd?.Invoke();
        }
        else
        {
            SituationEndCondition = false;
            TurnManager.instance.currentPlayerIndex = 0;
        }   
    }

    //public void ProceedToNextTurn()
    //{

    //    TurnManager.instance.players[TurnManager.instance.currentPlayerIndex].myTurn = false;

    //    PlayerStatUI.instance.UpdateSummaryHUDUI();

    //    //TurnManager.instance.currentPlayerIndex++;


    //    //allTurnsOfPlayer.Clear();
    //    TurnManager.instance.StartTurn();
    //}

    async Task PerformTurn(Turn turn)
    {
        
        if (turn.target == null || TurnManager.instance.players.Contains(turn.target.GetComponent<PlayerTurn>()))
        {
            await turn.Command.Execute();
        }
        turn.IsPerformed = true;
        OnActionExecution?.Invoke(turn);

        //turnsToBePerformed.Remove(turn);
        performedTurns.Add(turn);
    }

    public void AddTurn(Turn turn)
    {
        allTurnsOfPlayer.Add(turn);
        if (turn.Command.GetActionType() != "MeleeMove")
        {
            Debug.Log(turn.Command.GetActionType());
            OnNewAction?.Invoke();
            //TextFadeInOut.instance.AddTextToQueue(turn);
        }
        
        PlayerStatUI.instance.UpdateSummaryHUDUI();
        if (turn.Command.GetActionName() != "Move")
        {
            UI.instance.ShowFlyingText((turn.Command.GetAPValue()*-1).ToString(), turn.Player.GetComponent<TemporaryStats>().FlyingTextParent, Color.cyan);
        }
        ActionActivator.instance.UpdateAvailableAction(TurnManager.instance.players[TurnManager.instance.currentPlayerIndex].GetComponent<CharacterBaseClasses>(), TurnManager.instance.players[TurnManager.instance.currentPlayerIndex].GetComponent<TemporaryStats>());

        


        SaveTurnInformation currentTurninfo = new SaveTurnInformation();
        AssignSaveInformation(currentTurninfo, turn);
        inputHandlerForSaving.toSaveData.Add(currentTurninfo);
      

    }


    void AssignSaveInformation(SaveTurnInformation currentTurninfo, Turn turn)
    {
        System.DateTime currentTime = System.DateTime.Now;
        currentTurninfo.DateTime = currentTime.ToString("yyyy-MM-dd HH:mm:ss");
        currentTurninfo.PlayerName = turn.Player.characterName;
        currentTurninfo.PlayerHP = turn.Player.GetComponent<TemporaryStats>().CurrentHealth;
        currentTurninfo.PlayerAp = turn.Player.GetComponent<TemporaryStats>().CurrentAP;
        currentTurninfo.PlayerPosition = turn.Player.GetComponent<TemporaryStats>().currentPlayerGridPosition;
        currentTurninfo.gridPlayerPosition = GridSystem.instance.WorldToGrid(currentTurninfo.PlayerPosition);
        if (turn.target)
        {
            currentTurninfo.TargetName = turn.target.characterName;
            currentTurninfo.TargetHP = turn.target.GetComponent<TemporaryStats>().CurrentHealth;
            currentTurninfo.TargetAp = turn.target.GetComponent<TemporaryStats>().CurrentAP;
            currentTurninfo.TargetPosition = turn.target.GetComponent<TemporaryStats>().currentPlayerGridPosition;
            currentTurninfo.gridTargetPosition = GridSystem.instance.WorldToGrid(currentTurninfo.TargetPosition);
        }
        currentTurninfo.ActionName = turn.Command.GetActionName();
        currentTurninfo.ActionAPCost = turn.Command.GetAPValue();
        
    }



    public void UndoTurn()
    {
        
       
        if (allTurnsOfPlayer.Count>0)
        {
           
            CharacterBaseClasses player = allTurnsOfPlayer[allTurnsOfPlayer.Count - 1].Player;

            player.GetComponent<TemporaryStats>().CurrentAP += allTurnsOfPlayer[allTurnsOfPlayer.Count - 1].Command.GetAPValue();

            if (allTurnsOfPlayer[allTurnsOfPlayer.Count - 1].Command.GetActionType() == "Melee")
            {
                inputHandlerForSaving.toSaveData.RemoveAt(inputHandlerForSaving.toSaveData.Count - 1);

                allTurnsOfPlayer.RemoveAt(allTurnsOfPlayer.Count - 1);
                List<GameObject> undoPaths = allTurnsOfPlayer[allTurnsOfPlayer.Count - 1].Command.GetPaths();
                foreach ( GameObject gm in undoPaths)
                {
                    Debug.Log("CCPATH: " + gm.name);
                }
               
                player.GetComponent<TemporaryStats>().currentPlayerGridPosition = undoPaths[0].transform.position;
                allTurnsOfPlayer.RemoveAt(allTurnsOfPlayer.Count - 1);
                IsPlayerUndo?.Invoke(true);

                inputHandlerForSaving.toSaveData.RemoveAt(inputHandlerForSaving.toSaveData.Count - 1);
            }
            else if (allTurnsOfPlayer[allTurnsOfPlayer.Count - 1].Command.GetActionType() == "SingleMelee")
            {
                inputHandlerForSaving.toSaveData.RemoveAt(inputHandlerForSaving.toSaveData.Count - 1);

                allTurnsOfPlayer.RemoveAt(allTurnsOfPlayer.Count - 1);
                IsPlayerUndo?.Invoke(false);
            }
            else if (allTurnsOfPlayer[allTurnsOfPlayer.Count - 1].Command.GetActionType() == "Move")
            {

                player.GetComponent<PlayerTurn>().isMoveOn = true;
                List<GameObject> undoPaths = allTurnsOfPlayer[allTurnsOfPlayer.Count - 1].Command.GetPaths();
                foreach (GameObject gm in undoPaths)
                {
                    Debug.Log("MOVEATH: " + gm.name);
                }
                player.GetComponent<TemporaryStats>().currentPlayerGridPosition = undoPaths[0].transform.position;
                allTurnsOfPlayer.RemoveAt(allTurnsOfPlayer.Count - 1);
                IsPlayerUndo?.Invoke(false);

                inputHandlerForSaving.toSaveData.RemoveAt(inputHandlerForSaving.toSaveData.Count - 1);
            }
            else
            {
                inputHandlerForSaving.toSaveData.RemoveAt(inputHandlerForSaving.toSaveData.Count - 1);

                allTurnsOfPlayer.RemoveAt(allTurnsOfPlayer.Count - 1);
                IsPlayerUndo?.Invoke(false);
            }
           // IsPlayerUndo?.Invoke(false);
        }
        else
        {
            if (TempManager.instance.currentState!= GameStates.MovementGridSelectionTurn)
            {
                UI.instance.SendNotification("Nothing to undo");
            }
            
        }
        TextFadeInOut.instance.RemoveLatestEntry();
        PlayerStatUI.instance.UpdateSummaryHUDUI();
        ActionActivator.instance.UpdateAvailableAction(TurnManager.instance.players[TurnManager.instance.currentPlayerIndex].GetComponent<CharacterBaseClasses>(), TurnManager.instance.players[TurnManager.instance.currentPlayerIndex].GetComponent<TemporaryStats>());
    }


}




