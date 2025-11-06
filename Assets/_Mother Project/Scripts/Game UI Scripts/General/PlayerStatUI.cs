using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerStatUI : MonoBehaviour
{
    #region summary HUD
   
    public HoverDisplayStats hoverDisplayStats;

    #endregion

    #region new Summary Stat

    public Image SummaryStatParent;
    public Image SummaryStatParentEnemy;
    public RectTransform AvatarSummaryPrefab;
    public RectTransform AvatarSummaryPrefabEnemy;
    public List<PlayableCharacterUI> CharacterUIList;
    List<RectTransform> AvatarSummaryPrefabList;

    #endregion

    [SerializeField] private Image playerStatDetailsPanel;
    [SerializeField] private Image playerAvatarDetails;
/*    [SerializeField] private TMP_Text playerAPTextDetails;
    [SerializeField] private TMP_Text playerHPTextDetails;*/
 
    Vector3 positionOffset = new Vector3(0, 2, 0);
    public static PlayerStatUI instance;

    private void Awake()
    {
        if (instance==null)
        {
            instance = this;
        }
    }

    #region summary HUD

    public void GetPlayerStatSummary(CharacterBaseClasses currentPlayer)
    {
        /*playerAvatarSummary.sprite = currentPlayer.avatarHead;
        playerNameTextSummary.text = currentPlayer.characterName;
        playerAPTextSummary.text = "AP: " + currentPlayer.GetComponent<TemporaryStats>().CurrentAP.ToString();
        playerHPTextSummary.text = "HP: " + currentPlayer.GetComponent<TemporaryStats>().CurrentHealth.ToString();
        playerRPTextSummary.text = "RP: " + currentPlayer.GetComponent<TemporaryStats>().CurrentResolve.ToString();*/
    }

    #endregion

    public void CreateSummaryList()
    {

        foreach (Transform child in SummaryStatParent.transform)
        {
            Destroy(child.gameObject);
        }
        
        foreach (Transform child in SummaryStatParentEnemy.transform)
        {
            Destroy(child.gameObject);
        }


        HashSet<CharacterBaseClasses> uiPlayerHashSet = new HashSet<CharacterBaseClasses>();
        
        foreach (PlayerTurn pt in TurnManager.instance.players)
        {
            
                uiPlayerHashSet.Add(pt.GetComponent<CharacterBaseClasses>());
            
            
        }

        foreach (CharacterBaseClasses player in uiPlayerHashSet)
        {
            
                Transform tempAvatarUI;
            if (player.GetComponent<TemporaryStats>().CharacterTeam == TeamName.TeamA)
            {
                
                
                tempAvatarUI = Instantiate(AvatarSummaryPrefab.transform, SummaryStatParent.transform);
                tempAvatarUI.GetComponent<PlayableCharacterUI>().myCharacter = player;
                CharacterUIList.Add(tempAvatarUI.GetComponent<PlayableCharacterUI>());
               // AvatarSummaryPrefabList.Add(tempAvatarUI.GetComponent<RectTransform>());
                    
                
            }
            else
            {
                tempAvatarUI = Instantiate(AvatarSummaryPrefabEnemy.transform, SummaryStatParentEnemy.transform);
                tempAvatarUI.GetComponent<PlayableCharacterUI>().myCharacter = player;
                CharacterUIList.Add(tempAvatarUI.GetComponent<PlayableCharacterUI>());
            }

           




        }
        UpdateSummaryHUDUI();
    }

    public void UpdateSummaryHUDUI()
    {
        foreach (PlayableCharacterUI summaryHUD in CharacterUIList)
        {
            summaryHUD.UpdateHUD();

        }

        
    }
    private void SwapItemsInLayout(RectTransform item1, RectTransform item2)
    {
        if (item1 == null || item2 == null) return;

        Vector3 pos1 = item1.anchoredPosition;
        Vector3 pos2 = item2.anchoredPosition;

        // Animate positions
        item1.DOAnchorPos(pos2, .1f).SetEase(Ease.InOutQuad);
        item2.DOAnchorPos(pos1, .1f).SetEase(Ease.InOutQuad);

        // Rebuild layout after the swap
        LayoutRebuilder.ForceRebuildLayoutImmediate(AvatarSummaryPrefab);

    }

    public void GetPlayerStatDetails(CharacterBaseClasses currentPlayer)
    {
        

        playerAvatarDetails.sprite = currentPlayer.avatarHead;
        //playerNameTextDetails.text = currentPlayer.characterName;
        //playerClassTextDetails.text = currentPlayer.;
/*
        playerAPTextDetails.text = "AP: " + currentPlayer.GetComponent<TemporaryStats>().CurrentAP.ToString();
        playerHPTextDetails.text = "HP: " + currentPlayer.GetComponent<TemporaryStats>().CurrentHealth.ToString();*/
     

    }


}
