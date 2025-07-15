using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using DG.Tweening;
using TMPro;

public class ButtonHoverAnimation : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public float hoverScale = 1.2f;
    //public float hoverOffset = 20f;
    public Color hoverColor = new Color(66 / 255f, 121 / 255f, 255 / 255f, 1f); // Hex: 4279FF
    private Vector3 initialScale;
    private Vector3 initialPosition;
    private Color initialColor;
    private Image buttonImage;
    private TextMeshProUGUI hoverText; // Reference to the TextMeshPro component


    private void Awake()
    {
        hoverText = GetComponentInChildren<TextMeshProUGUI>();
    }
    private void Start()
    {
        // Store the initial scale, position, and color
        initialScale = transform.localScale;
        initialPosition = transform.localPosition;

        buttonImage = GetComponent<Image>();
        if (buttonImage != null)
        {
            initialColor = buttonImage.color;
        }

        // Find the TextMeshPro component as a child of the button
        
        //if (hoverText != null)
        //{
        //    hoverText.gameObject.SetActive(false); // Initially disable the TextMeshPro component
        //}
    }

    private void Update()
    {
        
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
       
        // Scale up and move up animation on hover
        transform.DOScale(initialScale * hoverScale, 0.3f)
            .SetEase(Ease.OutBack);

        //transform.DOLocalMoveY(initialPosition.y + hoverOffset, 0.3f)
        //    .SetEase(Ease.OutBack);

        // Change the color on hover
        if (buttonImage != null)
        {
            initialColor= buttonImage.color;
            buttonImage.DOColor(hoverColor, 0.3f);
        }

        //// Enable the TextMeshPro component on hover
        //if (hoverText != null)
        //{
        //    hoverText.gameObject.SetActive(true);
        //}
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        // Reset the button to its initial state on exit
        transform.DOScale(initialScale, 0.3f)
            .SetEase(Ease.OutBack);

        //transform.DOLocalMoveY(initialPosition.y, 0.3f)
        //    .SetEase(Ease.OutBack);

        // Reset the color on exit
        if (buttonImage != null)
        {

            buttonImage.DOColor(initialColor, 0.3f);
        }

        // Disable the TextMeshPro component on exit
        //if (hoverText != null)
        //{
        //    hoverText.gameObject.SetActive(false);
        //}
    }
    public GameObject GetHoverText()
    {
        return hoverText.gameObject;
    }
}
