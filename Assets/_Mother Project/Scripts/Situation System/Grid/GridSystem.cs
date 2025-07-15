using System.Collections;
using System.Collections.Generic;
//using Unity.Mathematics;
using UnityEngine;
using Cysharp.Threading.Tasks;
using System;
using StarterAssets;
using Unity.AI.Navigation;
using UnityEditor;


public class GridSystem : MonoBehaviour
{
    public static event Action OnGridGeneration;
    public static event Action OnGridGenerationSpawn;
    public static event Action OnGridPositionInitialization;


    public static GridSystem instance;
    [SerializeField] HexOrientation hexOrientation;
    [SerializeField]
    GameObject gameManager;

    [SerializeField]
    int rows = 10;
    [SerializeField]
    int columns = 10;
    [SerializeField]
    int gridUpstairThresholdRows;
    [SerializeField]
    int gridUpstairThresholdColumns;


    [SerializeField]
    public float scale = 1;
    [SerializeField]
    GameObject gridPrefab;
    [SerializeField]
    Vector3 leftBottomLocation = Vector3.zero;
    [SerializeField]
     public GameObject gridStartLocation;

    public GameObject[,] _gridArray;
    public Dictionary<Vector3, GameObject> cubeCoordinates = new Dictionary<Vector3, GameObject>();
    //[SerializeField]
    //int startX = 0, startY = 0, endX = 2, endY = 2;
    [SerializeField]
    List<GameObject> path = new List<GameObject>();

    //bool findDistance = false;
    [SerializeField]
    GameObject player;
    public bool IsGridOn;
    [SerializeField]
    LayerMask gridStatlayer;
    [SerializeField]
    Bounds gridBounds;
    [SerializeField]
    bool GridVisualOn = true;

    int expGain = 0;

    private void Awake()
    {

        if (instance != null)
        {
            Debug.LogWarning("Found more than one GridSystem in the scene");
        }
        instance = this;

        _gridArray = new GameObject[columns, rows];
        


    }
    // Start is called before the first frame update
    void Start()
    {
       
        
    }



    private void OnEnable()
    {
        InputManager.OnInteractionPressed += GenerateGridOnButton;
        GridInput.GridEscape += OnExitGrid;
        SwitchMC.OnCharacterChange += SetMainPlayer;
    }

    private void OnDisable()
    {
        InputManager.OnInteractionPressed -= GenerateGridOnButton;
        GridInput.GridEscape -= OnExitGrid;
        SwitchMC.OnCharacterChange -= SetMainPlayer;
    }





    // Update is called once per frame
    void Update()
    {
        //if (findDistance)
        //{
        //    SetDistance();
        //    SetPath();
        //    findDistance = false;
        //}
        if (Input.GetKeyDown(KeyCode.G) && IsGridOn && TempManager.instance.currentState!= GameStates.MovementGridSelectionTurn && TempManager.instance.currentState != GameStates.TargetSelectionTurn)
        {
            if(GridVisualOn)
            {
                GridVisualEnable();
                GridVisualOn= false;
            }  
            else
            {
                GridVisualDisable();
                GridVisualOn= true;
            }
        }
        /* if (Input.GetKeyDown(KeyCode.F))
         {
             GenerateGridOnButton();
         }*/     
    }

    void SetMainPlayer()
    {
        player = SwitchMC.Instance.mainCharacter;
    }
    public void GenerateGridOnButton()
    {
        if (IsGridOn)
        {
            return;
        }
        //IsGridOn = true;
        //leftBottomLocation = player.transform.position - new Vector3(Mathf.Floor(rows / 2f), 0f, Mathf.Floor(columns / 2f));
        OnGridPositionInitialization?.Invoke();
        leftBottomLocation = gridStartLocation.transform.position;
        Invoke("InitializeGrid", 0f);
        InputManager.OnInteractionPressed -= GenerateGridOnButton;
        Cursor.lockState = CursorLockMode.None;
     
        //player.transform.position =    new Vector3(leftBottomLocation.x, leftBottomLocation.y+0.1f, leftBottomLocation.z);
        ExperienceManager.instance.AddExperiencePoints(expGain);
        expGain += 100;

    }

