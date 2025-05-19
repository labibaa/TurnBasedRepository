using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.AI;
using Cysharp.Threading.Tasks;
using System;
using DG.Tweening.Plugins.Core.PathCore;
using System.Linq;
using UnityEngine.TextCore.Text;
using System.Drawing;
using Color = UnityEngine.Color;
using UnityEngine.Experimental.GlobalIllumination;

public class GridMovement : MonoBehaviour
{

    public LineRenderer lineRendererPrefab; // Reference to your LineRenderer prefab
    public List<LineRenderer> currentLineRenderer;
    public Material lineMaterial; // Material for the line renderer
    public Color lineColor = Color.white;




    public static GridMovement instance;

    
 
    [SerializeField]
    GameObject gridNavigatorPrefab;
    GameObject gridNavigator;
    Vector2 navigationCoordinates;
    [SerializeField]
    public bool inPathSelection = false;
    [SerializeField]
    public List<GameObject> path = new List<GameObject>();
    public List<GameObject> highlightedPath = new List<GameObject>();
    public List<GameObject> BorderAreaTemp = new List<GameObject>();
    [SerializeField] Vector3 defaultSpeedAttribute;
    Dictionary<Direction, int> directionValue;
    [Header("Move Visual Settings")]
    [Space]
    [SerializeField]
    Renderer clickIcon;

    [SerializeField]
    UnityEngine.Color rightPathColor;

    private static readonly int CLICK_TIME_PROPERTY = Shader.PropertyToID("_Click_Time");
    [SerializeField]
    UnityEngine.Color defaultPathColor;
    [SerializeField]
    public LayerMask layerMask;
    [SerializeField]
    List<GameObject> neighborListGameobjects;
    #region Raycast Stuff
    Ray ray;
    RaycastHit hitInfo;
    #endregion



    #region
    [SerializeField]
    public NavMeshAgent agent;
    [SerializeField]
    GameObject player;
    GridPlayerAnimation gridPlayerAnim;
    GameObject selectedPlayer;


    #endregion

    #region
   // [SerializeField] Direction storePreviousDirection = Direction.none;
    bool oppositeDirection;

    #endregion

    #region MoveSCriptableObjectRelatedStuff
    private ActionStat moveScriptable;
    private ImprovedActionStat improvedActionScriptable;
    private int apCost;
    private int currentAP = 0;
    private int playerAP = 4;
    private NavMeshAgent agentFromMoveAction;
    Vector3 currentGridPositionPlayer;
    [SerializeField]
    private bool isInGrid;




    #endregion

    private void OnEnable()
    {
        //GridInput.GridMove += GridMovementDirection;
        GridInput.GridSelect += Selection;
        GridInput.GridReset += ResetPathSelection;
        GridInput.GridEscape += ExitGrid;
        HealthManager.OnGridDisable += DisableGridElement;
        GridSystem.OnGridGeneration += EnableGridElement;



        //setting animation Idle

    }

    private void OnDisable()
    {
        //GridInput.GridMove -= GridMovementDirection;
        GridInput.GridSelect -= Selection;
        GridInput.GridReset -= ResetPathSelection;
        GridInput.GridEscape -= ExitGrid;
        HealthManager.OnGridDisable -= DisableGridElement;
        GridSystem.OnGridGeneration -= EnableGridElement;





    }

    private void Awake()
    {
        directionValue = new Dictionary<Direction, int>() {
            {Direction.up,1 },
            {Direction.down,-1},
            {Direction.left,-1},
            {Direction.right,1 }


        };

        //Singleton
        if (instance != null)
        {
            Debug.LogWarning("Found more than one grid movement in the scene");
        }
        instance = this;



        //gridNavigatorPrefab.SetActive(true);
        navigationCoordinates = Vector2.zero;
        gridPlayerAnim = agent.GetComponent<GridPlayerAnimation>();

        apCost = 1; // it means 2 steps cost 1 ap
        playerAP = 5;


    }


    private void Start()
    {
        gridNavigator = Instantiate(gridNavigatorPrefab, GridSystem.instance._gridArray[Mathf.FloorToInt(GridSystem.instance._gridArray.GetLength(0) / 2f), Mathf.FloorToInt(GridSystem.instance._gridArray.GetLength(1) / 2f)].transform.position + new Vector3(0f, 0.08f, 0f), gridNavigatorPrefab.transform.rotation);


        navigationCoordinates.x = GridSystem.instance._gridArray.GetLength(0) / 2f;
        navigationCoordinates.y = GridSystem.instance._gridArray.GetLength(1) / 2f;

        gridPlayerAnim.SetMoveAnimation(0, 1);

    }

