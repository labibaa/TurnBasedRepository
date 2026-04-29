using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System;
using DG.Tweening;

public class PlayableCharacterUI : MonoBehaviour
{
    public TMP_Text classText;
    public Image avatarImage;
    public GameObject CurrentPlayerPanel;
    public RectTransform thisRectTransform;

    RectTransform currentItem;
    RectTransform PrevItem;
    public GameObject[] APImages; // Array to hold AP images

    public CharacterBaseClasses myCharacter;

    [Header("Stat Change SFX")]
    public AudioClip hpDamageSfx;
    public AudioClip hpHealSfx;

    [Header("Health Bar Refs")]
    public Image fill;          // main HP fill (Image Type = Filled, Horizontal)
    public Image damageGhost;   // lagging "lost HP" ghost (Image Type = Filled, Horizontal)
    public RectTransform shine; // shine highlight inside Fill
    public Image stripes;       // animated diagonal stripes panel; alpha driven by low-HP threshold

    [Header("HP Drain Timing")]
    [Tooltip("Seconds it takes the main Fill to tween to the new HP.")]
    public float fillDrainDuration = 0.28f;
    [Tooltip("Seconds the ghost fill waits before chasing the main fill.")]
    public float ghostDelay = 0.35f;
    [Tooltip("Seconds the ghost fill takes to catch up, once it starts moving.")]
    public float ghostDrainDuration = 0.55f;

    [Header("Shine Sweep")]
    [Tooltip("How often the shine sweeps across, in seconds.")]
    public float shineInterval = 3f;
    [Tooltip("How long one sweep lasts, in seconds.")]
    public float shineDuration = 0.9f;
    [Tooltip("Width of the shine as a fraction of the bar width (0..1).")]
    [Range(0.05f, 1f)] public float shineWidthFraction = 0.25f;

    [Header("Low-HP Chip Stripes")]
    [Tooltip("Below this HP fraction, the animated diagonal stripes fade in.")]
    [Range(0f, 1f)] public float lowHpThreshold = 0.4f;
    [Tooltip("Max opacity of the stripes when HP is very low.")]
    [Range(0f, 1f)] public float stripesMaxAlpha = 0.9f;

    private float _lastKnownHP = -1f;

    // HP bar internal animation state.
    private float _displayedFill;
    private float _displayedGhost;
    private bool _hpSeeded;
    private Tween _drainTween, _ghostTween;
    private Coroutine _shineCo;

    // ==========================================
    // LIFECYCLE
    // ==========================================

    // Runs an initial HUD refresh when the character UI spawns.
    private void Start()
    {
        SeedHealthBar();
        UpdateHUD();
        TryStartShineLoop();
    }

    private void OnEnable()
    {
        TryStartShineLoop();
    }

    private void TryStartShineLoop()
    {
        if (!Application.isPlaying || shine == null) return;
        if (!gameObject.activeInHierarchy) return;
        if (_shineCo != null) return;
        _shineCo = StartCoroutine(ShineLoop());
    }

    // Drives the chip-stripes opacity based on the currently displayed HP fraction.
    private void Update()
    {
        if (stripes == null) return;
        float t = 1f - Mathf.InverseLerp(0f, lowHpThreshold, Mathf.Max(_displayedFill, 0.0001f));
        Color c = stripes.color;
        c.a = Mathf.Lerp(c.a, t * stripesMaxAlpha, Time.deltaTime * 8f);
        stripes.color = c;
    }

    private void OnDisable()
    {
        // Intentionally do NOT kill _drainTween / _ghostTween — they run on DOTween's global scheduler
        // so the ghost-damage animation keeps progressing while the panel is hidden, and the player
        // catches whatever's left of it (or the final state) when the panel is shown again.
        if (_shineCo != null) { StopCoroutine(_shineCo); _shineCo = null; }
    }

    // ==========================================
    // HUD REFRESH
    // ==========================================

    // Refreshes HP bar fill, plays HP-change SFX, rebuilds the AP pips, updates the avatar, and scales the active player's HUD.
    public void UpdateHUD()
    {
        float currentHP = myCharacter.GetComponent<TemporaryStats>().CurrentHealth;
        float maxHP = myCharacter.GetComponent<CharacterBaseClasses>().HealthPoints;
        float targetFraction = SafeFraction(currentHP, maxHP);

        if (!_hpSeeded)
        {
            _displayedFill = _displayedGhost = targetFraction;
            ApplyFills();
            _hpSeeded = true;
        }
        else
        {
            TweenTo(targetFraction);
        }

        PlayHPDeltaSfx(currentHP);

        // Update AP visibility
        UpdateAPImages();

        // Update avatar image
        avatarImage.sprite = myCharacter.avatarHead;
        CurrentPlayerHUD();
    }

    // Seeds displayed fill/ghost values from the character's current HP without animating. Safe to call before Start finishes.
    private void SeedHealthBar()
    {
        if (myCharacter == null) return;
        TemporaryStats ts = myCharacter.GetComponent<TemporaryStats>();
        if (ts == null) return;
        float currentHP = ts.CurrentHealth;
        float maxHP = myCharacter.HealthPoints;
        _displayedFill = _displayedGhost = SafeFraction(currentHP, maxHP);
        ApplyFills();
        _hpSeeded = true;
    }

