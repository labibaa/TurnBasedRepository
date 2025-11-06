using UnityEngine;
using UnityEngine.EventSystems;
using DG.Tweening;  // Import DOTween namespace

public class ButtonScale : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public Vector3 hoverScale = new Vector3(1.2f, 1.2f, 1.2f); // Scale when hovered
    private Vector3 originalScale;
    public float tweenDuration = 0.2f; // Duration of the tween effect

    private void Start()
    {
        originalScale = transform.localScale;  // Store the initial scale
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        // Smoothly scale to the hover scale
        transform.DOScale(hoverScale, tweenDuration).SetEase(Ease.OutQuad);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        // Smoothly scale back to the original scale
        transform.DOScale(originalScale, tweenDuration).SetEase(Ease.OutQuad);
    }
}
