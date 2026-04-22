using UnityEngine;
using UnityEngine.EventSystems;
using TMPro;
using System.Collections;

[RequireComponent(typeof(RectTransform))]
public class HoverDisplayStats : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public GameObject myCharacter;  // Now this will be set from another script
    public TextMeshProUGUI hpText;
    public TextMeshProUGUI apText;

    private TemporaryStats characterStats;

    [SerializeField] private ParticleSystem hoverParticles;

    private ParticleSystem.EmissionModule _emission;


    void Start()
    {
        Camera mainCam = Camera.main;

        Debug.Log("Camera Position: " + mainCam.transform.position);
        Debug.Log("Particle Position: " + hoverParticles.transform.position);

        hoverParticles.transform.position = new Vector3(
            hoverParticles.transform.position.x,
            hoverParticles.transform.position.y,
            mainCam.transform.position.z + mainCam.nearClipPlane + 1f
        );

        ParticleSystemRenderer psRenderer = hoverParticles.GetComponent<ParticleSystemRenderer>();
        psRenderer.sortingLayerName = "Default";
        psRenderer.sortingOrder = 101;

        _emission = hoverParticles.emission;
        _emission.rateOverTime = 3f;
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
        _emission.rateOverTime = 15f; // burst on hover
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
        _emission.rateOverTime = 3f; // back to idle

       /* hpText.gameObject.SetActive(false);
        apText.gameObject.SetActive(false);*/
    }

}
