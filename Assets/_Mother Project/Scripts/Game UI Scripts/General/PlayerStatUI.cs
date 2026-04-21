using System.Collections.Generic;
using DG.Tweening;
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
                PlayableCharacterUI charUI = tempAvatarUI.GetComponent<PlayableCharacterUI>();
                charUI.myCharacter = player;
                CharacterUIList.Add(charUI);
            }
            else
            {
                tempAvatarUI = Instantiate(AvatarSummaryPrefabEnemy.transform, SummaryStatParentEnemy.transform);
                PlayableCharacterUI charUI = tempAvatarUI.GetComponent<PlayableCharacterUI>();
                charUI.myCharacter = player;
                CharacterUIList.Add(charUI);
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

        item1.DOAnchorPos(pos2, .1f).SetEase(Ease.InOutQuad);
        item2.DOAnchorPos(pos1, .1f).SetEase(Ease.InOutQuad);

        LayoutRebuilder.ForceRebuildLayoutImmediate(SummaryStatParent.rectTransform);
    }

    public void GetPlayerStatDetails(CharacterBaseClasses currentPlayer)
    {
        playerAvatarDetails.sprite = currentPlayer.avatarHead;
    }


}
