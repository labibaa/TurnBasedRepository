using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using Xamin;

public class InputHandlerForSaving : MonoBehaviour
{
    [SerializeField] string userName;
    [SerializeField] string fileName; // use the name wanted to save the file and add .json extention
    [SerializeField] TMP_InputField userNameInput;
 
    public List<SaveTurnInformation> toSaveData = new List<SaveTurnInformation>(); //put the list that needs to be saved in json
    public List<SaveTurnInformation> SavedData = new List<SaveTurnInformation>(); 
    
    private void Start()
    {
        userName = System.Environment.UserName;
        int runCount = PlayerPrefs.GetInt("GameRunCount", 0);
        runCount++;
        PlayerPrefs.SetInt("GameRunCount", runCount);
        PlayerPrefs.Save();
        fileName= userName+ fileName + runCount.ToString() +".json";
    }

    public void SaveTurnToJson() //call function to save data
    {
        
        FileHandler.SaveToJsonData<SaveTurnInformation>(toSaveData,fileName);

    }

    public void LoadDataFromJson()
    {
        SavedData = FileHandler.LoadJsonData<SaveTurnInformation>(fileName);       
    }

    public void setUserName()
    {
        fileName = userNameInput.text;
        
        userNameInput.gameObject.SetActive(false);
    }

    public String GetFileName { get => fileName;}
}
