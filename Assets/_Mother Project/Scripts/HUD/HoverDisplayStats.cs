using UnityEngine;
using UnityEngine.EventSystems;
using TMPro;

public class HoverDisplayStats : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public GameObject myCharacter;  // Now this will be set from another script
    public TextMeshProUGUI hpText;
    public TextMeshProUGUI apText;

    private TemporaryStats characterStats;

    // Public method to update myCharacter from another script
    public void SetCharacter(GameObject character)
    {
        myCharacter = character;
        characterStats = myCharacter.GetComponent<TemporaryStats>();  // Update the character stats when myCharacter is set
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (characterStats != null)
        {
            hpText.text = characterStats.CurrentHealth.ToString();
            apText.text = characterStats.CurrentAP.ToString();

            hpText.gameObject.SetActive(true);
            apText.gameObject.SetActive(true);
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        hpText.gameObject.SetActive(false);
        apText.gameObject.SetActive(false);
    }
}
