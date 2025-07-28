using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UltimateSystem : MonoBehaviour
{

    public static UltimateSystem _instance;
    [SerializeField]
    CharacterBaseClasses player;
    public bool IsUltimate;
    // Start is called before the first frame update

    private void Awake()
    {
        if (_instance==null)
        {
            _instance = this;
        }
    }
 
    public bool checkUltimateAvailability(CharacterBaseClasses player,TemporaryStats playerTemp)
    {
        //return player.GetPlayerUltimate().GetultimateThreshold() <= playerTemp.playerUltimateBarCount;//in case of action count being the ultimate decider
        return player.GetPlayerUltimate().GetultimateThreshold() <= playerTemp.playerUltimateBarCount;
    }
    
    
    
    public void useUltimate(CharacterBaseClasses player, TemporaryStats playerTemp, CharacterBaseClasses targetDefender, TemporaryStats currentStatTarget)
    {
        player.GetPlayerUltimate().setValues(player,playerTemp,targetDefender,currentStatTarget);
        ICommand ultiCommand = player.GetPlayerUltimate() as ICommand;
        Turn turn = new Turn(player, ultiCommand , 0);
        HandleTurnNew.instance.AddTurn(turn);
        ActionArchive.instance.isTurnAdded = true;
        IsUltimate = false;
        /*        player.GetPlayerUltimate().ExecuteUltimate();
                playerTemp.playerUltimateBarCount=0;
                ActionActivator.instance.UpdateAvailableAction(player, playerTemp);
                player.GetComponent<TemporaryStats>().PlayerUltimateBar.GetComponent<UltimateUI>().ResetUltimateBar();*/
    }
}
