using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[DisallowMultipleComponent]
public class UIButtonSFX : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler
{
    public AudioClip hoverClip;
    public AudioClip clickClip;

    [Range(0f, 1f)] public float volumeScale = 1f;

    private Button _button;

    // ==========================================
    // LIFECYCLE
    // ==========================================

    // Caches the sibling Button (if any) and subscribes PlayClick to its onClick event.
    private void Awake()
    {
        _button = GetComponent<Button>();
        if (_button != null)
        {
            _button.onClick.AddListener(PlayClick);
        }
    }

    // Unsubscribes from the Button event to avoid leaks when the GameObject is destroyed.
    private void OnDestroy()
    {
        if (_button != null)
        {
            _button.onClick.RemoveListener(PlayClick);
        }
    }

    // ==========================================
    // POINTER EVENTS
    // ==========================================

    // Plays the hover SFX when the pointer enters, skipping if the sibling Button is non-interactable.
    public void OnPointerEnter(PointerEventData eventData)
    {
        if (_button != null && !_button.interactable) return;
        PlayHover();
    }

    // Currently unused; kept for interface symmetry if exit SFX are ever needed.
    public void OnPointerExit(PointerEventData eventData) { }

    // Fallback click handler for non-Button clickables (Button already wires PlayClick through onClick).
    public void OnPointerDown(PointerEventData eventData)
    {
        if (_button != null) return;
        if (clickClip != null) PlayClick();
    }

    // ==========================================
    // SFX PLAYBACK
    // ==========================================

    // Plays the assigned hover clip through the SoundManager.
    private void PlayHover()
    {
        if (SoundManager.Instance == null || hoverClip == null) 
        {
            Debug.Log("Missing Hover SFX");
            return;
        }  
        SoundManager.Instance.PlaySound(hoverClip, volumeScale);
    }

    // Plays the assigned click clip through the SoundManager.
    private void PlayClick()
    {
        if (SoundManager.Instance == null || clickClip == null) 
        {
            Debug.Log("Missing Hover SFX"); return;
        }
        SoundManager.Instance.PlaySound(clickClip, volumeScale);
    }
}