    void DisableGridElement()
    {
        gridNavigator.SetActive(false);
        ResetHighlightedPath();
        ResetPathSelection();
    }
    void EnableGridElement()
    {
        gridNavigator.SetActive(true);
    }
    private void Update()
    {

        if (Input.GetMouseButton(1) && inPathSelection)
        {
          

            PlayerStatUI.instance.GetPlayerStatSummary(agent.gameObject.GetComponent<CharacterBaseClasses>());
            PlayerStatUI.instance.GetPlayerStatDetails(agent.gameObject.GetComponent<CharacterBaseClasses>());
            ButtonStackManager.instance.UndoStackEntry();
              TempManager.instance.ChangeGameState(GameStates.MidTurn);
            ResetPathSelection();
            ResetHighlightedPath();
            player.gameObject.GetComponent<GridInput>().enabled = false;
            agent.gameObject.GetComponent<PlayerTurn>().isMoveOn = true;
            //path.Clear();
            selectedPlayer = null;

            inPathSelection = false;

            UnityEngine.Cursor.lockState = CursorLockMode.None;
            return;
        }


        UnityEngine.Cursor.lockState = CursorLockMode.None;
        #region Mouse Pointer Mapping
        if (Input.GetMouseButtonDown(0) && inPathSelection)  // grid move mouse code
        {
            // Cast a ray from the mouse cursor into the scene with the specified layer mask
            ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            

            // Check if the ray hits an object on the specified layer
            if (Physics.Raycast(ray, out hitInfo, Mathf.Infinity, layerMask))
            {
                // Check if the object hit is in your grid array
                GameObject clickedGridCell = hitInfo.collider.gameObject;
                if ((clickedGridCell.GetComponent<GridStat>().IsOccupied && agent.gameObject != clickedGridCell.GetComponent<GridStat>().OccupiedGameObject) || Vector3.Distance(clickedGridCell.transform.position,agent.GetComponent<TemporaryStats>().currentPlayerGridPosition)<0.1f)
                {
                    UI.instance.SendNotification("Already Occupied");
                    return;
                }
                if (!InAdjacentMatrix(agent.GetComponent<TemporaryStats>().currentPlayerGridPosition,clickedGridCell.transform.position,playerAP))
                {
                    UI.instance.SendNotification("Out Of Range");
                    return;
                }



                gridNavigator.transform.position = clickedGridCell.transform.position + new Vector3(0f, 0.08f, 0f);
                // Check if the clicked object is part of your grid

                // Perform selection logic here
                // For example, change the color or perform an action on the clicked grid cell
                ChangeColor(clickedGridCell, UnityEngine.Color.red);
                
                path.Add(clickedGridCell);

                AssignMovement();
                clickIcon.gameObject.SetActive(true);
                clickIcon.transform.position = clickedGridCell.transform.position + new Vector3(0f, 0.08f, 0f);
                clickIcon.material.SetFloat(CLICK_TIME_PROPERTY,Time.time);
                Invoke("DisableRenderer", 0.5f);


            }
        }
        #endregion

        OnVaultAnimation();

    }

    private void DisableRenderer()
    {
        clickIcon.gameObject.SetActive(false);
    }

    void AssignMovement() //actions that include movement on grid are implemented here
    {
        ICommand MovementConrete;
        if (moveScriptable.moveName=="WarpSurge")
        {
            MovementConrete = new WarpSurge(path, agent, false, "Move");
        }
        else if (moveScriptable.moveName == "GroundBlast")
        {
            MovementConrete = new GroundBlast(path, agent, false, "Move",moveScriptable);
        }
        else if (moveScriptable.moveName == "Dash")
        {
            MovementConrete = new Dash(path, agent, false, "Move", moveScriptable);
        }
        else
        {
             MovementConrete = new Move(path, agent, false, "Move");
        }
        

       // agent.GetComponent<GhostTrail>().SpawnGhost(agent.transform, path[path.Count - 1].transform);


        currentGridPositionPlayer = path[path.Count - 1].transform.position;

        agent.gameObject.GetComponent<TemporaryStats>().currentPlayerGridPosition = currentGridPositionPlayer;
        TempManager.instance.ChangeGameState(GameStates.MidTurn);
        GameManager.instance.AddCommand(MovementConrete);

        //int previousPV = HandleTurn.instance.GetLatestTurnWithCharacter(agent.gameObject.GetComponent<CharacterBaseClasses>())?.PriorityValue ?? 0;
        int previousPV = 20;
        Turn turn = new Turn(agent.gameObject.GetComponent<CharacterBaseClasses>(), MovementConrete, previousPV + moveScriptable.PriorityValue); //action adding to turn list
        HandleTurnNew.instance.AddTurn(turn);
        ResetPathSelection();
        ResetHighlightedPath();



        player.gameObject.GetComponent<GridInput>().enabled = false;
        //path.Clear();
        selectedPlayer = null;

        inPathSelection = false;
        
        UnityEngine.Cursor.lockState = CursorLockMode.None;

        PlayerStatUI.instance.GetPlayerStatSummary(agent.gameObject.GetComponent<CharacterBaseClasses>()); //character ui
        PlayerStatUI.instance.GetPlayerStatDetails(agent.gameObject.GetComponent<CharacterBaseClasses>());
    }


