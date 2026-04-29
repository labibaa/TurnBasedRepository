using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[RequireComponent(typeof(Image))]
public class AvatarHoverEffect : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [Header("Scale")]
    [SerializeField] private float hoverScale = 1.08f;
    [SerializeField] private float enterDuration = 0.15f;
    [SerializeField] private float exitDuration = 0.12f;

    [Header("Color Pulse")]
    [SerializeField] private Color hoverTint = new Color(1f, 0.96f, 0.82f, 1f);

    [Header("Glow Ring (optional)")]
    [SerializeField] private Image glowRing;
    [SerializeField] private float glowAlpha = 0.75f;
    [SerializeField] private float glowStartScale = 0.85f;
    [SerializeField] private float glowEndScale = 1.15f;


    private RectTransform _rect;
    private Image _image;
    private Vector3 _baseScale;
    private Color _baseColor;

    private void Awake()
    {
        _rect = (RectTransform)transform;
        _image = GetComponent<Image>();
        _baseScale = _rect.localScale;
        _baseColor = _image.color;

        if (glowRing != null)
        {
            Color c = glowRing.color;
            c.a = 0f;
            glowRing.color = c;
        }

    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        KillTweens();

        _rect.DOScale(_baseScale * hoverScale, enterDuration).SetEase(Ease.OutBack);
        _image.DOColor(hoverTint, enterDuration);

        if (glowRing != null)
        {
            glowRing.DOFade(glowAlpha, enterDuration);
            glowRing.rectTransform.localScale = Vector3.one * glowStartScale;
            glowRing.rectTransform.DOScale(Vector3.one * glowEndScale, enterDuration).SetEase(Ease.OutQuad);
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        KillTweens();

        _rect.DOScale(_baseScale, exitDuration).SetEase(Ease.OutQuad);
        _image.DOColor(_baseColor, exitDuration);

        if (glowRing != null)
            glowRing.DOFade(0f, exitDuration);
    }

    private void OnDisable()
    {
        KillTweens();
        if (_rect != null) _rect.localScale = _baseScale;
        if (_image != null) _image.color = _baseColor;
        if (glowRing != null)
        {
            Color c = glowRing.color;
            c.a = 0f;
            glowRing.color = c;
        }
    }

    private void KillTweens()
    {
        if (_rect != null) _rect.DOKill();
        if (_image != null) _image.DOKill();
        if (glowRing != null)
        {
            glowRing.DOKill();
            glowRing.rectTransform.DOKill();
        }
    }
}
