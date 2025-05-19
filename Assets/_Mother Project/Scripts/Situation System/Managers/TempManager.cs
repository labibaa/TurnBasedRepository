using System;
using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using Xamin;

public class TempManager : MonoBehaviour
{
    public string actionName;
    public GameObject attacker;
    
    public GameObject defender;
    
    public List<CharacterBaseClasses> listAdjacency;
    public List<PlayerTurn> targetPlayers;

    public bool IsAdjacent;
    public bool found;
    
    public GameStates currentState;
    public PlayerType PlayerType;

    public int ApCost = 6;

    public static TempManager instance;

    public static event Action<GameStates> GameStateChanged;
    [SerializeField]
    TurnManager turnManager;

    public List<CharacterBaseClasses> players = new List<CharacterBaseClasses>();

    public GameObject SituationUIPanel;
    public GameObject UlimateUIPanel;
    [SerializeField] GameObject SituationUI_MoveList;
    [SerializeField] GameObject actionButton;
    [SerializeField] GameObject PressSpace;
    [SerializeField] float rotationSpeed = 0.2f;


    private void Awake()
    {
        if(instance == null)
        {
            instance = this;
        }
    }
    
    void Start()
    {
        DelayForTime();
        listAdjacency = new List<CharacterBaseClasses>();
        foreach(PlayerTurn player in TurnManager.instance.TargetHashset)
        {
            players.Add(player.GetComponent<CharacterBaseClasses>());
        }

       
       
    }

    // Update is called once per frame
    private void Update()
    {
     /*   if (GridSystem.instance.IsGridOn && attacker.tag != "Player")
        {
            SituationUI_MoveList.SetActive(false);
            actionButton.SetActive(false);
            PressSpace.SetActive(true);
        }
        if (GridSystem.instance.IsGridOn && attacker.tag == "Player")
        {
            SituationUI_MoveList.SetActive(true);
            actionButton.SetActive(true);
            PressSpace.SetActive(false);
        }*/
        if(GridSystem.instance.IsGridOn && currentState != GameStates.Simulation && !UltimateSystem._instance.IsUltimate)
        {
            RotateCharactersOnGrid();
        }
       
    }

    public void RotateCharactersOnGrid()
    {
        Transform closestTarget = TurnManager.instance.FindClosestTarget(TurnManager.instance.target, attacker.GetComponent<CharacterBaseClasses>());
        Vector3 directionToTarget = closestTarget.position - attacker.transform.position;
        directionToTarget.y = 0; // Zeroing out the y-component to prevent tilting up or down

        Quaternion targetRotation = Quaternion.LookRotation(-directionToTarget);
        Vector3 eulerRotation = targetRotation.eulerAngles;
        eulerRotation.x = 0; // Locking rotation around x-axis
        eulerRotation.z = 0; // Locking rotation around z-axis
        targetRotation = Quaternion.Euler(eulerRotation);

        //closestTarget.transform.rotation = targetRotation;
        closestTarget.transform.rotation = Quaternion.Lerp(closestTarget.transform.rotation, targetRotation, Time.deltaTime * rotationSpeed);

        // Rotate the player
        Quaternion playerRotation = Quaternion.LookRotation(closestTarget.position - attacker.transform.position, Vector3.up);
        Vector3 playerEulerRotation = playerRotation.eulerAngles;
        playerEulerRotation.x = 0; // Locking rotation around x-axis
        playerEulerRotation.z = 0; // Locking rotation around z-axis
        playerRotation = Quaternion.Euler(playerEulerRotation);

        //attacker.transform.rotation = playerRotation;
        attacker.transform.rotation = Quaternion.Lerp(attacker.transform.rotation, playerRotation, Time.deltaTime * rotationSpeed) ;
    }

    public async UniTask CharacterRotation(CharacterBaseClasses target, CharacterBaseClasses player, float speed)
    {
        Vector3 directionToTarget = target.transform.position - player.transform.position;
        directionToTarget.y = 0; // Zeroing out the y-component to prevent tilting up or down

        Quaternion targetRotation = Quaternion.LookRotation(-directionToTarget);
        Vector3 eulerRotation = targetRotation.eulerAngles;
        eulerRotation.x = 0; // Locking rotation around x-axis
        eulerRotation.z = 0; // Locking rotation around z-axis
        targetRotation = Quaternion.Euler(eulerRotation);

        // Rotate the player
        Quaternion playerRotation = Quaternion.LookRotation(target.transform.position - player.transform.position, Vector3.up);
        Vector3 playerEulerRotation = playerRotation.eulerAngles;
        playerEulerRotation.x = 0; // Locking rotation around x-axis
        playerEulerRotation.z = 0; // Locking rotation around z-axis
        playerRotation = Quaternion.Euler(playerEulerRotation);

        float elapsedTime = 0;
        while (elapsedTime < speed)
        {
            elapsedTime += Time.deltaTime;
            float t = elapsedTime / speed;

            target.transform.rotation = Quaternion.Lerp(target.transform.rotation, targetRotation, t);
            player.transform.rotation = Quaternion.Lerp(player.transform.rotation, playerRotation, t);
            //TempManager.instance.RotateCharactersOnGrid();
            //await UniTask.Yield();

        }

    }