    void GridMovementDirection(Direction moveDirection)
    {
        Vector2 currentGrid = GridSystem.instance.WorldToGrid(gridNavigator.transform.position);
        navigationCoordinates = currentGrid;
        GridStat currentGridStat = GridSystem.instance._gridArray[(int)currentGrid.x, (int)currentGrid.y].GetComponent<GridStat>();  // this is holding the vairabgle for undoing the system. this stores what it is the previous grid stat


        if ((moveDirection == Direction.up || moveDirection == Direction.down) && (navigationCoordinates.y + directionValue[moveDirection] < GridSystem.instance._gridArray.GetLength(0) && navigationCoordinates.y + directionValue[moveDirection] > -1))
        {

            navigationCoordinates.y = navigationCoordinates.y + (directionValue[moveDirection] * GridSystem.instance.scale); // here updating the navigation coordinates based on the input. 
        }
        else if ((moveDirection == Direction.left || moveDirection == Direction.right) && (navigationCoordinates.x + directionValue[moveDirection] < GridSystem.instance._gridArray.GetLength(1) && navigationCoordinates.x + directionValue[moveDirection] > -1))
        {
            navigationCoordinates.x = navigationCoordinates.x + (directionValue[moveDirection] * GridSystem.instance.scale);//same thing
        }

        gridNavigator.transform.position = GridSystem.instance._gridArray[(int)navigationCoordinates.x, (int)navigationCoordinates.y].transform.position + new Vector3(0f, 0.08f, 0f);// here updated the gridnavigator prefab based on the input. accessing the _gridArray, inputting the updated rows and columns and then updating the transrom.position

        //Debug.Log($"gridnavogator:{gridNavigator.transform.position}");
        //GridSystem.instance.WorldToGrid(gridNavigator.transform.position);

        if (inPathSelection)
        {
            Vector2 tempGridCoordinate = GridSystem.instance.WorldToGrid(gridNavigator.transform.position);
            GridStat tempGridStat = GridSystem.instance._gridArray[(int)tempGridCoordinate.x, (int)tempGridCoordinate.y].GetComponent<GridStat>();



            if (tempGridStat.Visited == -1 && tempGridStat.IsOccupied == false && tempGridStat.IsWalkable == true && playerAP > 0)
            {
                currentAP++;
                if (currentAP > apCost)
                {
                    playerAP--;
                    currentAP = 0;
                }

                Debug.Log("PlayerAP: " + playerAP);




                tempGridStat.gameObject.GetComponent<SpriteRenderer>().color = rightPathColor;
                path.Add(GridSystem.instance._gridArray[(int)tempGridCoordinate.x, (int)tempGridCoordinate.y]);





                GameObject dummyPointerObject = Instantiate(selectedPlayer.GetComponent<TemporaryStats>().Pathmark, path[path.Count - 1].transform.position, Quaternion.identity);//generating the object
                dummyPointerObject.transform.parent = path[path.Count - 1].transform;//making the path it is generated on its parent


                dummyPointerObject.name = selectedPlayer.name;
                dummyPointerObject.tag = "Cue";
                GridSystem.instance._gridArray[(int)tempGridCoordinate.x, (int)tempGridCoordinate.y].GetComponent<GridStat>().Visited = 1; // making the path visited
                GridSystem.instance._gridArray[(int)tempGridCoordinate.x, (int)tempGridCoordinate.y].GetComponent<GridStat>().PressedKeyDirection = moveDirection;
            }
            else
            {
                Debug.Log("Current" + moveDirection);

                if (DictionaryManager.instance.GiveMoveDirection(currentGridStat.PressedKeyDirection) == moveDirection)
                {
                    currentAP--;
                    if (currentAP < 0)
                    {
                        playerAP++;
                        currentAP = apCost;
                    }

                    Debug.Log("PlayerAP: " + playerAP);
                    GameObject tempObject = path[path.Count - 1];
                    tempObject.GetComponent<SpriteRenderer>().color = defaultPathColor;
                    tempObject.GetComponent<GridStat>().Visited = -1;
                    Destroy(tempObject.transform.Find(selectedPlayer.name).gameObject);//destroyin the visual cue
                    path.RemoveAt(path.Count - 1);

                    GameObject temp = path[path.Count - 1];
                    navigationCoordinates = GridSystem.instance.WorldToGrid(temp.transform.position);
                    gridNavigator.transform.position = GridSystem.instance._gridArray[(int)navigationCoordinates.x, (int)navigationCoordinates.y].transform.position + new Vector3(0f, 0.08f, 0f); ;

                }
                else
                {




                    Debug.Log("Wrong Selection");

                    GameObject temp = path[path.Count - 1];
                    navigationCoordinates = GridSystem.instance.WorldToGrid(temp.transform.position);
                    gridNavigator.transform.position = GridSystem.instance._gridArray[(int)navigationCoordinates.x, (int)navigationCoordinates.y].transform.position + new Vector3(0f, 0.08f, 0f); ;

                }

            }


        }


    }

    async void Selection()
    {

        if (!inPathSelection)
        {
            Vector2 tempGridCoordinate = GridSystem.instance.WorldToGrid(gridNavigator.transform.position);
            if (GridSystem.instance._gridArray[(int)tempGridCoordinate.x, (int)tempGridCoordinate.y].GetComponent<GridStat>().OccupiedObject == StringData.PlayerTag)
            {
                GridSystem.instance._gridArray[(int)tempGridCoordinate.x, (int)tempGridCoordinate.y].GetComponent<SpriteRenderer>().color = rightPathColor;
                path.Add(GridSystem.instance._gridArray[(int)tempGridCoordinate.x, (int)tempGridCoordinate.y]);
                inPathSelection = true;
                GameObject occupiedGameObject = GridSystem.instance._gridArray[(int)tempGridCoordinate.x, (int)tempGridCoordinate.y].GetComponent<GridStat>().OccupiedGameObject;
                selectedPlayer = occupiedGameObject;
                agent = occupiedGameObject.GetComponent<NavMeshAgent>();

                Debug.Log("selected");
            }
            else
            {
                Debug.LogError("No player");

            }
        }
        else
        {

            AssignMovement();


        }


    }

