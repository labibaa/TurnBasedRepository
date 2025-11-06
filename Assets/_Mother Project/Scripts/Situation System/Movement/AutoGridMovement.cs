using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AutoGridMovement : MonoBehaviour
{
    public static AutoGridMovement instance;
    [SerializeField]
    GameObject start;
    [SerializeField]
    GameObject end;
    [SerializeField]
    int gridLengthX;
    int gridLengthY;

    Vector2[] neighborOffsets = new Vector2[]
          {
                new Vector2(0, 1),  // Up
                new Vector2(0, -1), // Down
                new Vector2(1, 0),  // Right
                new Vector2(-1, 0)  // Left
          };

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
    }
    private void Start()
    {
        gridLengthX = GridSystem.instance._gridArray.GetLength(0);
        gridLengthY = GridSystem.instance._gridArray.GetLength(1);
    }

    private class Node
    {
        public Vector2 position;
        public int gCost;
        public int hCost;
        public int FCost { get { return gCost + hCost; } }
        public Node parent;
    }


    // Calculate Manhattan distance heuristic
    private float CalculateManhattanDistance(Vector2 start, Vector2 target)
    {
        return Mathf.Abs(start.x - target.x) + Mathf.Abs(start.y - target.y);
    }


    public List<GameObject> FindPath(Vector2 start, Vector2 target) //find shortest path in grid 
    {

        List<GameObject> path = new List<GameObject>();

        Node startNode = new Node { position = start };
        Node targetNode = new Node { position = target };
        List<Node> openList = new List<Node>();
        HashSet<Node> closedList = new HashSet<Node>();
        openList.Add(startNode);
        while (openList.Count > 0)
        {
            Node currentNode = openList[0];
            for (int i = 1; i < openList.Count; i++)
            {
                if (openList[i].FCost < currentNode.FCost || (openList[i].FCost == currentNode.FCost && openList[i].hCost < currentNode.hCost))
                {
                    currentNode = openList[i];
                }

            }

            openList.Remove(currentNode);
            closedList.Add(currentNode);

            if (currentNode.position == targetNode.position)
            {
                // Reconstruct path
                Node pathNode = currentNode;
                while (pathNode != null)
                {

                    path.Add(GridSystem.instance._gridArray[(int)pathNode.position.x, (int)pathNode.position.y]);

                    pathNode = pathNode.parent;
                }
                path.Reverse();
                break;
            }

            // Check neighbors
            List<Node> neighbors = new List<Node>();

            foreach (Vector2 offset in neighborOffsets)
            {
                Vector2 neighborPos = currentNode.position + offset;
                if (neighborPos.x >= 0 && neighborPos.x < GridSystem.instance._gridArray.GetLength(0) && neighborPos.y >= 0 && neighborPos.y < GridSystem.instance._gridArray.GetLength(1))
                {
                    GameObject neighborObject = GridSystem.instance._gridArray[(int)neighborPos.x, (int)neighborPos.y];
                    // Check if neighbor is traversable
                    if (neighborObject.GetComponent<GridStat>().IsWalkable)
                    {
                        Node neighborNode = new Node { position = neighborPos, parent = currentNode };
                        neighbors.Add(neighborNode);
                    }
                }
            }

            foreach (Node neighbor in neighbors)
            {
                if (closedList.Contains(neighbor))
                {
                    continue;
                }

                int newCostToNeighbor = currentNode.gCost + 1; // Assuming each step has a cost of 1
                if (newCostToNeighbor < neighbor.gCost || !openList.Contains(neighbor))
                {
                    neighbor.gCost = newCostToNeighbor;
                    neighbor.hCost = (int)CalculateManhattanDistance(neighbor.position, targetNode.position);
                    neighbor.parent = currentNode;

                    if (!openList.Contains(neighbor))
                    {
                        openList.Add(neighbor);
                    }
                }
            }
        }

        //foreach(GameObject gm in path)
        //{
        //    gm.GetComponent<SpriteRenderer>().color = Color.blue;
        //}

        return path;


    }



    public Vector2 CheckClosestAdjacent(Vector3 startPosition, Vector3 targetPosition)
    {
        List<Vector2> adjacentPositions = new List<Vector2>();
        Vector2 targetCoordinate = GridSystem.instance.WorldToGrid(targetPosition);
        Vector2 playerCoordinate = GridSystem.instance.WorldToGrid(startPosition);



        for (int i = 0; i < neighborOffsets.Length; i++)
        {
            Vector2 currentTargetCoordinate = targetCoordinate + neighborOffsets[i];
            if ((currentTargetCoordinate.x < gridLengthX && currentTargetCoordinate.x >= 0) && (currentTargetCoordinate.y < gridLengthY && currentTargetCoordinate.y >= 0))
            {
                if (GridSystem.instance._gridArray[(int)currentTargetCoordinate.x, (int)currentTargetCoordinate.y].GetComponent<GridStat>().IsWalkable)
                {
                    adjacentPositions.Add(currentTargetCoordinate);
                }

            }
        }

        //for(int i = -1; i <= 1; i++)
        //{
        //    for(int j = -1; j <= 1; j++)
        //    {
        //        if(targetCoordinate.x+i<GridSystem.instance._gridArray.GetLength(0) && targetCoordinate.y+j < GridSystem.instance._gridArray.GetLength(1) && GridSystem.instance._gridArray[(int)targetCoordinate.x+i,(int)targetCoordinate.y+j].GetComponent<GridStat>().IsWalkable)
        //        {
        //            if (i!=0 && j!=0) {
        //                adjacentPositions.Add(new Vector2(targetCoordinate.x+i, targetCoordinate.y+j));
        //            }
        //        }

        //    }
        //}
        //failsafe

        Vector2 finalTarget = Vector2.zero;
        float maxDistance = int.MaxValue;
        float dis;
        foreach (Vector2 pos in adjacentPositions)
        {
            dis = Vector2.Distance(playerCoordinate, pos);
            if (dis < maxDistance)
            {
                maxDistance = dis;
                finalTarget = pos;
            }

        }

        return finalTarget;

    }

    public GameObject GetFarthestGridFromTarget(CharacterBaseClasses target)
    {
        List<Vector2> cornerHexGrids = new List<Vector2>();
        cornerHexGrids.Add(new Vector2(0, 0));
        cornerHexGrids.Add(new Vector2(0, GridSystem.instance._gridArray.GetLength(0) - 1));
        cornerHexGrids.Add(new Vector2(GridSystem.instance._gridArray.GetLength(1) - 1, 0));
        cornerHexGrids.Add(new Vector2(GridSystem.instance._gridArray.GetLength(1) - 1, GridSystem.instance._gridArray.GetLength(0) - 1));
        GameObject tempGrid = GridSystem.instance._gridArray[(int)cornerHexGrids[0].x, (int)cornerHexGrids[0].y]; ;
        float tempDistance = 0;
        foreach (Vector2 pos in cornerHexGrids)
        {

            GameObject grid = GridSystem.instance._gridArray[(int)pos.x, (int)pos.y];
            float distanceFromTarget = Vector3.Distance(grid.transform.position, target.transform.position);
            if (tempDistance < distanceFromTarget)
            {
                tempDistance = distanceFromTarget;
                tempGrid = grid;
            }
        }
        return tempGrid;
    }



}