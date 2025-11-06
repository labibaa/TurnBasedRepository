using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class TextHoverEffect : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public TMP_Text textMeshPro; // Assign this in the Inspector
    public Color hoverColor = Color.red; // Set your desired hover color in the Inspector
    public float hoverFontSize = 40f; // Set your desired font size when hovered in the Inspector

    private Color originalColor;
    private float originalFontSize;

    private void Start()
    {
        if (textMeshPro == null)
        {
            textMeshPro = GetComponentInChildren<TMP_Text>();
        }

        if (textMeshPro != null)
        {
            originalColor = textMeshPro.color;
            originalFontSize = textMeshPro.fontSize;
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (textMeshPro != null)
        {
            textMeshPro.color = hoverColor;
            textMeshPro.fontSize = hoverFontSize;
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (textMeshPro != null)
        {
            textMeshPro.color = originalColor;
            textMeshPro.fontSize = originalFontSize;
        }
    }
}
