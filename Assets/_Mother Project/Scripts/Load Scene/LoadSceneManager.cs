using Cysharp.Threading.Tasks;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityEngine.Analytics;
using UnityEngine.SceneManagement;
using UnityEngine.TextCore.Text;
public class LoadSceneManager : MonoBehaviour
{
    public static LoadSceneManager instance;

    private PlayerDataSave playerDataSave;
    public List<PlayerDataSave> SaveCharacterStats = new List<PlayerDataSave>();
    private List<IPersistableData> persistableDataList;

    public String prevScene;
    bool isPrevScene;
    public List<GameObject> leftOutcharacters = new List<GameObject>();
    public bool ToAddUnlinkedCharacter;
    public bool IsnewGame = true;
    GameObject gameObjectMC;
    public int playerDefeatCounter = 0;
    private void Awake()
    {
        if(instance == null)
        {
            instance = this;
            //var d = new GameObject { name = "[LoadSceneManager]" };
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            //var d = new GameObject { name = "[LoadSceneManager]" };
            Destroy(gameObject);
        }
       
    }
    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
        SceneManager.sceneUnloaded += OnSceneUnloaded;
        SwitchMC.OnCharacterRemove += LeftOutCharacter;
        SwitchMC.OnPrevScene += LoadPrevScene;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
        SceneManager.sceneUnloaded -= OnSceneUnloaded;
        SwitchMC.OnPrevScene -= LoadPrevScene;
    }
    private void Start()
    {
         persistableDataList = FindAllIPersitableDataObjects();
        // OnSceneLoaded(SceneManager.GetActiveScene(), LoadSceneMode.Single);
    }

    public async UniTask NormalSceneLoading(string SceneName)
    {
        await SceneManager.LoadSceneAsync(SceneName, LoadSceneMode.Single).ToUniTask();
        persistableDataList = FindAllIPersitableDataObjects();
        LoadGame();
    }
    public async void LoadScene(string sceneName) // new scene load async
    {
        if (!isPrevScene)
        {
            SaveGame();
        }

        await LoadMyScene(sceneName); // Properly wait for scene load
    }

    private async UniTask LoadMyScene(string sceneName)
    {
        Debug.Log("Start" + SceneManager.GetActiveScene().name);
        await SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Single).ToUniTask();
        Debug.Log("Finish" + SceneManager.GetActiveScene().name);
        persistableDataList = FindAllIPersitableDataObjects();

        LoadGame(); // Now called AFTER scene is fully loaded

        isPrevScene = false;
    }

    /* public void SavePlayerState()
     {
         TemporaryStats playerState = new TemporaryStats(gameObject.GetComponent<TemporaryStats>().CurrentHealth, gameObject.GetComponent<TemporaryStats>().CurrentAP, gameObject.GetComponent<TemporaryStats>().CurrentDex);
         string json = JsonUtility.ToJson(playerState);
         File.WriteAllText(Application.persistentDataPath + "/playerState.json", json);
     }*/
    public void OnSceneLoaded(Scene scene, LoadSceneMode mode) // load save game data after new scene is loaded
    {
       
    }

    public void OnSceneUnloaded(Scene scene)
    {
        // SaveGame();
    }
    public void StartNewGame() //save default data when new game is started
    {
        foreach (IPersistableData player_GO in persistableDataList)
        {
            GameObject Ch_obj = ((MonoBehaviour)player_GO).gameObject;
            ShowSavedData.Instance.DefaultCharacterData(Ch_obj);      
           // LoadGame();
        }
        WeaponManager.instance.DefaultWeaponActions();
        IsnewGame = true;
    }

    public void ContinueGame()  //load previously saved data when continue game is pressed
    {
        foreach (IPersistableData player_GO in persistableDataList)
        {
            GameObject Ch_obj = ((MonoBehaviour)player_GO).gameObject;
            ShowSavedData.Instance.LoadTemporaryStatsNextScene(Ch_obj);
            if (Ch_obj.GetComponent<TemporaryStats>().isMainCharacter)
            {
                gameObjectMC = Ch_obj;
            }
        }
        LoadScene(gameObjectMC.GetComponent<TemporaryStats>().currentScene);
    }

    void LoadGame()
    {
        foreach (IPersistableData player_GO in persistableDataList)
        {
            GameObject Ch_obj = ((MonoBehaviour)player_GO).gameObject;
            ShowSavedData.Instance.LoadTemporaryStatsNextScene(Ch_obj); //load from json
            // player_GO.LoadData(playerDataSave);
            Debug.Log("Data Loaded");
            //SwitchMC.Instance.CharacterSwitch();
        }
        if (!IsnewGame)
        {
            SwitchMC.Instance.RemoveUnlinkedCharacter();
        }
        else
        {
            SwitchMC.Instance.SwitchToNextCharacter();
            //WeaponManager.instance.DefaultWeaponActions();  //in scene loading function
        }
        IsnewGame = false;

    }

    public void SaveGame() //call to save both character data
    {
        if (persistableDataList == null || persistableDataList.Count == 0)
        {
            Debug.LogError("Persistable data list is null or empty in SaveGame");
            return;
        }
        if (ToAddUnlinkedCharacter)
        {
            SwitchMC.Instance.AddUnlinkedCharacter();
            ToAddUnlinkedCharacter = false;
        }
        foreach (IPersistableData player_GO in persistableDataList) 
        {
            GameObject Ch_obj = ((MonoBehaviour)player_GO).gameObject;
            ShowSavedData.Instance.AddCharacterData(Ch_obj);//save data to json

            // player_GO.SaveData(playerDataSave);
            // SaveCharacterStats.Add(playerDataSave);
            //ShowSavedData.Instance.AddCharacterData(saveData);
        }

    }

    public List<IPersistableData> FindAllIPersitableDataObjects() //all active characters that needs data saving has IPersistable interface implimented
    {
        IEnumerable<IPersistableData> ipersistabledataObjects = FindObjectsOfType<MonoBehaviour>().OfType<IPersistableData>();
        
        return new List<IPersistableData>(ipersistabledataObjects);
    }

    public void LeftOutCharacter(GameObject leftOutCharacter)
    {
        leftOutcharacters.Clear();
        leftOutcharacters.Add(leftOutCharacter);
    }

    public void LoadPrevScene() //to load the scene of left ou character
    {
        isPrevScene = true;
        ToAddUnlinkedCharacter = true;
        SwitchMC.Instance.BackToUnlinkedCharacter();
        prevScene = leftOutcharacters[0].GetComponent<TemporaryStats>().currentScene;
        LoadScene(prevScene);
       
    }
}
