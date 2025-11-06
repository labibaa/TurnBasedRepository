using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SaveGameInteractor : MonoBehaviour,IInteractable
{
    private List<IPersistableData> persistableDataList;
    private void Start()
    {
        persistableDataList = LoadSceneManager.instance.FindAllIPersitableDataObjects();
       
    }
    public void Interact()
    {
        SaveGameData();
    }

    public void SaveGameData()
    {
        foreach (IPersistableData player_GO in persistableDataList)
        {
            GameObject Ch_obj = ((MonoBehaviour)player_GO).gameObject;
            ShowSavedData.Instance.AddCharacterData(Ch_obj);
        }
    }

}
