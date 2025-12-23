using StarterAssets;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class GridActivation : MonoBehaviour
{

    public static GridActivation instance;
    [SerializeField]
    public List<GameObject> players = new List<GameObject>();
    [SerializeField]
    List<GameObject> uiGameObjects = new List<GameObject>();
    [SerializeField]
    List<GameObject> gameObjectsTobeDisabled = new List<GameObject>();
    [SerializeField]
    List<GameObject> gameObjectsTobeEnabled = new List<GameObject>();

    public List<GameObject> playableCharacter = new List<GameObject>();
    [SerializeField]
    GameManager gameManager;
    [SerializeField]
    GameObject grid;
    [SerializeField]
    GameObject miniStatUI;
    [SerializeField]
    GameObject cameraController;


    [SerializeField]
    GameObject gridAudio;

    public CanvasGroup blackScreen;


    public TeamName myTeam;
   // int count =0;

    //Enable and disable Grid system and corresponding UI with necessary components with it
    private void OnEnable()
    {
        WaveManager.OnGridReady += EnableSituaionUI;
        HealthManager.OnGridDisable += DisableSituationSystem;
       // GridSystem.OnGridGeneration += HandleCharacterSpawn;
        HealthManager.OnGridDisable += DisableSituaionUI;
        HealthManager.OnGridDisable += HandleCharacterDeSpawn ;
       // cameraController.SetActive(true);
    }
    private void OnDisable()
    {
        WaveManager.OnGridReady -= EnableSituaionUI;
        HealthManager.OnGridDisable -= DisableSituationSystem;
        //GridSystem.OnGridGeneration -= HandleCharacterSpawn;
        HealthManager.OnGridDisable -= DisableSituaionUI;
        HealthManager.OnGridDisable -= HandleCharacterDeSpawn;
        //cameraController.SetActive(false);
    }
//Failsafe2


    private void OnTriggerEnter(Collider other)
    {
        playableCharacter = SwitchMC.Instance.characters;
        if (other.CompareTag("Player") && !GridSystem.instance.IsGridOn)
        {
            if (gameObject.CompareTag("ObjectiveGrid"))
            {
                TempManager.instance.IsObjective = true;
            }
            else
            {
                TempManager.instance.IsObjective = false;
            }
            GridSystem.instance.GenerateGridOnButton();
        }
    }


    private void Awake()
    {
        instance = this;
    }

    private async void EnableSituaionUI()
    {
        //await UI.instance.AnimatePanelAsync();
        foreach (GameObject ui in uiGameObjects)
        {
            ui.SetActive(true);
            //StartCoroutine(BlackScreenTransition());

        }

        TurnManager.instance.StartTurn();
        PlayerStatUI.instance.CreateSummaryList();
        DisableUIObjects();
        gridAudio.GetComponent<AudioSource>().enabled = true;
        gridAudio.GetComponent<AudioSource>().Play();
        this.GetComponent<BoxCollider>().enabled = false;

    }

    private IEnumerator BlackScreenTransition()
    {
        // Fade in (0 → 0.2)
        for (float t = 0; t < 0.2f; t += Time.deltaTime)
        {
            blackScreen.alpha = t;
            yield return null;
        }
        blackScreen.alpha = 1;

        // Hold for 1.5s
        yield return new WaitForSeconds(1.5f);

        // Fade out (1 → 0.2)
        for (float t = 0; t < 0.2f; t += Time.deltaTime)
        {
            blackScreen.alpha = 1 - t;
            yield return null;
        }
        blackScreen.alpha = 0;
    }


    private async void DisableSituaionUI()
    {
        //await UI.instance.AnimatePanelAsync();
        foreach (GameObject ui in uiGameObjects)
        {
            ui.SetActive(false);
        }
       
        
        //PlayerStatUI.instance.CreateSummaryList();
        EnableUIObjects();
        gridAudio.GetComponent<AudioSource>().Stop();
        gridAudio.GetComponent<AudioSource>().enabled = false;
       

    }




    private void DisableSituationSystem()
    {
        foreach (Transform child in grid.transform)
        {
            // Destroy the child GameObject
            Destroy(child.gameObject);
        }

        //Transform[] children = miniStatUI.GetComponentsInChildren<Transform>();



        foreach (Transform child in miniStatUI.transform)
        {
            Destroy(child.gameObject);
        }



        foreach(GameObject Pc in playableCharacter)
        {
            Pc.SetActive(true);
            //playableCharacter.GetComponent<CharacterController>().enabled = true;
            Pc.GetComponent<ThirdPersonController>().enabled = true;
            Pc.GetComponent<ThirdPersonController>().DisableAnim();
            //player.GetComponent<PlayerMove>().enabled = true;
            Pc.GetComponent<TemporaryStats>().SelectionParticle.SetActive(false);

            //might need to refactor this part, putting them on  a funciton
            //player.GetComponent<GridInput>().enabled = true;
            Pc.GetComponent<GridPlayerAnimation>().enabled = false;
        }

        Debug.Log("Grid Finish");
        gameManager.GetComponent<GridMovement>().enabled = false;
        gameManager.GetComponent<TurnManager>().enabled = false;


        TempManager.instance.SituationUIPanel.SetActive(false);
        TempManager.instance.UlimateUIPanel.SetActive(false);

        GridSystem.instance.IsGridOn = false;
        // HandleTurnNew.instance.SituationEndCondition = false;
        LoadSceneManager.instance.SaveGame();
        LoadSceneManager.instance.GameDataLoad();
        WaitDelay(2f);
        SwitchMC.Instance.CharacterSwitch();

    }
    public void HandleCharacterSpawn()
    {
        foreach (GameObject p in players)
        {
            p.SetActive(true);
        }

        foreach (GameObject p in gameObjectsTobeEnabled)
        {
            p.SetActive(true);
        }
    }

    private void HandleCharacterDeSpawn()
    {
        foreach (GameObject p in players)
        {
             p.SetActive(false);
           
        }

        foreach (GameObject p in gameObjectsTobeEnabled)
        {
            p.SetActive(false);
        }
    }


    private void DisableUIObjects()
    {
        foreach (GameObject gameObjectsUI in gameObjectsTobeDisabled)
        {
            gameObjectsUI.SetActive(false);
        }
        
    }

    private void EnableUIObjects()
    {
        foreach (GameObject gameObjectsUI in gameObjectsTobeDisabled)
        {
            gameObjectsUI.SetActive(true);
        }
    }


    IEnumerator WaitDelay(float time)
    {
      
        yield return new WaitForSeconds(time);

    }


}
