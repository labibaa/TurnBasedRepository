using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DemoGameMAnger : MonoBehaviour
{
 
    private enum GameState
    {
        PlayerTurn,
        EnemyTurn,
        Simulation
    }

    private GameState currentState;
    private List<ActionCommand> playerCommands;
    private List<ActionCommand> enemyCommands;
    private int playerAP;

    private void Start()
    {
       // InitializeGame();
    }

    private void InitializeGame()
    {
        // Initialize game state and variables
        currentState = GameState.PlayerTurn;
        playerCommands = new List<ActionCommand>();
        enemyCommands = new List<ActionCommand>();
        playerAP = 5;

        // Start player's turn
        StartPlayerTurn();
    }

    private void StartPlayerTurn()
    {
        Debug.Log("Player's Turn");

        // TODO: Implement player actions and selection logic here
        // Allow the player to select actions and add them to playerCommands list
        // Ensure the player doesn't exceed the AP limit and handles AP correctly

        // Once player has finished selecting actions, move to the next state
        currentState = GameState.EnemyTurn;
        StartEnemyTurn();
    }

    private void StartEnemyTurn()
    {
        Debug.Log("Enemy's Turn");

        // TODO: Implement enemy actions and selection logic here
        // Allow the enemy to select actions and add them to enemyCommands list
        // Ensure the enemy handles AP correctly

        // Once enemy has finished selecting actions, move to the next state
        currentState = GameState.Simulation;
        StartSimulation();
    }

    private void StartSimulation()
    {
        Debug.Log("Simulation");

        // TODO: Implement simulation logic here
        // Simulate the stacked commands of the players and enemies simultaneously
        // Apply the commands, resolve actions, calculate outcomes, etc.

        // Once simulation is complete, move to the next state or end the game
        // You can add logic here to determine the next state or end conditions
        // For example, if there are more rounds to play, go to the player's turn again
        // Otherwise, end the game and show the results
    }

    // Example function to add player action command to the stack
    public void AddPlayerActionCommand(ActionCommand command)
    {
        playerCommands.Add(command);
        playerAP -= command.APCost;
    }

    // Example function to add enemy action command to the stack
    public void AddEnemyActionCommand(ActionCommand command)
    {
        enemyCommands.Add(command);
    }

    // Example class to represent an action command
    public class ActionCommand
    {
        public ActionType Type { get; set; }
        public int APCost { get; set; }
        // Add more properties as needed

        public ActionCommand(ActionType type, int apCost)
        {
            Type = type;
            APCost = apCost;
        }
    }

    // Example enum for action types
    public enum ActionType
    {
        Kill,
        Survive,
        Deal
    }


}
