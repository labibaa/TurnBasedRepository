// =====================================================================
// HealthBar.cs
// Animated HP bar for uGUI. Drop onto a RectTransform with this hierarchy:
//
//   HealthBar            (this script, RectTransform, Image = BG/frame)
//   ├─ DamageGhost       (Image, type=Filled, Horizontal, Origin Left)  <-- lagging fill
//   ├─ Fill              (Image, type=Filled, Horizontal, Origin Left)  <-- main fill
//   │   └─ Shine         (Image with /Sprites/UI-Gradient or white; parented inside Fill)
//   └─ Stripes           (Image using the ChipStripes shader/material, on top of Fill)
//
// Fill and DamageGhost should use sprites with Image Type = Filled.
// Shine is a narrow (~25% width) semi-transparent white gradient that
//   animates across the Fill rect via anchoredPosition.
// Stripes uses the shader below — its alpha is driven by the low-HP threshold.
//
// Usage:
//     healthBar.SetMax(120);
//     healthBar.SetHP(120);
//     ...
//     healthBar.Damage(34);     // animates
//     healthBar.Heal(18);
// =====================================================================

using System.Collections;
using UnityEngine;
using UnityEngine.UI;

[ExecuteAlways]
public class HealthBar : MonoBehaviour
{
    [Header("Refs")]
    public Image fill;          // main red fill (Image Type = Filled, Horizontal)
    public Image damageGhost;   // lagging "lost HP" ghost (Image Type = Filled, Horizontal)
    public RectTransform shine; // shine highlight inside Fill
    public CanvasGroup stripes; // animated diagonal stripes overlay (shader does the motion)

    [Header("Values")]
    public float hp = 100f;
    public float hpMax = 100f;

    [Header("Drain timing")]
    [Tooltip("Seconds it takes the main Fill to tween to the new HP.")]
    public float fillDrainDuration = 0.28f;

    [Tooltip("Seconds the ghost fill waits before it starts chasing the main fill.")]
    public float ghostDelay = 0.35f;

    [Tooltip("Seconds the ghost fill takes to catch up, once it starts moving.")]
    public float ghostDrainDuration = 0.55f;

    [Header("Shine sweep")]
    [Tooltip("How often the shine sweeps across, in seconds.")]
    public float shineInterval = 3f;

    [Tooltip("How long one sweep lasts, in seconds.")]
    public float shineDuration = 0.9f;

    [Tooltip("Width of the shine as a fraction of the bar width (0..1). E.g. 0.25 = shine is 25% wide.")]
    [Range(0.05f, 1f)] public float shineWidthFraction = 0.25f;

    [Header("Low-HP chip stripes")]
    [Tooltip("Below this HP fraction, the animated diagonal stripes fade in.")]
    [Range(0f, 1f)] public float lowHpThreshold = 0.4f;

    [Tooltip("Max opacity of the stripes when HP is very low.")]
    [Range(0f, 1f)] public float stripesMaxAlpha = 0.9f;

    // --- internal ---
    float displayedFill;        // current tweened fraction for the main fill (0..1)
    float displayedGhost;       // current tweened fraction for the ghost fill (0..1)
    Coroutine drainCo, ghostCo, shineCo;
    RectTransform barRect;

    void Awake()
    {
        barRect = transform as RectTransform;
        displayedFill = displayedGhost = SafeFraction(hp, hpMax);
        ApplyFills();
        if (Application.isPlaying) shineCo = StartCoroutine(ShineLoop());
    }

    void OnValidate()
    {
        hp = Mathf.Clamp(hp, 0, hpMax);
        displayedFill = displayedGhost = SafeFraction(hp, hpMax);
        ApplyFills();
    }

    void Update()
    {
        // Drive the chip-stripes opacity based on current displayed HP.
        if (stripes != null)
        {
            float frac = displayedFill;
            float t = 1f - Mathf.InverseLerp(0f, lowHpThreshold, Mathf.Max(frac, 0.0001f));
            // t=0 when fraction>=threshold, t=1 when fraction=0
            stripes.alpha = Mathf.Lerp(stripes.alpha, t * stripesMaxAlpha, Time.deltaTime * 8f);
        }
    }

    // ----------- Public API -----------

