using Cysharp.Threading.Tasks;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Playables;

public class WaveManager : MonoBehaviour
{
    public static WaveManager instance;

    public static event Action OnGridReady;
    public static event Action OnGridInit;

    public List<WaveWrapperClass> PlayerWaves;
    public WaveWrapperClass currentWave;

    GameObject UnLinkedCharacter;

    private void OnEnable()
    {
        GridSystem.OnGridGenerationSpawn += OnGridSpawned;
        SwitchMC.OnCharacterRemove += UnlikedRemove;
    }

    private void OnDisable()
    {
        GridSystem.OnGridGenerationSpawn -= OnGridSpawned;
        SwitchMC.OnCharacterRemove -= UnlikedRemove;
    }

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
    }

    public void IniCurrentWave(GameObject waveTrigger)
    {
        GridTrigger trigger = waveTrigger.GetComponent<GridTrigger>();
        foreach (var wave in PlayerWaves)
        {
            if (wave.WaveID == trigger.WaveTriggerID)
            {
                currentWave = wave;
                break;
            }
        }

        if (currentWave != null && currentWave.GridStartLocation != null)
        {
            GridSystem.instance.gridStartLocation = currentWave.GridStartLocation;
        }

        waveTrigger.SetActive(false);
    }

    void OnGridSpawned()
    {
        if (currentWave != null && currentWave.WaveTimeline != null)
        {
            currentWave.WaveTimeline.Play();
            StartCoroutine(WaitForTimelineToFinish(currentWave.WaveTimeline));
        }
        else
        {
            StartWave();
        }
    }

    IEnumerator WaitForTimelineToFinish(PlayableDirector timeline)
    {
        while (timeline.state == PlayState.Playing)
        {
            yield return null;
        }
        StartWave();
    }

    void StartWave()
    {
        WeaponManager.instance.LoadWeaponData();
        OnGridInit?.Invoke();
        HandleWave();
        GridSystem.instance.IsGridOn = true;
        OnGridReady?.Invoke();
    }

    void HandleWave()
    {
        TurnManager.instance.players.Clear();
        GridActivation.instance.players.Clear();

        if (currentWave == null) return;

        foreach (var waveCharacter in currentWave.CharactersOfTheWave.ToList())
        {
            if (UnLinkedCharacter != null && waveCharacter == UnLinkedCharacter.GetComponent<PlayerTurn>())
            {
                currentWave.CharactersOfTheWave.Remove(waveCharacter);
            }
        }

        HashSet<GameObject> currentPlayers = new HashSet<GameObject>();
        foreach (PlayerTurn players in currentWave.CharactersOfTheWave)
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

    public void UnlikedRemove(GameObject gameObject)
    {
        UnLinkedCharacter = gameObject;
    }

    public void MoveMatchedToFirst(WaveWrapperClass wave, PlayerTurn target)
    {
        if (wave == null || wave.CharactersOfTheWave == null)
            return;

        var list = wave.CharactersOfTheWave;

        int index = list.IndexOf(target);
        if (index <= 0)
            return;

        list.RemoveAt(index);
        list.Insert(0, target);
    }

    public void GridStartAssasinate(GameObject targetEnemy)
    {
        PlayerTurn targetTurn = targetEnemy.GetComponent<PlayerTurn>();
        foreach (var wave in PlayerWaves)
        {
            if (wave.CharactersOfTheWave.Contains(targetTurn))
            {
                currentWave = wave;
                break;
            }
        }

        if (currentWave != null)
        {
            if (currentWave.GridStartLocation != null)
            {
                GridSystem.instance.gridStartLocation = currentWave.GridStartLocation;
            }

            if (currentWave.Trigger != null)
            {
                currentWave.Trigger.SetActive(false);
            }
        }

        MoveMatchedToFirst(currentWave, SwitchMC.Instance.mainCharacter.GetComponent<PlayerTurn>());
        GridSystem.instance.GenerateGridOnButton();
    }
}
