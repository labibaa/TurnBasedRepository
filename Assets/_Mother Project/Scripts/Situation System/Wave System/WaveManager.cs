using Cysharp.Threading.Tasks;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.TextCore.Text;

public class WaveManager : MonoBehaviour
{
    public static WaveManager instance;

    [SerializeField]
    public static event Action OnGridReady;
    public static event Action OnGridInit;

    public List<WaveWrapperClass> PlayerWaves;
    public List<GameObject> WaveTriggers;
    public List<PlayableDirector> WaveTimelines; // New: List of PlayableDirector for each wave
    public List<GameObject> gridWaveStartLocation;
    GameObject UnLinkedCharacter;

    int TotalNumberOfWavesThisScene;
    int currentWaveCount = 0;

    private void OnEnable()
    {
        GridSystem.OnGridGenerationSpawn += IncreaseWaveCount;
        GridSystem.OnGridPositionInitialization += GridWaveStartLocation;
        HealthManager.OnGridDisable += EnableNewTrigger;
        // GridSystem.OnGridGeneration += ActivateWaveCharacters;
        SwitchMC.OnCharacterRemove += UnlikedRemove;

    }

    private void OnDisable()
    {
        GridSystem.OnGridGenerationSpawn -= IncreaseWaveCount;
        GridSystem.OnGridPositionInitialization -= GridWaveStartLocation;
        HealthManager.OnGridDisable -= EnableNewTrigger;
        // GridSystem.OnGridGeneration -= ActivateWaveCharacters;
        SwitchMC.OnCharacterRemove -= UnlikedRemove;
    }

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
    }

    void Start()
    {
        TotalNumberOfWavesThisScene = PlayerWaves.Count;
    }


    void GridWaveStartLocation()
    {
       // MoveMatchedToFirst(PlayerWaves[ currentWaveCount], SwitchMC.Instance.mainCharacter.GetComponent<PlayerTurn>());
        GridSystem.instance.gridStartLocation = gridWaveStartLocation[currentWaveCount];
    }

    void IncreaseWaveCount()
    {
        if (currentWaveCount != 0)
        {
            WaveTriggers[currentWaveCount].SetActive(false);
        }

        // Play the timeline associated with the current wave
        if (currentWaveCount < WaveTimelines.Count)
        {
            PlayWaveTimeline();
        }
        else
        {
            StartWave(); // Start the wave if there's no timeline available
        }
    }

    void PlayWaveTimeline()
    {
        PlayableDirector timeline = WaveTimelines[currentWaveCount];
        if (timeline != null)
        {
            timeline.Play();
            // Wait for the timeline to finish before starting the wave
            StartCoroutine(WaitForTimelineToFinish(timeline));
        }
        else
        {
            StartWave(); // If no timeline is assigned, start the wave immediately
        }
    }

    IEnumerator WaitForTimelineToFinish(PlayableDirector timeline)
    {
        while (timeline.state == PlayState.Playing)
        {
            yield return null; // Wait until the timeline has finished playing
        }
        StartWave(); // Start the wave after the timeline finishes
    }

    void StartWave()
    {

        WeaponManager.instance.LoadWeaponData();
        OnGridInit?.Invoke();
        currentWaveCount++;

        HandleWave();
        GridSystem.instance.IsGridOn = true;
        OnGridReady?.Invoke();
    }

    void EnableNewTrigger()
    {
        if (currentWaveCount < TotalNumberOfWavesThisScene)
        {
            WaveTriggers[currentWaveCount].SetActive(true);
        }
    }

    void HandleWave() //enemy wave system handle
    {
      
        // List<GameObject> playableC = new List<GameObject>();
        TurnManager.instance.players.Clear();
        GridActivation.instance.players.Clear();
        if (TotalNumberOfWavesThisScene >= currentWaveCount)
        {
            foreach (var waveCharacter in PlayerWaves[currentWaveCount - 1].CharactersOfTheWave.ToList())
            {
                if (UnLinkedCharacter != null && waveCharacter == UnLinkedCharacter.GetComponent<PlayerTurn>())
                {
                    Debug.Log(waveCharacter);
                    PlayerWaves[currentWaveCount - 1].CharactersOfTheWave.Remove(waveCharacter);
                }
            }
            HashSet<GameObject> currentPlayers = new HashSet<GameObject>();
            foreach (PlayerTurn players in PlayerWaves[currentWaveCount - 1].CharactersOfTheWave)
            {
                StartCoroutine(players.GetComponent<TemporaryStats>().ReStartCharacter());
                TurnManager.instance.players.Add(players);
                currentPlayers.Add(players.gameObject);
                GridActivation.instance.players.Add(players.gameObject);          
            }
            GridActivation.instance.HandleCharacterSpawn();

            foreach (GameObject player in currentPlayers)
            {
                player.GetComponent<TemporaryStats>().AssignSpawnPosition();
            }
        }
    }

    public void UnlikedRemove(GameObject gameObject)
    {
        UnLinkedCharacter = gameObject;
    }

    public void MoveMatchedToFirst(WaveWrapperClass wave, PlayerTurn target) //1st player for grid
    {
        if (wave == null || wave.CharactersOfTheWave == null)
            return;

        var list = wave.CharactersOfTheWave;

        int index = list.IndexOf(target);
        if (index <= 0)
            return; // not found or already first

        list.RemoveAt(index);
        list.Insert(0, target);
    }
    public void GridStartAttackAnimation(GameObject Attacker)
    {
       // await CutsceneManager.instance.PlayAnimationForCharacter(Attacker, "Fall on back");
        MoveMatchedToFirst(PlayerWaves[currentWaveCount], SwitchMC.Instance.mainCharacter.GetComponent<PlayerTurn>());
    }
}