    public void SetMax(float newMax, bool clampCurrent = true)
    {
        hpMax = Mathf.Max(1f, newMax);
        if (clampCurrent) hp = Mathf.Min(hp, hpMax);
        displayedFill = displayedGhost = SafeFraction(hp, hpMax);
        ApplyFills();
    }

    public void SetHP(float value, bool snap = true)
    {
        hp = Mathf.Clamp(value, 0, hpMax);
        if (snap)
        {
            displayedFill = displayedGhost = SafeFraction(hp, hpMax);
            ApplyFills();
        }
        else
        {
            TweenTo(SafeFraction(hp, hpMax));
        }
    }

    public void Damage(float amount)
    {
        hp = Mathf.Clamp(hp - amount, 0, hpMax);
        TweenTo(SafeFraction(hp, hpMax));
    }

    public void Heal(float amount)
    {
        hp = Mathf.Clamp(hp + amount, 0, hpMax);
        TweenTo(SafeFraction(hp, hpMax));
    }

    // ----------- Core tween -----------

    void TweenTo(float targetFraction)
    {
        // Main fill drains quickly
        if (drainCo != null) StopCoroutine(drainCo);
        drainCo = StartCoroutine(TweenValue(
            getter:  () => displayedFill,
            setter:  v  => { displayedFill = v; if (fill) fill.fillAmount = v; },
            to:      targetFraction,
            duration: fillDrainDuration,
            easing:  EaseOutCubic,
            delay:   0f
        ));

        // Ghost fill waits, then chases. Only chase DOWN (for damage), not up.
        if (targetFraction < displayedGhost)
        {
            if (ghostCo != null) StopCoroutine(ghostCo);
            ghostCo = StartCoroutine(TweenValue(
                getter:  () => displayedGhost,
                setter:  v  => { displayedGhost = v; if (damageGhost) damageGhost.fillAmount = v; },
                to:      targetFraction,
                duration: ghostDrainDuration,
                easing:  EaseOutQuart,
                delay:   ghostDelay
            ));
        }
        else
        {
            // Healing: snap ghost up with the main fill, no lag.
            displayedGhost = targetFraction;
            if (damageGhost) damageGhost.fillAmount = targetFraction;
        }
    }

    IEnumerator TweenValue(System.Func<float> getter, System.Action<float> setter,
                           float to, float duration, System.Func<float,float> easing, float delay)
    {
        if (delay > 0f) yield return new WaitForSeconds(delay);

        float from = getter();
        float elapsed = 0f;
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / duration);
            setter(Mathf.Lerp(from, to, easing(t)));
            yield return null;
        }
        setter(to);
    }

    // ----------- Shine sweep -----------

    IEnumerator ShineLoop()
    {
        if (shine == null) yield break;

        // Keep the shine sized relative to the bar width.
        // Shine should be a RectTransform anchored left-inside its parent (the Fill),
        // stretched vertically. We move it by anchoredPosition.x.
        while (true)
        {
            yield return new WaitForSeconds(shineInterval);

            RectTransform parent = shine.parent as RectTransform;
            if (parent == null) continue;

            float parentWidth = parent.rect.width;
            float shineWidth = parentWidth * shineWidthFraction;

            // Resize shine
            Vector2 size = shine.sizeDelta;
            shine.sizeDelta = new Vector2(shineWidth, size.y);

            // Travel from -shineWidth (off-left) to parentWidth + shineWidth (off-right).
            float startX = -shineWidth;
            float endX   =  parentWidth + shineWidth;

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

    // ----------- helpers -----------

    void ApplyFills()
    {
        if (fill)        fill.fillAmount        = displayedFill;
        if (damageGhost) damageGhost.fillAmount = displayedGhost;
    }

    static float SafeFraction(float a, float b) => b <= 0f ? 0f : Mathf.Clamp01(a / b);

    // Tiny easing functions (no DOTween dependency).
    static float EaseOutCubic(float x) => 1f - Mathf.Pow(1f - x, 3f);
    static float EaseOutQuart(float x) => 1f - Mathf.Pow(1f - x, 4f);
    static float EaseInOutSine(float x) => -(Mathf.Cos(Mathf.PI * x) - 1f) / 2f;
}
