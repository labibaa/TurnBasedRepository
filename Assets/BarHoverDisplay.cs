using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;

public class BarHoverDisplay : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public Image bar; // Reference to the bar (HP or AP)
    public TMP_Text hoverText; // Reference to the text that will display the current value

    private PlayableCharacterUI characterUI; // Reference to the PlayableCharacterUI script

    private void Start()
    {
        characterUI = GetComponentInParent<PlayableCharacterUI>();
        hoverText.gameObject.SetActive(false); // Ensure the text is hidden by default
    }

    // This function is triggered when the pointer enters the bar area
    public void OnPointerEnter(PointerEventData eventData)
    {
        //hoverText.gameObject.SetActive(true); // Show the text
        //UpdateHoverText(); // Update the text with current values
    }

    // This function is triggered when the pointer exits the bar area
    public void OnPointerExit(PointerEventData eventData)
    {
        hoverText.gameObject.SetActive(false); // Hide the text when not hovering
    }

    // Update the hover text with the current value of HP or AP
    //private void UpdateHoverText()
    //{
    //    if (bar == characterUI.hpBar)
    //    {
    //        float currentHP = characterUI.myCharacter.GetComponent<TemporaryStats>().CurrentHealth;
    //        hoverText.text = "HP: " + currentHP.ToString("0");
    //    }
    //    else if (bar == characterUI.apBar)
    //    {
    //        float currentAP = characterUI.myCharacter.GetComponent<TemporaryStats>().CurrentAP;
    //        hoverText.text = "AP: " + currentAP.ToString("0");
    //    }
    //}
}