    public void ResetPathSelection()
    {

        //
        selectedPlayer = null;
        inPathSelection = false;
        foreach (GameObject obj in path)
        {
            obj.GetComponent<GridStat>().Visited = -1;//
            obj.GetComponent<GridStat>().PressedKeyDirection = Direction.none;//
            obj.GetComponent<SpriteRenderer>().color = defaultPathColor;
           // obj.GetComponent<Renderer>().material.color = defaultPathColor;



        }
        path.Clear();//resetting the path
        //bringing back the nabigator position to the player position
        Vector2 tempStartPoint = GridSystem.instance.WorldToGrid(agent.transform.position);//storing the player position in grid



        //GridSystem.instance._gridArray[Mathf.FloorToInt(tempStartPoint.x), Mathf.FloorToInt(tempStartPoint.y)].GetComponent<SpriteRenderer>().color = rightPathColor;//making the player position grid green
        gridNavigator.transform.position = GridSystem.instance._gridArray[Mathf.FloorToInt(tempStartPoint.x), Mathf.FloorToInt(tempStartPoint.y)].transform.position + new Vector3(0f, 0.08f, 0f);//setting the navigator position by retreiving the transform of the grid that is on the playerPosition
        navigationCoordinates.x = tempStartPoint.x;
        navigationCoordinates.y = tempStartPoint.y; //storing the current value of navigator, so that when input is pressed, it moves accordingly
                                                    //path.Add(GridSystem.instance._gridArray[(int)navigationCoordinates.x, (int)navigationCoordinates.y]);//adding the current player position to the path 


    }

    public async UniTask MoveCharacterGrid(List<GameObject> path, NavMeshAgent agent, Vector3 speedAttributes, string actionName)
    {
        
        

       

        for (int i = 1; i < path.Count; i++)
        {
            
            //agent.GetComponent<TemporaryStats>().lastPosition = path[i - 1];

            if (path[i].GetComponent<GridStat>().IsOccupied && path[i].GetComponent<GridStat>().OccupiedGameObject.CompareTag("Player"))
            {
                //agent.speed = 0;
                //agent.isStopped = true;
                //agent.SetDestination(agent.transform.position);
                ////currentGridPositionPlayer = agent.transform.position;


                Debug.Log("SG");


                //agent.GetComponent<PlayerMove>().startMoving = false;
                //agent.GetComponent<PlayerMove>().speed = 0f;

                ResetPathSelection();

                for (int jvalue = i; jvalue < path.Count; jvalue++)
                {
                    Destroy(path[jvalue].transform.Find(agent.gameObject.name).gameObject);
                }
                break;

            }
            else
            {
              
                if (path[i].GetComponent<GridStat>().Height != -1)
                {
                    Vector3 pathToGo = new Vector3(path[i].transform.position.x, path[i].GetComponent<GridStat>().Height, path[i].transform.position.z);
                    agent.SetDestination(pathToGo);
                }
                else
                {
                    agent.SetDestination(path[i].transform.position);
                }

                //agent.transform.localPosition = Vector3.MoveTowards(agent.transform.position, path[i].transform.position, 0.01f);

                //agent.GetComponent<PlayerMove>().startMoving = true;
                //agent.GetComponent<PlayerMove>().targetPos = path[i].transform.position;


            }



            if (agent.gameObject.CompareTag("Player") )//have to change the hardcode later
            {
                if(actionName == "Dash")
                {
                    agent.gameObject.GetComponent<Animator>().SetBool("dash", true);
                }

                // gridPlayerAnim.SetMoveAnimation(10, 1);
                agent.GetComponent<GridPlayerAnimation>().SetMoveAnimation(10, 1);

            }
            else
            {
                agent.gameObject.GetComponent<Animator>().SetBool("Walking", true);
            }

            // CutsceneManager.instance.PlayAnimationForCharacter(agent.gameObject,"Walking");

            //setting the move animation
            //while (agent.pathPending || agent.remainingDistance > agent.stoppingDistance)//sequentially going from one point to another
            //while (Vector3.Distance(agent.transform.position, path[i].transform.position) > 0.1f)
            agent.speed = speedAttributes.x;
            agent.angularSpeed = speedAttributes.y;
            agent.acceleration = speedAttributes.z;
            while (agent.pathPending || agent.remainingDistance > agent.stoppingDistance && (GridSystem.instance.IsGridOn))
            {

                await UniTask.Yield();


            }
            //path[i].GetComponent<Renderer>().material.color = new UnityEngine.Color(0, 0, 0, 0); ;// defaultPathColor;

            if (i != 0 && !agent.gameObject.GetComponent<TemporaryStats>().AutoMove)
            {
                Transform foundTransform = path[i].transform.Find(agent.gameObject.name);
                if (foundTransform != null)
                {
                    Destroy(foundTransform.gameObject);
                }
            }

        }

        //agent.GetComponent<PlayerMove>().startMoving = false;//setting the rotation and movement to zero . as all the path has been traversed 
        agent.speed = defaultSpeedAttribute.x;
        agent.angularSpeed = defaultSpeedAttribute.y;
        agent.acceleration = defaultSpeedAttribute.z;
        
        ResetPathSelection();
        if (agent.gameObject.CompareTag("Player"))
        {
            if (actionName == "Dash")
            {
                agent.gameObject.GetComponent<Animator>().SetBool("dash", false);
            }

            agent.GetComponent<GridPlayerAnimation>().SetMoveAnimation(0, 1);//setting the idle animation
           
           
        }
        else
        {
            agent.gameObject.GetComponent<Animator>().SetBool("Walking", false);
        }


    }