    async UniTask InitializeGrid() // initializes the grid when conditions are met
    {

        if (gridPrefab)
        {
            OnGridGeneration?.Invoke();//Gridcamera is subscribed to this event and Camera is switched to  gridCamera 
            player.GetComponent<ThirdPersonController>().DisableAnim();
            player.GetComponent<ThirdPersonController>().enabled = false;
            //player.GetComponent<CharacterController>().enabled = false;
            //player.GetComponent<PlayerMove>().enabled = true;

            await GenerateGrid();

            //might need to refactor this part, putting them on  a funciton
            //player.GetComponent<GridInput>().enabled = true;
            player.GetComponent<GridPlayerAnimation>().enabled = true;
            gameManager.GetComponent<GridMovement>().enabled = true;
            gameManager.GetComponent<TurnManager>().enabled = true;




            
            CalculateBounds();
            OnGridGenerationSpawn?.Invoke();
            GridVisualDisable();
        }
        else
        {

            Debug.LogError("assign the grid prefab or Grid Already On ");
        }
    }

    bool IsObstaclePresent(Vector3 position)
    {
        Collider[] colliders = Physics.OverlapBox(position, gridPrefab.transform.localScale / 2f, Quaternion.identity);

        // Check if there is any collider with the specified tag
        foreach (Collider collider in colliders)
        {
            if (collider.CompareTag("Obs"))
            {
                return true;
            }
        }

        return false;
    }

    async UniTask GenerateGrid()
    {
        for (int i = 0; i < columns; i++)
        {
            for (int j = 0; j < rows; j++)
            {
                if (j % 2 == 0)

                {
                    _gridArray[i, j] = Instantiate(gridPrefab, new Vector3(leftBottomLocation.x + i * (scale + 0.675f), leftBottomLocation.y + 0.02f, leftBottomLocation.z + ((scale + 0.35f) * j)), gridPrefab.transform.rotation);
                }
                else
                {
                    _gridArray[i, j] = Instantiate(gridPrefab, new Vector3(leftBottomLocation.x + i * (scale + 0.675f) - 0.85f, leftBottomLocation.y + 0.02f, leftBottomLocation.z + ((scale + 0.35f) * j)), gridPrefab.transform.rotation);
                }
                _gridArray[i, j].transform.SetParent(gameObject.transform);
                _gridArray[i, j].name = $"Cell {i},{j}";
                _gridArray[i, j].layer = 14;

              
                _gridArray[i, j].GetComponent<GridStat>().neighborCoordinates = GetAdjacentTiles(new Vector2(i, j));
            }
        }
        //await UniTask.Delay(TimeSpan.FromSeconds(1f));
    }

    //void SetDistance()
    //{

    //    InitialSetup();
    //    int x = startX;
    //    int y = startY;

    //    int[] testArray = new int[rows*columns];

    //    for(int step = 1; step < rows * columns; step++)
    //    {
    //        foreach(GameObject obj in _gridArray)
    //        {
    //            if (obj&&obj.GetComponent<GridStat>().Visited == step - 1)
    //            {
    //                TestFourDirections(obj.GetComponent<GridStat>().X, obj.GetComponent<GridStat>().Y, step);
    //            }
    //        }
    //    }


    //}


    //void SetPath()
    //{
    //    int step;
    //    int x = endX;
    //    int y = endY;

    //    List<GameObject> tempList = new List<GameObject> ();

    //    path.Clear ();
    //    if (_gridArray[endX, endY] && _gridArray[endX,endY].GetComponent<GridStat>().Visited>0)
    //    {
    //        path.Add(_gridArray[x,y]);
    //        step = _gridArray[x, y].GetComponent<GridStat>().Visited -1;
    //    }
    //    else
    //    {
    //        Debug.LogError("Cant reach to the point!");
    //        return;
    //    }

    //    for(int i = step; step> -1; step--)
    //    {
    //        if (TestDirection(x, y, step, Direction.up))
    //        {
    //            tempList.Add(_gridArray[x, y + 1]);
    //        }
    //        if (TestDirection(x, y, step, Direction.right))
    //        {
    //            tempList.Add(_gridArray[x+1, y ]);
    //        }
    //        if (TestDirection(x, y, step, Direction.down))
    //        {
    //            tempList.Add(_gridArray[x, y - 1]);
    //        }
    //        if (TestDirection(x, y, step, Direction.left))
    //        {
    //            tempList.Add(_gridArray[x-1, y ]);
    //        }


