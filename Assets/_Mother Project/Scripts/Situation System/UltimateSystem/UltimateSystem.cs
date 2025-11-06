using Cysharp.Threading.Tasks;
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
    
    
    
    public async UniTask useUltimate(CharacterBaseClasses player, TemporaryStats playerTemp, CharacterBaseClasses targetDefender, TemporaryStats currentStatTarget)
    {
        if (player.GetUltimateScripitable().isUltimateSingleTarget)
        {
            player.GetPlayerUltimate().setValues(player, playerTemp, targetDefender, currentStatTarget);
        }
        else
        {
            //visual cue
            List<CharacterBaseClasses> targetsInRange = GridMovement.instance.InAdjacentMatrix(playerTemp.currentPlayerGridPosition, playerTemp.CharacterTeam, player.GetUltimateScripitable().ultimateRange, Color.red);
            if (targetsInRange.Count <= 0)
            {
                ActionArchive.instance.NoTargetVisual_AOE();
            }
            else
            {
                for (int i = 0; i < targetsInRange.Count; i++)          //visual cue
                {
                    targetsInRange[i].GetComponent<TemporaryStats>().EnemyTargetSelectionParticle.SetActive(true);
                }
                Transform ct = TurnManager.instance.FindClosestTarget(TurnManager.instance.target, player);
                CharacterBaseClasses target =  ct.GetComponent<CharacterBaseClasses>();
                TemporaryStats targetTemp =  ct.GetComponent<TemporaryStats>();
                await UniTask.Delay(1500);
                for (int i = 0; i < targetsInRange.Count; i++)           //visual cue
                {
                    targetsInRange[i].GetComponent<TemporaryStats>().EnemyTargetSelectionParticle.SetActive(false);
                }
                ActionArchive.instance.TargetReset_AoE();
                player.GetPlayerUltimate().setValues(player, playerTemp, target, targetTemp);
            }    
        }
       
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