    void ExitGrid()
    {

        foreach (GameObject gameObject in GridSystem.instance._gridArray)
        {
            Destroy(gameObject);
        }
        Array.Clear(GridSystem.instance._gridArray, 0, GridSystem.instance._gridArray.Length);
        //ResetPathSelection();
        Destroy(gridNavigator);





        Debug.Log("exitGrid");
    }

    public void EndTurn()
    {
        foreach (GameObject obj in GridSystem.instance._gridArray)
        {
            obj.GetComponent<GridStat>().Visited = -1;//
            obj.GetComponent<SpriteRenderer>().color = defaultPathColor;
        }
    }


    public void ExecuteModeOn()
    {
        GridInput.GridMove -= GridMovementDirection;//temporarily disabling the navigator movement while player is moving
        GridInput.GridReset -= ResetPathSelection;
        GridInput.GridSelect -= Selection;
    }

    public void ExecuteModeOff()
    {
        GridInput.GridMove += GridMovementDirection;//enabling the player movement again after the player movement is done
        GridInput.GridReset += ResetPathSelection;
        GridInput.GridSelect += Selection;
    }



    public void setMoveParam(ActionStat move, int currentPlayerAP, Vector3 currentPositionOnGrid, NavMeshAgent playerAgent)
    {
        moveScriptable = move;
        agentFromMoveAction = playerAgent;
        apCost = moveScriptable.APCost - 1;
        playerAP = currentPlayerAP;
        inPathSelection = true;
        agent = playerAgent;
        currentGridPositionPlayer = currentPositionOnGrid;
        setGridNav();
    }

    public void setGridNav()
    {

        Vector2 playerGridCoOrdinates = GridSystem.instance.WorldToGrid(currentGridPositionPlayer);

        gridNavigator.transform.position = GridSystem.instance._gridArray[(int)playerGridCoOrdinates.x, (int)playerGridCoOrdinates.y].transform.position;
        Vector2 tempGridCoordinateForSelection = GridSystem.instance.WorldToGrid(gridNavigator.transform.position);
        selectedPlayer = agent.gameObject;
        GridSystem.instance._gridArray[(int)tempGridCoordinateForSelection.x, (int)tempGridCoordinateForSelection.y].GetComponent<SpriteRenderer>().color = rightPathColor;
        path.Add(GridSystem.instance._gridArray[(int)tempGridCoordinateForSelection.x, (int)tempGridCoordinateForSelection.y]);



        //if (!inPathSelection)
        //{
        //    Vector2 tempGridCoordinate = GridSystem.instance.WorldToGrid(gridNavigator.transform.position);
        //    if (GridSystem.instance._gridArray[(int)tempGridCoordinate.x, (int)tempGridCoordinate.y].GetComponent<GridStat>().OccupiedObject == StringData.PlayerTag)
        //    {
        //        GridSystem.instance._gridArray[(int)tempGridCoordinate.x, (int)tempGridCoordinate.y].GetComponent<SpriteRenderer>().color = rightPathColor;
        //        path.Add(GridSystem.instance._gridArray[(int)tempGridCoordinate.x, (int)tempGridCoordinate.y]);
        //        inPathSelection = true;
        //        GameObject occupiedGameObject = GridSystem.instance._gridArray[(int)tempGridCoordinate.x, (int)tempGridCoordinate.y].GetComponent<GridStat>().OccupiedGameObject;
        //        selectedPlayer = occupiedGameObject;
        //        agent = occupiedGameObject.GetComponent<NavMeshAgent>();

        //        Debug.Log("selected");
        //    }
        //    else
        //    {
        //        Debug.LogError("No player");

        //    }
        //}
    }

    public void getMoveParam(out List<GameObject> pathGameObject, out NavMeshAgent agentNavmesh)// out int remainingPlayerAP)
    {
        pathGameObject = path;
        agentNavmesh = agentFromMoveAction;
        //remainingPlayerAP = playerAP;
        ResetPathSelection();

        selectedPlayer = null;

        inPathSelection = false;
    }


    public bool InAdjacentMatrix(Vector3 attackPosition, Vector3 targetPosition, int range) //this checks if a player is on its range or not
    {
        Vector2 attackerCoordinates = GridSystem.instance.WorldToGrid(attackPosition);
        Vector2 targetCoordinates = GridSystem.instance.WorldToGrid(targetPosition);


        if(GetAdjacentNeighbors(attackerCoordinates, range).Contains(targetCoordinates)){
            return true;
        }
        else
        {
            return false;
        }

      


    }

