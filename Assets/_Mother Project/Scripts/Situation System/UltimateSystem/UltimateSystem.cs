using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UltimateSystem : MonoBehaviour
{

    public static UltimateSystem _instance;
    [SerializeField]
    CharacterBaseClasses player;
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
        player.GetPlayerUltimate().setValues(player,playerTemp);
        player.GetPlayerUltimate().Execute();
        playerTemp.playerUltimateBarCount=0;
        ActionActivator.instance.UpdateAvailableAction(player, playerTemp);
        player.GetComponent<TemporaryStats>().PlayerUltimateBar.GetComponent<UltimateUI>().ResetUltimateBar();
    }
}
