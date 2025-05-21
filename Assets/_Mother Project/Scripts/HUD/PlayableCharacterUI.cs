using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System;
using UnityEditorInternal.Profiling.Memory.Experimental;
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

    private void Start()
    {
        UpdateHUD();
    }

    public void UpdateHUD()
    {
        // Update HP bar
        float currentHP = myCharacter.GetComponent<TemporaryStats>().CurrentHealth;
        float maxHP = myCharacter.GetComponent<TemporaryStats>().PlayerHealth;
        hpBar.fillAmount = currentHP / maxHP;

        // Update AP visibility
        UpdateAPImages();

        // Update avatar image
        avatarImage.sprite = myCharacter.avatarHead;
        CurrentPlayerHUD();
    }

    private void UpdateAPImages()
    {
        int currentAP = myCharacter.GetComponent<TemporaryStats>().CurrentAP;

        for (int i = 0; i < APImages.Length; i++)
        {
            APImages[i].SetActive(i < currentAP);
        }
    }


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

    public void ScaleItem(RectTransform item)
    {
        if (item == null) return;

        thisRectTransform = item;
        currentItem = item;
        // Scale up the item
        thisRectTransform.DOScale(Vector3.one * 1f, 0.2f)
            .SetEase(Ease.OutBack);
    }
    private void DescaleItem()
    {
        if (thisRectTransform == null) return;
        PrevItem = thisRectTransform;
        // Scale down the item back to normal
        thisRectTransform.DOScale(Vector3.one * .65f, 0.2f)
            .SetEase(Ease.InBack);
    }

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