    public List<CharacterBaseClasses> InAdjacentMatrix(Vector3 attackPosition, TeamName teamName, int range, UnityEngine.Color gridColor) //this returns the available list of characters
    {
        Vector2 attackerCoordinate = GridSystem.instance.WorldToGrid(attackPosition);
        
        List<CharacterBaseClasses> adjacencyList = new List<CharacterBaseClasses>();
        List<Vector2> neighborListCoordinates;
        neighborListCoordinates = GetAdjacentNeighbors(attackerCoordinate,range);
        neighborListGameobjects.Clear();

        int gridSizeX = GridSystem.instance._gridArray.GetLength(0);
        int gridSizeY = GridSystem.instance._gridArray.GetLength(1);


        for (int i=0;i<neighborListCoordinates.Count;i++)
        {
            if ((neighborListCoordinates[i].x>=0 && neighborListCoordinates[i].x<gridSizeX)&&(neighborListCoordinates[i].y >= 0 && neighborListCoordinates[i].y < gridSizeY))
            {

                neighborListGameobjects.Add(GridSystem.instance._gridArray[(int)neighborListCoordinates[i].x, (int)neighborListCoordinates[i].y]);
                
                CharacterBaseClasses character;

           
                ChangeColor(GridSystem.instance._gridArray[(int)neighborListCoordinates[i].x, (int)neighborListCoordinates[i].y], gridColor);

                //ChangeColor(GridSystem.instance._gridArray[tempX, tempY], Color.red);
                highlightedPath.Add(GridSystem.instance._gridArray[(int)neighborListCoordinates[i].x, (int)neighborListCoordinates[i].y]);


                if (GridSystem.instance._gridArray[(int)neighborListCoordinates[i].x, (int)neighborListCoordinates[i].y].TryGetComponent(out GridStat gridStat) &&
                    gridStat.OccupiedGameObject != null &&
                    gridStat.OccupiedGameObject.TryGetComponent(out character))
                {
                    if (character.gameObject.GetComponent<TemporaryStats>().playerMortality == Mortality.Alive && teamName != character.gameObject.GetComponent<TemporaryStats>().CharacterTeam)
                    {
                        adjacencyList.Add(character);
                    }

                }


            }
        }

       
        List<GameObject> BorderArea = new List<GameObject>();
        foreach (GameObject gm in neighborListGameobjects)
        {
            var neighbors = gm.GetComponent<GridStat>().neighborCoordinates;
            bool notContains=false;
            for (int i =0;i<neighbors.Count;i++)
            {
                if (!neighborListCoordinates.Contains(neighbors[i]) || neighbors[i].x<0|| neighbors[i].x>=gridSizeX || neighbors[i].y < 0 || neighbors[i].y >= gridSizeY)
                {
                    notContains = true;
                    break;
                }
            }
            if (notContains)
            {
                BorderArea.Add(gm);
            }
        }
       
      
       // foreach(GameObject gm in BorderArea)
       // {
       //     ChangeColor(gm,Color.blue);
       // }
       //DrawBorder(attackPosition,BorderArea);
        return adjacencyList;
    }

    public void ChangeColor(GameObject gridElement, UnityEngine.Color color)
    {
        gridElement.GetComponent<SpriteRenderer>().color = color;
        //gridElement.GetComponent<Renderer>().material.color = color;
    }
    public void ResetHighlightedPath()
    {
        for (int i = 0; i < highlightedPath.Count; i++)
        {
            ChangeColor(highlightedPath[i], defaultPathColor);
        }
        highlightedPath.Clear();
        for(int i = 0; i < currentLineRenderer.Count; i++)
        {
            Destroy(currentLineRenderer[i]);
        }
        currentLineRenderer.Clear();
    }
    


    #region GridMouse




    #endregion

    public void OnVaultAnimation()
    {
        if (agent.enabled)
        {
            if(agent.isOnOffMeshLink)
            {
                var meshLink = agent.currentOffMeshLinkData;

                if(meshLink.offMeshLink.area == NavMesh.GetAreaFromName("Jump"))
                {
                    //CutsceneManager.instance.PlayJumpAnimation(agent.gameObject, "BoxJump");
                }
            }
        }
    }
     public void SetDefaultPathColor(UnityEngine.Color color)
    {
        defaultPathColor = color;
    }


