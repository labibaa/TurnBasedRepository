using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cysharp.Threading.Tasks;
public class ShadowOfPlayer : MonoBehaviour
{
    public GameObject SpawnedGhost;
    GameObject IsSpawned;
    public List<Turn> ActionTurnListForGhost = new List<Turn>();

    private int i = 0;
    public int j=0;
    Vector3 initialPositionOfGhost;
    Vector3 lastPositionOfGhost;
    Stack<Vector3> lastPositionGhosts = new Stack<Vector3>();

    private void Start()
    {
        
        
    }

    private void OnEnable()
    {
        HandleTurnNew.OnNewAction += UpdateList;
        HandleTurnNew.IsPlayerUndo += UndoGhost;
    }

    private void OnDisable()
    {
        HandleTurnNew.OnNewAction -= UpdateList;
        HandleTurnNew.IsPlayerUndo -= UndoGhost;
   
    }
    private void Update()
    {



        if (TempManager.instance.currentState == GameStates.Simulation|| TempManager.instance.currentState == GameStates.StartTurn)
        {
            Destroy(IsSpawned);

            
        }

    }

    void UpdateList()
    {
        if (TurnManager.instance.players[TurnManager.instance.currentPlayerIndex].gameObject == this.gameObject)
        {
            ActionTurnListForGhost = HandleTurnNew.instance.GetAllTurns();
         /* if (ActionTurnListForGhost.Count<2)
            {
                
               // ActionGhostRepeat();
            }*/
            TempManager.instance.ChangeGameState(GameStates.GhostPlay);
            Debug.Log("ding dong");
            ActionGhostSingular();
            
        }
        else{

            j = 0;
        }
    }

    public async void ActionGhostRepeat() //repeat preview of action using player shadow
    {
  
            if (IsSpawned == null)
            {
                IsSpawned = Instantiate(SpawnedGhost.gameObject, transform.position, Quaternion.identity, transform.parent);
                initialPositionOfGhost = IsSpawned.transform.position;


            }

            // for (int i = 0; i <= ActionTurnListForGhost.Count; i++)
            while (true)
            {
                await CompleteAction(i);

                    i = (i + 1) % ActionTurnListForGhost.Count;
                if (i == 0)
                {
                    IsSpawned.transform.position = initialPositionOfGhost;
                }
                if (TempManager.instance.currentState == GameStates.Simulation|| TempManager.instance.currentState == GameStates.StartTurn)
                {
                    Destroy(IsSpawned);

                    break;
                }

            }

    }

    public async void ActionGhostSingular()  // preview of action using player shadow
    {

        if (IsSpawned == null)
        {
            j = 0;
            IsSpawned = Instantiate(SpawnedGhost.gameObject, transform.position, Quaternion.identity, transform.parent);
            initialPositionOfGhost = IsSpawned.transform.position;

        }

        // for (int i = 0; i <= ActionTurnListForGhost.Count; i++)

        Debug.Log("j"+j+"ping dong"+ ActionTurnListForGhost.Count);


        for (;j<ActionTurnListForGhost.Count;) {
            Debug.Log("ing dong");
            if (ActionTurnListForGhost[j].Command.GetActionType()!="Melee")
            {
                lastPositionGhosts.Push(IsSpawned.transform.position);
                //lastPositionOfGhost = IsSpawned.transform.position;
            }
            
            await CompleteAction(j);
            //% ActionTurnListForGhost.Count;
            
            if (TempManager.instance.currentState == GameStates.Simulation )
            {
                Destroy(IsSpawned);
            }
            j++;
          
        }

        TempManager.instance.ChangeGameState(GameStates.MidTurn);

    }

    void UndoGhost(bool isMeleeMove)
    {
        

        if (TurnManager.instance.players[TurnManager.instance.currentPlayerIndex].gameObject == this.gameObject)
        {
            if (isMeleeMove)
            {
                j = j - 2;

            }
            else
            {
                j--;
            }
            //have to decrease j
            if (lastPositionGhosts.Count>0)
            {
                IsSpawned.transform.position = lastPositionGhosts.Pop();
            }
            
            if (HandleTurnNew.instance.allTurnsOfPlayer.Count<1)
            {
                Destroy(IsSpawned.gameObject);
            }
            //IsSpawned.transform.position = lastPositionOfGhost;
        }
      
    }




    async UniTask CompleteAction(int index)
    {

        if (IsSpawned==null)
        {
            return;
        }
        
        if (ActionTurnListForGhost[index].Command.GetActionName().Equals("Dash") || ActionTurnListForGhost[index].Command.GetActionName().Equals("Move") || ActionTurnListForGhost[index].Command.GetActionName().Equals("WarpSurge"))
        {
            
            List<GameObject>pathToDestination = ActionTurnListForGhost[index].Command.GetPaths();
            IsSpawned.GetComponent<Animator>().Play("Dash");
          
            await IsSpawned.GetComponent<LerpAndLoop>().MoveToDestination(IsSpawned.transform.position,pathToDestination[pathToDestination.Count-1].transform.position );
            IsSpawned.GetComponent<Animator>().Play("Idle");
           
        }
       
        else
        {
           
            await CutsceneManager.instance.PlayAnimationForGhost(IsSpawned, ActionTurnListForGhost[index].Command.GetActionName(),ActionTurnListForGhost[index].Command.GetTarget().gameObject);
           
        }
    }



}