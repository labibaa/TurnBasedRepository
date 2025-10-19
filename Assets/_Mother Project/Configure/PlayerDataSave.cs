using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class PlayerDataSave
{
    public string Name;
    public int MaxPlayerHealth;
    public int CurrentPlayerHealth;
    public int PlayerAP;
    public int CurrentDex;
    public int CurrentEndurance;
    public int CurrentStrength;
    public int CurrentArcana;
    public int CurrentIntelligence;
    public float CurrentDamageMultiplier;
    public int CurrentExp;
    public TeamName CharacterTeam;
    public bool IsMainCHaracter;
    public bool IsLinkOn;
    public string CurrentScene;
    public List<InventoryItem> Items = new List<InventoryItem>();

    public PlayerDataSave(string name,int maxHealth,int playerHealth, int playerAP, int currentDex, int currentEndurance, int currentStrength, int currentArcana, int currentIntelligence,float currentDamageMultiplier, int exp ,TeamName teamName, bool isMain, bool isLinkOn, string currentScene, List<InventoryItem> items)
    {
        Name = name;
        MaxPlayerHealth = maxHealth;
        CurrentPlayerHealth = playerHealth;
        PlayerAP = playerAP;
        CurrentDex = currentDex;
        CurrentEndurance = currentEndurance;
        CurrentStrength = currentStrength;
        CurrentArcana = currentArcana;
        CurrentIntelligence = currentIntelligence;
        CurrentDamageMultiplier = currentDamageMultiplier;
        CurrentExp = exp;
        CharacterTeam = teamName;
        IsMainCHaracter = isMain;
        IsLinkOn = isLinkOn;
        CurrentScene = currentScene;
        Items = items;
    }
}