    public List<Vector2> GetAdjacentNeighbors(Vector2 nodePosition, int range)
    {
        List<Vector2> finalAdjacentNeighbors = new List<Vector2>();       
            for (int i = -range; i <= range; i++)
            {
                int elementPerRow = ((2 * range + 1) - Mathf.Abs(i)) / 2;
                if (Mathf.Abs(i) % 2 == 0)
                {
                    for (int j = -elementPerRow; j <= elementPerRow; j++)
                    {                     
                        finalAdjacentNeighbors.Add(new Vector2(nodePosition.x + j, nodePosition.y + i));
                    }
                }
                else
                {
                    if (nodePosition.y % 2 == 0)
                    {
                        for (int j = -elementPerRow + 1; j <= elementPerRow; j++)
                        {
                            finalAdjacentNeighbors.Add(new Vector2(nodePosition.x + j, nodePosition.y + i));
                        }
                    }
                    else
                    {
                        for (int j = -elementPerRow; j < elementPerRow; j++)
                        {
                            finalAdjacentNeighbors.Add(new Vector2(nodePosition.x + j, nodePosition.y + i));
                        }
                    }                  
                }
            }
            
        return finalAdjacentNeighbors;
    }
    void DrawBorder(Vector3 center, List<GameObject>BorderGameObjects)
    {
        var topRightCell = neighborListGameobjects.OrderByDescending(t => t.transform.position.z).ThenByDescending(t => t.transform.position.x).FirstOrDefault();
        var topLeftCell = neighborListGameobjects.OrderByDescending(t => t.transform.position.z).ThenBy(t => t.transform.position.x).FirstOrDefault();
        var midLeftCell = neighborListGameobjects.OrderBy(t => t.transform.position.x).ThenByDescending(t => t.transform.position.z).FirstOrDefault();
        var midRightCell = neighborListGameobjects.OrderByDescending(t => t.transform.position.x).ThenBy(t => t.transform.position.z).FirstOrDefault();
        var leftBottomCell = neighborListGameobjects.OrderBy(t => t.transform.position.z).ThenBy(t => t.transform.position.x).FirstOrDefault();       
        var rightBottomCell = neighborListGameobjects.OrderBy(t => t.transform.position.z).ThenByDescending(t => t.transform.position.x).FirstOrDefault();

        List<Transform> lineRendererTransform = new List<Transform>();
        //top right cell

        lineRendererTransform.Add(topRightCell.GetComponent<GridStat>().directionTransformMap[HexOrientation.DownRight]);
        lineRendererTransform.Add(topRightCell.GetComponent<GridStat>().directionTransformMap[HexOrientation.Down]);
        lineRendererTransform.Add(topRightCell.GetComponent<GridStat>().directionTransformMap[HexOrientation.DownLeft]);
        lineRendererTransform.Add(topRightCell.GetComponent<GridStat>().directionTransformMap[HexOrientation.UpLeft]);


        //top right cell to middle right
        List<GameObject> topRightGameobjects = BorderGameObjects
    .Where(obj => obj.transform.position.z > center.z && obj.transform.position.x >= topRightCell.transform.position.x && obj != topRightCell && obj != topRightCell && obj != midRightCell)
    .ToList();

        topRightGameobjects.Reverse();
        foreach (var cell in topRightGameobjects)
        {
         
            lineRendererTransform.Add(cell.GetComponent<GridStat>().directionTransformMap[HexOrientation.Down]);
            lineRendererTransform.Add(cell.GetComponent<GridStat>().directionTransformMap[HexOrientation.DownLeft]);
            lineRendererTransform.Add(cell.GetComponent<GridStat>().directionTransformMap[HexOrientation.UpLeft]);
          
        }

        //middle right cell 
        lineRendererTransform.Add(midRightCell.GetComponent<GridStat>().directionTransformMap[HexOrientation.Down]);
        lineRendererTransform.Add(midRightCell.GetComponent<GridStat>().directionTransformMap[HexOrientation.DownLeft]);
        lineRendererTransform.Add(midRightCell.GetComponent<GridStat>().directionTransformMap[HexOrientation.UpLeft]);
        lineRendererTransform.Add(midRightCell.GetComponent<GridStat>().directionTransformMap[HexOrientation.Up]);

        //middle right cell to bottom right cell
        List<GameObject> bottomRightGameobjects = BorderGameObjects
    .Where(obj => obj.transform.position.z < center.z && obj.transform.position.x >= rightBottomCell.transform.position.x && obj != rightBottomCell && obj != midRightCell)
    .ToList();
        bottomRightGameobjects.Reverse();
        foreach (var cell in bottomRightGameobjects)
        {
            lineRendererTransform.Add(cell.GetComponent<GridStat>().directionTransformMap[HexOrientation.DownLeft]);
            lineRendererTransform.Add(cell.GetComponent<GridStat>().directionTransformMap[HexOrientation.UpLeft]);
            lineRendererTransform.Add(cell.GetComponent<GridStat>().directionTransformMap[HexOrientation.Up]);
        
            
        }
        //bottom right cell 
        lineRendererTransform.Add(rightBottomCell.GetComponent<GridStat>().directionTransformMap[HexOrientation.DownLeft]);
        lineRendererTransform.Add(rightBottomCell.GetComponent<GridStat>().directionTransformMap[HexOrientation.UpLeft]);
        lineRendererTransform.Add(rightBottomCell.GetComponent<GridStat>().directionTransformMap[HexOrientation.Up]);
        lineRendererTransform.Add(rightBottomCell.GetComponent<GridStat>().directionTransformMap[HexOrientation.UpRight]);




        //bottom right cell to bottom left cell

        List<GameObject> cellsDownBetween = BorderGameObjects
    .Where(cell => cell.transform.position.x <= rightBottomCell.transform.position.x && cell.transform.position.x >= leftBottomCell.transform.position.x
                && cell.transform.position.z == rightBottomCell.transform.position.z && cell != rightBottomCell && cell != leftBottomCell)
    .ToList();

        cellsDownBetween.Reverse();
        // Apply color function to each cell in the list
        foreach (var cell in cellsDownBetween)
        {

            lineRendererTransform.Add(cell.GetComponent<GridStat>().directionTransformMap[HexOrientation.UpLeft]);
            lineRendererTransform.Add(cell.GetComponent<GridStat>().directionTransformMap[HexOrientation.Up]);
            lineRendererTransform.Add(cell.GetComponent<GridStat>().directionTransformMap[HexOrientation.UpRight]);
        }


        //bottom left cell

        lineRendererTransform.Add(leftBottomCell.GetComponent<GridStat>().directionTransformMap[HexOrientation.UpLeft]);
        lineRendererTransform.Add(leftBottomCell.GetComponent<GridStat>().directionTransformMap[HexOrientation.Up]);
        lineRendererTransform.Add(leftBottomCell.GetComponent<GridStat>().directionTransformMap[HexOrientation.UpRight]);
        lineRendererTransform.Add(leftBottomCell.GetComponent<GridStat>().directionTransformMap[HexOrientation.DownRight]);

        //bottom left cell to middle left cell  -------imp

        List<GameObject> bottomLeftGameobjects = BorderGameObjects
   .Where(obj => obj.transform.position.z < center.z && obj.transform.position.x < center.x && obj.transform.position.x<leftBottomCell.transform.position.x && obj != leftBottomCell && obj != midLeftCell)
   .ToList();

       
        //bottomLeftGameobjects.RemoveAt(0);
        foreach (var cell in bottomLeftGameobjects)

        {
           
            lineRendererTransform.Add(cell.GetComponent<GridStat>().directionTransformMap[HexOrientation.Up]);
            lineRendererTransform.Add(cell.GetComponent<GridStat>().directionTransformMap[HexOrientation.UpRight]);
            //lineRendererTransform.Add(cell.GetComponent<GridStat>().directionTransformMap[HexOrientation.DownRight]);
            
        }


        //middle left cell

       lineRendererTransform.Add(midLeftCell.GetComponent<GridStat>().directionTransformMap[HexOrientation.Up]);
       lineRendererTransform.Add(midLeftCell.GetComponent<GridStat>().directionTransformMap[HexOrientation.UpRight]);
       lineRendererTransform.Add(midLeftCell.GetComponent<GridStat>().directionTransformMap[HexOrientation.DownRight]);
       lineRendererTransform.Add(midLeftCell.GetComponent<GridStat>().directionTransformMap[HexOrientation.Down]);



        //middle left cell to top left

        List<GameObject> topLeftGameobjects = BorderGameObjects
    .Where(obj => obj.transform.position.z > center.z && obj.transform.position.x < center.x && obj.transform.position.x<topLeftCell.transform.position.x && obj != topLeftCell && obj != midLeftCell)
    .ToList();
        
        //topLeftGameobjects.RemoveAt(topLeftGameobjects.Count-1);
        foreach (var cell in topLeftGameobjects)
        {
            
            //lineRendererTransform.Add(cell.GetComponent<GridStat>().directionTransformMap[HexOrientation.UpRight]);
            lineRendererTransform.Add(cell.GetComponent<GridStat>().directionTransformMap[HexOrientation.DownRight]);
            lineRendererTransform.Add(cell.GetComponent<GridStat>().directionTransformMap[HexOrientation.Down]);
           
            
        }

        

        //top left
        lineRendererTransform.Add(topLeftCell.GetComponent<GridStat>().directionTransformMap[HexOrientation.UpRight]);
        lineRendererTransform.Add(topLeftCell.GetComponent<GridStat>().directionTransformMap[HexOrientation.DownRight]);
        lineRendererTransform.Add(topLeftCell.GetComponent<GridStat>().directionTransformMap[HexOrientation.Down]);
        lineRendererTransform.Add(topLeftCell.GetComponent<GridStat>().directionTransformMap[HexOrientation.DownLeft]);
        //top left to top right
        List<GameObject> cellsBetween = BorderGameObjects
    .Where(cell => cell.transform.position.x <= topRightCell.transform.position.x && cell.transform.position.x >= topLeftCell.transform.position.x
                && cell.transform.position.z == topRightCell.transform.position.z && cell != topRightCell && cell != topLeftCell)
    .ToList();

        // Apply color function to each cell in the list
        foreach (var cell in cellsBetween)
        {
            lineRendererTransform.Add(cell.GetComponent<GridStat>().directionTransformMap[HexOrientation.DownRight]);
            lineRendererTransform.Add(cell.GetComponent<GridStat>().directionTransformMap[HexOrientation.Down]);
            lineRendererTransform.Add(cell.GetComponent<GridStat>().directionTransformMap[HexOrientation.DownLeft]);
            
        }




      

        //lineRendererPrefab.positionCount = lineRendererTransform.Count;
        //for(int i = 0; i < lineRendererTransform.Count; i++)
        //{
        //    lineRendererPrefab.SetPosition(i, lineRendererTransform[i].transform.position);
        //}

        for (int i = 0; i < lineRendererTransform.Count-1 ; i++)
        {
            
            //Instantiate(GameObject.CreatePrimitive(PrimitiveType.Cube), lineRendererTransform[i]);
            //yield return new WaitForSeconds(0.5f);
            CreateLine(lineRendererTransform[i].position, lineRendererTransform[i + 1].position, i);

        }

    }


