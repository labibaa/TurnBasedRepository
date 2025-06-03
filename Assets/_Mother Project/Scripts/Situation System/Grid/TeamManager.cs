using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.Rendering;

public class TeamManager : MonoBehaviour
{
    public static TeamManager instance;


   
    public static Dictionary<TeamName, List<TemporaryStats>> teamPlayerLists = new Dictionary<TeamName, List<TemporaryStats>>();
    List<TeamName> currentTeams= new List<TeamName>();
    public static Dictionary<TeamName,int> currentTeamDic;



    private void OnEnable()
    {
        HealthManager.OnGridDisable += ResetPlayerTeamList;
        GridSystem.OnGridGenerationSpawn += OnStartGrid;
    }

    private void OnDisable()
    {
        HealthManager.OnGridDisable -= ResetPlayerTeamList;
        GridSystem.OnGridGenerationSpawn -= OnStartGrid;
    }

    private void Awake()
    {


        instance = this;
        OnStartGrid();
       
    }


    void OnStartGrid()
    {
        currentTeamDic = new Dictionary<TeamName, int>();
        // Initialize the dictionary with empty lists for each team
        PopulateTeamPlayerList();
    }

    public void PopulateTeamPlayerList()
    {
        foreach (TeamName teamEnum in Enum.GetValues(typeof(TeamName)))
        {
            if (!teamPlayerLists.ContainsKey(teamEnum))
            {
                teamPlayerLists.Add(teamEnum, new List<TemporaryStats>());
            }
        }
    }

    public void ResetPlayerTeamList()
    {
        teamPlayerLists.Clear();
        currentTeamDic.Clear();
       currentTeams.Clear();

    }

    // Example method to add a player to its corresponding team list
    public void AddPlayerToTeamList(TemporaryStats playerStates)
    {       

        if (playerStates != null)
        {
            teamPlayerLists[playerStates.CharacterTeam].Add(playerStates);


            if (currentTeamDic.ContainsKey(playerStates.CharacterTeam))
            {
                // Key exists, update the value
                
                
                currentTeamDic[playerStates.CharacterTeam]++;
                foreach (var kvp in currentTeamDic)
                {
                    Debug.Log($"<color=red> Dictionary Value {kvp.Value} Key {kvp.Key}</color>");
                }
            }
            else
            {
                // Key doesn't exist, add it with a value of 1
                currentTeamDic.Add(playerStates.CharacterTeam, 1);
            }




            if (!currentTeams.Contains(playerStates.CharacterTeam))
            {
                currentTeams.Add(playerStates.CharacterTeam);
            }
           
        }
    }
    public void RemovePlayerFromTeamList(TemporaryStats playerStates)
    {
        if (playerStates != null)
        {
            if (teamPlayerLists.ContainsKey(playerStates.CharacterTeam))
            {
                Debug.Log("Deleted: " + playerStates.CharacterTeam + "Value: " + currentTeamDic[playerStates.CharacterTeam]);
                currentTeamDic[playerStates.CharacterTeam]--;
              

                teamPlayerLists[playerStates.CharacterTeam].Remove(playerStates);
                Debug.Log(playerStates.gameObject.name + " removed");
            }
            else
            {
                Debug.LogWarning("Team list does not contain key: " + playerStates.CharacterTeam);
            }
        }
    }

    public void TeamMemberList(TeamName teamToCheck) //checks team list of all players
    {

        /*foreach (var kvp in teamPlayerLists)
        {
            // Print the key (TeamName)
            Debug.Log($"Team Name:   {kvp.Key}");

            if (kvp.Key == teamToCheck)
            {
                foreach (var stats in kvp.Value)
                {
                    Debug.Log("Players in " + kvp.Key + ":" + stats);
                }
            }
            foreach (var stats in kvp.Value)
            {
                Debug.Log("Player Stats:" + stats); // Assuming TemporaryStats has a meaningful ToString() implementation
            }
            
        }*/
    }
    // Example method to check if a team has no players
    public bool IsTeamEmpty(TeamName teamToCheck)
    {
       // Debug.Log("TeamNAme: "+teamToCheck+"Count: "+teamPlayerLists[teamToCheck].Count);
        return teamPlayerLists.TryGetValue(teamToCheck, out var players) && players.Count <= 0; 
    }
    public bool IsAnyTeamEmpty()
    {
        int nonZeroCount = 0;
        foreach (var kvp in currentTeamDic)
        {
            Debug.Log("SSSS"+ kvp.Value +"Key: " + kvp.Key.ToString());
            if (kvp.Value != 0)
            {
                nonZeroCount++;

                // If more than one key has a non-zero value, return false
                if (nonZeroCount > 1)
                {
                    return false;
                }
            }
        }

        return nonZeroCount == 1; // No team is empty
    }

    public void PrintDictionary()
    {
        foreach (var kvp in currentTeamDic)
        {
            Debug.Log($"<color=green> Dictionary Value {kvp.Value} Key {kvp.Key}</color>");
        }
    }

   
}