    //        GameObject tempObject = FindClosestGrid(_gridArray[endX, endY].transform, tempList);
    //        path.Add(tempObject);
    //        x = tempObject.GetComponent<GridStat>().X;
    //        y = tempObject.GetComponent<GridStat>().Y;
    //        tempList.Clear();

    //    }



    //}


    //void InitialSetup()
    //{

    //    foreach (GameObject obj in _gridArray)
    //    {
    //        obj.GetComponent<GridStat>().Visited = 3;


    //    }
    //    _gridArray[startX, startY].GetComponent<GridStat>().Visited = 0;

    //}

    //bool TestDirection(int x, int y, int step, Direction direction)
    //{
    //    //in case of direction, 1 is up,2 is right,3 is down, 4 is left
    //    switch (direction)
    //    {
    //        case Direction.up:
    //            if (y + 1 <rows && _gridArray[x,y+1] && _gridArray[x,y+1].GetComponent<GridStat>().Visited==step )
    //            {
    //                return true;
    //            }
    //            else
    //            {
    //                return false;
    //            }
    //        case Direction.right:
    //            if(x + 1 < columns && _gridArray[x+1, y] && _gridArray[x+1, y].GetComponent<GridStat>().Visited == step)
    //            {
    //                return true;
    //            }
    //            else
    //            {
    //                return false;
    //            }
    //        case Direction.down:
    //            if (y - 1 >-1 && _gridArray[x, y-1] && _gridArray[x , y-1].GetComponent<GridStat>().Visited == step)
    //            {
    //                return true;
    //            }
    //            else
    //            {
    //                return false;
    //            }
    //        case Direction.left:
    //            if (x - 1 >-1 && _gridArray[x - 1, y] && _gridArray[x - 1, y].GetComponent<GridStat>().Visited == step)
    //            {
    //                return true;
    //            }
    //            else
    //            {
    //                return false;
    //            }

    //    }

    //    return false;
    //}




    //void TestFourDirections(int x,int y,int step)
    //{
    //    if (TestDirection(x, y, -1, Direction.up))
    //    {
    //        SetVisited(x, y + 1, step);
    //    }
    //    if (TestDirection(x, y, -1, Direction.right))
    //    {
    //        SetVisited(x+1, y , step);
    //    }
    //    if (TestDirection(x, y, -1, Direction.down))
    //    {
    //        SetVisited(x, y - 1, step);
    //    }
    //    if (TestDirection(x, y, -1, Direction.left))
    //    {
    //        SetVisited(x-1, y, step);
    //    }
    //}




    //void SetVisited(int x,int y,int step)
    //{
    //    if (_gridArray[x, y])
    //    {
    //        _gridArray[x,y].GetComponent<GridStat>().Visited = step;
    //    }
    //}



    //GameObject FindClosestGrid(Transform targetLocation,List<GameObject> list)
    //{
    //    float currentDistance = scale * columns * rows;
    //    int indexNumber = 0;
    //    for(int i = 0; i < list.Count; i++)
    //    {
    //        if (Vector3.Distance(targetLocation.position, list[i].transform.position)<currentDistance)
    //        {
    //            currentDistance = Vector3.Distance(targetLocation.position, list[i].transform.position);
    //            indexNumber = i;
    //        }


    //    }
    //    return list[indexNumber];
    //}



    public Vector2 WorldToGrid(Vector3 worldPosition)
    {
        // Convert world position to grid coordinates
        float gridX = (worldPosition.x - leftBottomLocation.x) / (scale + 0.675f);
        float gridY = (worldPosition.z - leftBottomLocation.z) / (scale + 0.35f);

        // Adjust grid coordinates based on hexagonal grid layout
        gridX = ReshapeNumbers(gridX);
        gridY = ReshapeNumbers(gridY);

        // Debug.Log($"Location at Grid{gridX},{gridY}");
        // Debug.Log($"Location of gridArray{_gridArray[(int)gridX, (int)gridY].transform.position}");
        if (gridY % 2 == 1)
        {
            gridX = gridX + 1;
            
        }
        if (gridX> GridSystem.instance._gridArray.GetLength(0)-1)
        {
            gridX= GridSystem.instance._gridArray.GetLength(0)-1;
        }
        if (gridY > GridSystem.instance._gridArray.GetLength(1) - 1)
        {
            gridY = GridSystem.instance._gridArray.GetLength(1) - 1;
        }

        return new Vector2(gridX, gridY);
    }


