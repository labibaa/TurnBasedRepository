using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System;
using DG.Tweening;

public class PlayableCharacterUI : MonoBehaviour
{
    public TMP_Text classText;
    public Image avatarImage;
    public GameObject CurrentPlayerPanel;
    public RectTransform thisRectTransform;

    RectTransform currentItem;
    RectTransform PrevItem;
    public Image hpBar; // HP bar using fillAmount
    public GameObject[] APImages; // Array to hold AP images

    public CharacterBaseClasses myCharacter;

    [Header("Stat Change SFX")]
    public AudioClip hpDamageSfx;
    public AudioClip hpHealSfx;

    private float _lastKnownHP = -1f;

    // ==========================================
    // LIFECYCLE
    // ==========================================

    // Runs an initial HUD refresh when the character UI spawns.
    private void Start()
    {
        UpdateHUD();
    }

    // ==========================================
    // HUD REFRESH
    // ==========================================

    // Refreshes HP bar fill, plays HP-change SFX, rebuilds the AP pips, updates the avatar, and scales the active player's HUD.
    public void UpdateHUD()
    {
        // Update HP bar
        float currentHP = myCharacter.GetComponent<TemporaryStats>().CurrentHealth;
        float maxHP = myCharacter.GetComponent<CharacterBaseClasses>().HealthPoints;
        hpBar.fillAmount = currentHP / maxHP;

        PlayHPDeltaSfx(currentHP);

        // Update AP visibility
        UpdateAPImages();

        // Update avatar image
        avatarImage.sprite = myCharacter.avatarHead;
        CurrentPlayerHUD();
    }

    // Compares current HP against the last cached value and plays damage or heal SFX on change. Seeds silently on first call.
    private void PlayHPDeltaSfx(float currentHP)
    {
        if (_lastKnownHP < 0f)
        {
            _lastKnownHP = currentHP;
            return;
        }

        if (SoundManager.Instance != null)
        {
            if (currentHP < _lastKnownHP && hpDamageSfx != null)
                SoundManager.Instance.PlaySound(hpDamageSfx);
            else if (currentHP > _lastKnownHP && hpHealSfx != null)
                SoundManager.Instance.PlaySound(hpHealSfx);
        }

        _lastKnownHP = currentHP;
    }

    // Toggles AP pip visibility so the count of enabled pips matches the character's current AP.
    private void UpdateAPImages()
    {
        int currentAP = myCharacter.GetComponent<TemporaryStats>().CurrentAP;

        for (int i = 0; i < APImages.Length; i++)
        {
            APImages[i].SetActive(i < currentAP);
        }
    }


    // ==========================================
    // ACTIVE-PLAYER HIGHLIGHT
    // ==========================================

    // Scales this HUD up if it belongs to the character whose turn it is, otherwise scales it back down.
    public void CurrentPlayerHUD()
    {
        //CurrentPlayerPanel.SetActive(myCharacter.GetComponent<PlayerTurn>().myTurn);
        if (myCharacter.GetComponent<PlayerTurn>().myTurn)
        {
            ScaleItem(thisRectTransform);
        }
        else
        {
            DescaleItem();
        }
       //SwapItemsInLayout(currentItem,PrevItem);
    }

    // Tweens the given RectTransform up to full scale with an OutBack ease to emphasize the active player.
    public void ScaleItem(RectTransform item)
    {
        if (item == null) return;

        thisRectTransform = item;
        currentItem = item;
        // Scale up the item
        thisRectTransform.DOScale(Vector3.one * 1f, 0.2f)
            .SetEase(Ease.OutBack);
    }
    // Tweens this HUD's RectTransform back down to its idle scale for non-active characters.
    private void DescaleItem()
    {
        if (thisRectTransform == null) return;
        PrevItem = thisRectTransform;
        // Scale down the item back to normal
        thisRectTransform.DOScale(Vector3.one * .65f, 0.2f)
            .SetEase(Ease.InBack);
    }

    // ==========================================
    // LAYOUT UTILITY
    // ==========================================

    // Animates two layout items into each other's anchored positions (currently unused, reserved for turn-order swaps).
    private void SwapItemsInLayout(RectTransform item1, RectTransform item2)
    {
        if (item1 == null || item2 == null) return;

        Vector3 pos1 = item1.anchoredPosition;
        Vector3 pos2 = item2.anchoredPosition;

        // Animate positions
        item1.DOAnchorPos(pos2, .1f).SetEase(Ease.InOutQuad);
        item2.DOAnchorPos(pos1, .1f).SetEase(Ease.InOutQuad);

    }
}