    public void OnGameState(GameStates newStates)
    {
        currentState = newStates;

        switch(newStates)
        {
            case GameStates.StartTurn:
                SelectPlayer();
                break;
            case GameStates.MidTurn:
                TurnMid();
                break;
            case GameStates.FinishTurn:
                TurnFinished();
                break;
            case GameStates.EnemyTurn:
                StartEnemyTurn();
                break;
            case GameStates.Simulation:
                StartSimulation();
                break;

        }


        GameStateChanged?.Invoke(newStates); 


    }

    private void StartSimulation()
    {
        Debug.Log("simulation");
    }

    private void StartEnemyTurn()
    {
        OnGameState(GameStates.Simulation);
    }

    private void TurnFinished()
    {
        OnGameState(GameStates.StartTurn);
    }

    private void TurnMid()
    {
        if(ApCost >= 6)
        {
            OnGameState(GameStates.FinishTurn);
        }
        
    }

    private void TurnStarted()
    {
       
    }

    private void SelectPlayer()
    {
       // PlayerArrayList.Iterate();
    }

    

    // Start is called before the first frame update
    

    public void PopulateList()
    {
        ActionArchive.instance.GreaterStrike();
    }

    public void ShowTargetList(string action)
    {
        TurnManager.instance.ResetTargetHIghlightVisual();
        GridMovement.instance.ResetHighlightedPath();
        TurnManager.instance.targetsInRange.Clear();
        TurnManager.instance.nonCharacterTargetsInRange.Clear();
       // UI.instance.ClearTargetList();
        actionName = action;
        
        TempManager.instance.ChangeGameState(GameStates.TargetSelectionTurn);
        turnManager.PopulateTargetList(actionName);
        


    }
    public void PerformTargetFreeActions(string action)
    {
        Debug.Log("kill btnj");
        actionName = action;
         DictionaryManager.instance.GiveAction(TempManager.instance.actionName);
        //DictionaryManager.instance.GiveAction(TempManager.instance.actionName).Invoke();
        
        

        TempManager.instance.ChangeGameState(GameStates.MidTurn);
    }

    public void GreaterStrikeUIButton()
    {
        
        ActionArchive.instance.GreaterStrike();
    }


    public void ChangeGameState(GameStates whichAState)
    {
        
        currentState = whichAState;
        if (currentState == GameStates.StartTurn)
        {
           
                UI.instance.ResetPanels();
                //UI.instance.ShowPanel(UI.instance.ksdActionParent);
                SituationUIPanel.SetActive(true);
                UlimateUIPanel.SetActive(true);



        }
        else if (currentState == GameStates.MidTurn)
        {
            UI.instance.ResetPanels();
            //ShowPanel(ksdActionParent);
           // UI.instance.ShowPanel(UI.instance.actionPanel);
            //UI.instance._circleSelector.Open();
            SituationUIPanel.SetActive(true);
            UlimateUIPanel.SetActive(true);
        }
        else if (currentState == GameStates.GhostPlay)
        {
            UI.instance.ResetPanels();
            //ShowPanel(ksdActionParent);
            //UI.instance.ShowPanel(UI.instance.actionPanel);
            //UI.instance._circleSelector.Open();
            SituationUIPanel.SetActive(false);
            UlimateUIPanel.SetActive(false);
        }
        else if (currentState == GameStates.TargetSelectionTurn)
        {
            UI.instance.ResetPanels();
           // UI.instance.ShowPanel(UI.instance.targetListpanel);
            //UI.instance._circleSelector.Close();
            SituationUIPanel.SetActive(true);
            UlimateUIPanel.SetActive(true);
        }
        else if (currentState == GameStates.MovementGridSelectionTurn)
        {
            //UI.instance._circleSelector.Close();
            SituationUIPanel.SetActive(false);
            UlimateUIPanel.SetActive(false);
            UI.instance.ResetPanels();
        }
        else if (currentState == GameStates.Simulation)
        {
            UI.instance.HideAllPanel();
           // UI.instance.ShowPanel(UI.instance.timeLinePanel);
            SituationUIPanel.SetActive(false);
            UlimateUIPanel.SetActive(false);
            TurnManager.instance.players[TurnManager.instance.currentPlayerIndex].GetComponent<TemporaryStats>().SelectionParticle.SetActive(false);
            TurnManager.instance.players[TurnManager.instance.currentPlayerIndex].GetComponent<TemporaryStats>().PlayerActionListPanel.SetActive(false);
            TurnManager.instance.players[TurnManager.instance.currentPlayerIndex].GetComponent<TemporaryStats>().PlayerUltimateBar.SetActive(false);
            //UI.instance._circleSelector.Close();
        }
    }




    IEnumerator DelayForTime()
    {
        yield return new WaitForSeconds(2f);
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }


    public void TestAdjacency()
    {
        found = GridMovement.instance.InAdjacentMatrix(attacker.transform.position,defender.transform.position,1);


            List<CharacterBaseClasses> returnedList = GridMovement.instance.InAdjacentMatrix(attacker.transform.position,attacker.GetComponent<TemporaryStats>().CharacterTeam ,1,Color.red);
            listAdjacency.AddRange(returnedList);
            //ActionArchive.instance.Devour(attacker.GetComponent<CharacterBaseClasses>(), listAdjacency[0],attacker.gameObject.GetComponent<TemporaryStats>(), listAdjacency[0].gameObject.GetComponent<TemporaryStats>());
        

        
    }
    void ExecuteTurn()
    {
        Debug.Log("ashse");
        int range = HandleTurn.instance.allTurns.Count;
        for(int i = 0; i < range; i++)
        {
            HandleTurn.instance.allTurns[i].Command.Execute();
        }
    }

}