    // Compares current HP against the last cached value and plays damage or heal SFX on change. Seeds silently on first call.
    private void PlayHPDeltaSfx(float currentHP)
    {
        if (_lastKnownHP < 0f)
        {
            _lastKnownHP = currentHP;
            return;
        }

        if (SoundManager.Instance != null)
        {
            if (currentHP < _lastKnownHP && hpDamageSfx != null)
                SoundManager.Instance.PlaySound(hpDamageSfx);
            else if (currentHP > _lastKnownHP && hpHealSfx != null)
                SoundManager.Instance.PlaySound(hpHealSfx);
        }

        _lastKnownHP = currentHP;
    }

    // Toggles AP pip visibility so the count of enabled pips matches the character's current AP.
    private void UpdateAPImages()
    {
        int currentAP = myCharacter.GetComponent<TemporaryStats>().CurrentAP;

        for (int i = 0; i < APImages.Length; i++)
        {
            APImages[i].SetActive(i < currentAP);
        }
    }

    // ==========================================
    // HEALTH BAR ANIMATION
    // ==========================================

    // Animates the main fill to the target fraction; the ghost fill lags behind on damage and snaps up on heal.
    // Uses DOTween (global scheduler) so the animation progresses even if this GameObject is currently inactive.
    private void TweenTo(float targetFraction)
    {
        if (_drainTween != null && _drainTween.IsActive()) _drainTween.Kill();
        _drainTween = DOTween.To(
                () => _displayedFill,
                v => { _displayedFill = v; if (fill) fill.fillAmount = v; },
                targetFraction,
                fillDrainDuration)
            .SetEase(Ease.OutCubic)
            .SetLink(gameObject);

        if (targetFraction < _displayedGhost)
        {
            if (_ghostTween != null && _ghostTween.IsActive()) _ghostTween.Kill();
            _ghostTween = DOTween.To(
                    () => _displayedGhost,
                    v => { _displayedGhost = v; if (damageGhost) damageGhost.fillAmount = v; },
                    targetFraction,
                    ghostDrainDuration)
                .SetEase(Ease.OutQuart)
                .SetDelay(ghostDelay)
                .SetLink(gameObject);
        }
        else
        {
            // Healing: snap ghost up with the main fill, no lag.
            _displayedGhost = targetFraction;
            if (damageGhost) damageGhost.fillAmount = targetFraction;
        }
    }

    // Periodically sweeps the shine highlight from off-left to off-right across the fill rect.
    private IEnumerator ShineLoop()
    {
        if (shine == null) yield break;

        while (true)
        {
            yield return new WaitForSeconds(shineInterval);

            RectTransform parent = shine.parent as RectTransform;
            if (parent == null) continue;

            float parentWidth = parent.rect.width;
            float shineWidth = parentWidth * shineWidthFraction;

            Vector2 size = shine.sizeDelta;
            shine.sizeDelta = new Vector2(shineWidth, size.y);

            float startX = -shineWidth;
            float endX = parentWidth + shineWidth;

            float t = 0f;
            while (t < shineDuration)
            {
                t += Time.deltaTime;
                float k = EaseInOutSine(Mathf.Clamp01(t / shineDuration));
                float x = Mathf.Lerp(startX, endX, k);
                shine.anchoredPosition = new Vector2(x, shine.anchoredPosition.y);
                yield return null;
            }
        }
    }

    private void ApplyFills()
    {
        if (fill) fill.fillAmount = _displayedFill;
        if (damageGhost) damageGhost.fillAmount = _displayedGhost;
    }

    private static float SafeFraction(float a, float b) => b <= 0f ? 0f : Mathf.Clamp01(a / b);
    private static float EaseInOutSine(float x) => -(Mathf.Cos(Mathf.PI * x) - 1f) / 2f;

    // ==========================================
    // ACTIVE-PLAYER HIGHLIGHT
    // ==========================================

    // Scales this HUD up if it belongs to the character whose turn it is, otherwise scales it back down.
    public void CurrentPlayerHUD()
    {
        //CurrentPlayerPanel.SetActive(myCharacter.GetComponent<PlayerTurn>().myTurn);
        if (myCharacter.GetComponent<PlayerTurn>().myTurn)
        {
            ScaleItem(thisRectTransform);
            if (PlayerStatUI.instance != null)
                PlayerStatUI.instance.PromoteActiveToFront(this);
        }
        else
        {
            DescaleItem();
        }
    }

    // Tweens the given RectTransform up to full scale with an OutBack ease to emphasize the active player.
    public void ScaleItem(RectTransform item)
    {
        if (item == null) return;

        thisRectTransform = item;
        currentItem = item;
        thisRectTransform.DOScale(Vector3.one * 0.85f, 0.2f)
            .SetEase(Ease.OutBack);
    }

    // Tweens this HUD's RectTransform back down to its idle scale for non-active characters.
    private void DescaleItem()
    {
        if (thisRectTransform == null) return;
        PrevItem = thisRectTransform;
        thisRectTransform.DOScale(Vector3.one * .55f, 0.2f)
            .SetEase(Ease.InBack);
    }
}