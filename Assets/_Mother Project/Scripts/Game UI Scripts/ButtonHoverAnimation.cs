using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using DG.Tweening;
using TMPro;

public class ButtonHoverAnimation : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public float hoverScale = 1.2f;
    public Color hoverColor = new Color(66 / 255f, 121 / 255f, 255 / 255f, 1f);
    private Vector3 initialScale;
    private Color initialColor;
    private Image buttonImage;
    public Image buttonHoverImage;
    private TextMeshProUGUI hoverText; // Reference to the TextMeshPro component


    private void Awake()
    {
        hoverText = GetComponentInChildren<TextMeshProUGUI>();
    }
    private void Start()
    {
        initialScale = transform.localScale;

        buttonImage = GetComponent<Image>();
        if (buttonImage != null)
        {
            initialColor = buttonImage.color;
        }

    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        transform.DOScale(initialScale * hoverScale, 0.3f)
            .SetEase(Ease.OutBack);

        if (buttonImage != null)
        {
            initialColor = buttonImage.color;
            buttonImage.DOColor(hoverColor, 0.3f);
        }
        if (buttonHoverImage != null) 
        {
            buttonHoverImage.gameObject.SetActive(true);
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        transform.DOScale(initialScale, 0.3f)
            .SetEase(Ease.OutBack);

        if (buttonImage != null)
        {
            buttonImage.DOColor(initialColor, 0.3f);
        }
        if (buttonHoverImage != null)
        {
            buttonHoverImage.gameObject.SetActive(false);
        }
    }
    public GameObject GetHoverText()
    {
        return hoverText.gameObject;
    }
}
