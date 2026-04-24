using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using static UnityEngine.ParticleSystem;
using Coffee.UIExtensions; // UIParticle namespace

[RequireComponent(typeof(RectTransform))]
public class HoverDisplayStats : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    GameObject myCharacter;  // Now this will be set from another script
    public TextMeshProUGUI hpText;
    public TextMeshProUGUI apText;

    private TemporaryStats characterStats;
    [SerializeField] private UIParticle uiParticle;

    [SerializeField] private ParticleSystem hoverParticles;

    private ParticleSystem.EmissionModule _emission;


    void Start()
    {

        hoverParticles.Play();
    }

    // Public method to update myCharacter from another script
    public void SetCharacter(GameObject character)
    {
        myCharacter = character;
        characterStats = myCharacter.GetComponent<TemporaryStats>();  // Update the character stats when myCharacter is set
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        hoverParticles.Play(); // burst on hover
        /*    if (characterStats != null)
            {
                hpText.text = characterStats.CurrentHealth.ToString();
                apText.text = characterStats.CurrentAP.ToString();

                hpText.gameObject.SetActive(true);
                apText.gameObject.SetActive(true);
            }*/
    }

    public void OnPointerExit(PointerEventData eventData)
    {
       // _emission.rateOverTime = 3f; // back to idle

       /* hpText.gameObject.SetActive(false);
        apText.gameObject.SetActive(false);*/
    }

}