    float ReshapeNumbers(float number)
    {


        float fractionalValue = number - Mathf.Floor(number); // Calculate the fractional value

        float roundedNumber;

        if (fractionalValue < 0.5f)
        {
            roundedNumber = Mathf.Floor(number);
        }
        else
        {
            roundedNumber = Mathf.Ceil(number);
        }
        return roundedNumber;
    }




    void OnExitGrid()
    {
        IsGridOn = false;

        player.GetComponent<ThirdPersonController>().enabled = true;



        //might need to refactor this part, putting them on  a funciton
        player.GetComponent<GridInput>().enabled = false;
        player.GetComponent<GridPlayerAnimation>().enabled = false;
        gameManager.GetComponent<GridMovement>().enabled = false;
        OnGridGeneration?.Invoke();
        InputManager.OnInteractionPressed += GenerateGridOnButton;

    }

    public void InsideGridBound(GameObject[,] _gridArray)
    {

    }

    void CalculateBounds()
    {
        if (_gridArray.Length == 0 || _gridArray == null)
        {
            return;
        }

        int rowLength = _gridArray.GetLength(0);
        int columnLength = _gridArray.GetLength(1);
        // Initialize the bounds with the world position of the first GameObject in the array
        GameObject firstObject = _gridArray[0, 0];
        Vector3 worldPosition = firstObject.transform.TransformPoint(Vector3.zero);
        gridBounds = new Bounds(worldPosition, Vector3.zero);

        // Iterate through the gridArray to find the bounds
        foreach (GameObject obj in _gridArray)
        {
            // Transform the local position to world position
            worldPosition = obj.transform.TransformPoint(Vector3.zero);

            // Expand the bounds to include the world position of the current GameObject
            gridBounds.Encapsulate(worldPosition);
        }

    }

    public bool IsInGridBounds(Vector3 worldPosition)
    {
        // Check if the worldPosition is within the gridBounds
        return gridBounds.Contains(worldPosition);
    }

    void GridVisualEnable()
    {
        foreach (GameObject obj in _gridArray)
        {
            
            obj.GetComponent<GridStat>().SetGridCellColor(obj.GetComponent<GridStat>().storedGridColor);
        }
        GridMovement.instance.SetDefaultPathColor(new Color(255, 255, 255, 255));
    }
    void GridVisualDisable()
    { 
        foreach (GameObject obj in _gridArray)
        {
            obj.GetComponent<GridStat>().storedGridColor = obj.GetComponent<SpriteRenderer>().color;
            obj.GetComponent<GridStat>().SetGridCellColor(new Color(255,255,255,0));
             
        }
        GridMovement.instance.SetDefaultPathColor(new Color(255, 255, 255, 0));
    }


    public List<Vector2> GetAdjacentTiles(Vector2 gridCoordinates)
    {
        List<Vector2> adjacentTiles = new List<Vector2>();

        // Coordinates for adjacent tiles at distance 1
        Vector2[] directionsEvenY = {
            new Vector2(1, 0), new Vector2(1, -1),
            new Vector2(0, -1), new Vector2(-1, 0),
            new Vector2(1, 1), new Vector2(0, 1)
        };
        Vector2[] directionsOddY = {
            new Vector2(1, 0), new Vector2(-1, -1),
            new Vector2(0, -1), new Vector2(-1, 0),
            new Vector2(-1, 1), new Vector2(0, 1)
        };
        if (gridCoordinates.y % 2 == 0)
        {
            for (int i = 0; i < directionsEvenY.Length; i++)
            {

                Vector2 neighbor = gridCoordinates + directionsEvenY[i];
                adjacentTiles.Add(neighbor);
            }
        }
        else
        {
            for (int i = 0; i < directionsOddY.Length; i++)
            {

                Vector2 neighbor = gridCoordinates + directionsOddY[i];
                adjacentTiles.Add(neighbor);
            }
        }

        return adjacentTiles;
    }
}
