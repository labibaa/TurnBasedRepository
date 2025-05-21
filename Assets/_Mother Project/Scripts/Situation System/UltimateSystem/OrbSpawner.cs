using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OrbSpawner : MonoBehaviour
{
    public static OrbSpawner instance;
    int gridSizeX;
    int gridSizeY;
    [SerializeField] GameObject orb;
    [SerializeField] GameObject spawnedOrb;
    [SerializeField] GameObject smokeGameObject;
    [SerializeField] GameObject SkeletonGrabGameObject;
    int readyPlayer=0;

    // Start is called before the first frame update`````
    private void Awake()
    {

        if (instance != null)
        {
            Debug.LogWarning("Found more than one OrbSpawner in the scene");
        }
        instance = this;

      

    }
    private void OnEnable()
    {
        GridSystem.OnGridGenerationSpawn += ResetReadyCounter;
    }

    private void OnDisable()
    {
        GridSystem.OnGridGenerationSpawn -= ResetReadyCounter;
    }

    public void PlayerReadyCounter()
    {
        readyPlayer++;
        if (readyPlayer>=TurnManager.instance.players.Count)
        {
           // SpawnObject();
           
        }
    }


    public  void SpawnObject()
    {
        gridSizeX = GridSystem.instance._gridArray.GetLength(0);
        gridSizeY = GridSystem.instance._gridArray.GetLength(1);
        Destroy(spawnedOrb);
        // Generate a random position within the grid
        int randomX = Random.Range(0,gridSizeX);
        int randomY = Random.Range(0, gridSizeY);

        // Check if the position is occupied
        if (!GridSystem.instance._gridArray[randomX,randomY].GetComponent<GridStat>().IsOccupied && !GridSystem.instance._gridArray[randomX, randomY].GetComponent<GridStat>().HasOrb)
        {
           

            // Spawn the object at the random position
            spawnedOrb = Instantiate(orb,GridSystem.instance._gridArray[randomX, randomY].transform.position, Quaternion.identity);

            // Update grid stats to mark this position as occupied
            GridSystem.instance._gridArray[randomX, randomY].GetComponent<GridStat>().HasOrb= true;


        }
        else
        {
            // If the position is occupied, try again
            SpawnObject();
        }
    }


    public GameObject SpawnSmoke(Transform transform)
    {
        return Instantiate(SkeletonGrabGameObject,transform.position + new Vector3(0f,1f,0f),Quaternion.identity);
    }

    void ResetReadyCounter()
    {
        readyPlayer = 0;
    }
}