    void CreateLine(Vector3 startPos, Vector3 endPos,int count)
    {
        // Instantiate a new LineRenderer prefab
        LineRenderer lineRenderer = Instantiate(lineRendererPrefab, Vector3.zero, Quaternion.identity);
        currentLineRenderer.Add(lineRenderer);
        lineRenderer.name = count.ToString();
        // Set the material and color for the line renderer
       // lineRenderer.material = lineMaterial;
        //lineRenderer.startColor = lineColor;
       // lineRenderer.endColor = lineColor;

        // Set the positions of the line renderer
        lineRenderer.positionCount = 2;
        lineRenderer.SetPosition(0, startPos);
        lineRenderer.SetPosition(1, endPos);
    }


    public bool InGridBounds(Vector2 coord)
    {
        return (coord.x >= 0 && coord.x < GridSystem.instance._gridArray.GetLength(0)) && (coord.y >= 0 && coord.y < GridSystem.instance._gridArray.GetLength(1));
    }

    public  bool RemoveElementIfExists<T>(Queue<T> queue, T element)
    {
        bool found = false;
        int originalCount = queue.Count;
        Queue<T> tempQueue = new Queue<T>();

        for (int i = 0; i < originalCount; i++)
        {
            T item = queue.Dequeue();
            if (!found && EqualityComparer<T>.Default.Equals(item, element))
            {
                found = true;
            }
            else
            {
                tempQueue.Enqueue(item);
            }
        }

        // Refill the original queue with the elements in tempQueue
        while (tempQueue.Count > 0)
        {
            queue.Enqueue(tempQueue.Dequeue());
        }

        return found;
    }

}
