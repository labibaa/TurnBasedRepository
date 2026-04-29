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

    [Header("Active-Turn Swap")]
    [SerializeField] private float swapDuration = 0.3f;
    [SerializeField] private Ease swapEase = Ease.InOutQuad;

    Vector3 positionOffset = new Vector3(0, 2, 0);
    public static PlayerStatUI instance;

    private Sequence activeSwap;

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
           // DisableUIParticles(child);
            Destroy(child.gameObject);
        }
        
        foreach (Transform child in SummaryStatParentEnemy.transform)
        {
            //DisableUIParticles(child);
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
                EnsureAvatarHoverEffect(charUI);
                CharacterUIList.Add(charUI);
            }
            else
            {
                tempAvatarUI = Instantiate(AvatarSummaryPrefabEnemy.transform, SummaryStatParentEnemy.transform);
                PlayableCharacterUI charUI = tempAvatarUI.GetComponent<PlayableCharacterUI>();
                charUI.myCharacter = player;
                EnsureAvatarHoverEffect(charUI);
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
    public void PromoteActiveToFront(PlayableCharacterUI activeChar)
    {
        if (activeChar == null || SummaryStatParent == null) return;
        RectTransform activeRT = activeChar.transform as RectTransform;
        if (activeRT == null) return;

        Transform parent = activeRT.parent;
        if (parent != SummaryStatParent.transform) return;
        if (parent.childCount < 2) return;

        if (activeSwap != null && activeSwap.IsActive())
            activeSwap.Complete(true);

        if (activeRT.GetSiblingIndex() == 0) return;

        RectTransform frontRT = parent.GetChild(0) as RectTransform;
        if (frontRT == null || frontRT == activeRT) return;

        SwapItemsInLayout(activeRT, frontRT);
    }

    public void SwapItemsInLayout(RectTransform a, RectTransform b)
    {
        if (a == null || b == null || a == b) return;

        Transform parent = a.parent;
        if (parent == null || parent != b.parent) return;

        if (activeSwap != null && activeSwap.IsActive())
            activeSwap.Complete(true);

        Vector2 startA = a.anchoredPosition;
        Vector2 startB = b.anchoredPosition;

        LayoutGroup layout = parent.GetComponent<LayoutGroup>();
        bool layoutWasEnabled = layout != null && layout.enabled;
        if (layout != null) layout.enabled = false;

        bool restored = false;
        System.Action restore = () =>
        {
            if (restored) return;
            restored = true;

            int idxA = a.GetSiblingIndex();
            int idxB = b.GetSiblingIndex();
            a.SetSiblingIndex(idxB);
            b.SetSiblingIndex(idxA);

            if (layout != null) layout.enabled = layoutWasEnabled;
            if (parent is RectTransform pr) LayoutRebuilder.ForceRebuildLayoutImmediate(pr);
        };

        Sequence swap = DOTween.Sequence();
        swap.Join(a.DOAnchorPos(startB, swapDuration).SetEase(swapEase));
        swap.Join(b.DOAnchorPos(startA, swapDuration).SetEase(swapEase));
        swap.OnComplete(() => restore());
        swap.OnKill(() => restore());
        activeSwap = swap;
    }

    public void GetPlayerStatDetails(CharacterBaseClasses currentPlayer)
    {
        playerAvatarDetails.sprite = currentPlayer.avatarHead;
    }

    private static void EnsureAvatarHoverEffect(PlayableCharacterUI charUI)
    {
        if (charUI == null || charUI.avatarImage == null) return;
        if (!charUI.avatarImage.raycastTarget) charUI.avatarImage.raycastTarget = true;
        if (charUI.avatarImage.GetComponent<AvatarHoverEffect>() == null)
            charUI.avatarImage.gameObject.AddComponent<AvatarHoverEffect>();
    }
}
