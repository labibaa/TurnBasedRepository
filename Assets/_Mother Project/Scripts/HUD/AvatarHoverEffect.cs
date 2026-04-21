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

    [Header("Smoke Trail (auto-built in code)")]
    [SerializeField] private bool buildSmokeTrail = true;
    [SerializeField] private Vector3 smokeLocalOffset = new Vector3(0f, -30f, 0f);
    [SerializeField] private float smokeEmissionRate = 8f;
    [SerializeField] private float smokeMinSize = 40f;
    [SerializeField] private float smokeMaxSize = 80f;
    [SerializeField] private float smokeRiseSpeedMin = 40f;
    [SerializeField] private float smokeRiseSpeedMax = 80f;
    [SerializeField] private Color smokeTint = new Color(0.78f, 0.78f, 0.82f, 0.63f);

    private RectTransform _rect;
    private Image _image;
    private Vector3 _baseScale;
    private Color _baseColor;
    private ParticleSystem _smoke;

    private static Material _smokeMaterial;
    private static Texture2D _smokeTexture;

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

        if (buildSmokeTrail)
            _smoke = BuildSmokeTrail();
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

        if (_smoke != null && !_smoke.isEmitting)
            _smoke.Play();
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        KillTweens();

        _rect.DOScale(_baseScale, exitDuration).SetEase(Ease.OutQuad);
        _image.DOColor(_baseColor, exitDuration);

        if (glowRing != null)
            glowRing.DOFade(0f, exitDuration);

        if (_smoke != null)
            _smoke.Stop(true, ParticleSystemStopBehavior.StopEmitting);
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
        if (_smoke != null)
            _smoke.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
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

    private ParticleSystem BuildSmokeTrail()
    {
        GameObject go = new GameObject("SmokeTrail");
        go.transform.SetParent(transform, false);
        go.transform.localPosition = smokeLocalOffset;
        go.transform.localRotation = Quaternion.identity;
        go.transform.localScale = Vector3.one;

        ParticleSystem ps = go.AddComponent<ParticleSystem>();
        ps.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);

        var main = ps.main;
        main.duration = 2f;
        main.loop = true;
        main.startLifetime = new ParticleSystem.MinMaxCurve(1.2f, 1.8f);
        main.startSpeed = new ParticleSystem.MinMaxCurve(0.3f, 0.6f);
        main.startSize = new ParticleSystem.MinMaxCurve(smokeMinSize, smokeMaxSize);
        main.startRotation = new ParticleSystem.MinMaxCurve(-Mathf.PI, Mathf.PI);
        main.startColor = smokeTint;
        main.simulationSpace = ParticleSystemSimulationSpace.Local;
        main.scalingMode = ParticleSystemScalingMode.Hierarchy;
        main.playOnAwake = false;
        main.maxParticles = 60;

        var emission = ps.emission;
        emission.rateOverTime = smokeEmissionRate;

        var shape = ps.shape;
        shape.shapeType = ParticleSystemShapeType.Circle;
        shape.radius = 15f;

        var velocity = ps.velocityOverLifetime;
        velocity.enabled = true;
        velocity.space = ParticleSystemSimulationSpace.Local;
        velocity.x = new ParticleSystem.MinMaxCurve(-15f, 15f);
        velocity.y = new ParticleSystem.MinMaxCurve(smokeRiseSpeedMin, smokeRiseSpeedMax);

        var sizeOverLife = ps.sizeOverLifetime;
        sizeOverLife.enabled = true;
        AnimationCurve sizeCurve = new AnimationCurve(
            new Keyframe(0f, 0.3f),
            new Keyframe(1f, 1.2f));
        sizeOverLife.size = new ParticleSystem.MinMaxCurve(1f, sizeCurve);

        var colorOverLife = ps.colorOverLifetime;
        colorOverLife.enabled = true;
        Gradient g = new Gradient();
        g.SetKeys(
            new[]
            {
                new GradientColorKey(new Color(0.88f, 0.85f, 0.80f), 0f),
                new GradientColorKey(new Color(0.72f, 0.72f, 0.78f), 1f)
            },
            new[]
            {
                new GradientAlphaKey(0f, 0f),
                new GradientAlphaKey(1f, 0.3f),
                new GradientAlphaKey(0f, 1f)
            });
        colorOverLife.color = g;

        var rotOverLife = ps.rotationOverLifetime;
        rotOverLife.enabled = true;
        rotOverLife.z = new ParticleSystem.MinMaxCurve(-30f * Mathf.Deg2Rad, 30f * Mathf.Deg2Rad);

        var noise = ps.noise;
        noise.enabled = true;
        noise.strength = 0.2f;
        noise.frequency = 0.5f;
        noise.scrollSpeed = 0.3f;
        noise.quality = ParticleSystemNoiseQuality.Low;

        var renderer = ps.GetComponent<ParticleSystemRenderer>();
        renderer.material = GetSmokeMaterial();
        renderer.sortingOrder = 100;

        return ps;
    }

    private static Material GetSmokeMaterial()
    {
        if (_smokeMaterial != null) return _smokeMaterial;

        Shader shader = Shader.Find("Particles/Standard Unlit");
        if (shader == null) shader = Shader.Find("Universal Render Pipeline/Particles/Unlit");
        if (shader == null) shader = Shader.Find("Legacy Shaders/Particles/Alpha Blended");
        if (shader == null) shader = Shader.Find("Sprites/Default");

        Material m = new Material(shader) { name = "SmokeTrailMat (runtime)" };
        Texture2D tex = GetSmokeTexture();
        m.mainTexture = tex;
        if (m.HasProperty("_BaseMap")) m.SetTexture("_BaseMap", tex);
        if (m.HasProperty("_Mode")) m.SetFloat("_Mode", 2f);
        if (m.HasProperty("_Surface")) m.SetFloat("_Surface", 1f);
        if (m.HasProperty("_Blend")) m.SetFloat("_Blend", 0f);

        _smokeMaterial = m;
        return m;
    }

    private static Texture2D GetSmokeTexture()
    {
        if (_smokeTexture != null) return _smokeTexture;

        const int size = 64;
        Texture2D tex = new Texture2D(size, size, TextureFormat.RGBA32, false);
        Vector2 center = new Vector2(size * 0.5f, size * 0.5f);
        float maxDist = size * 0.5f;

        for (int y = 0; y < size; y++)
        {
            for (int x = 0; x < size; x++)
            {
                float d = Vector2.Distance(new Vector2(x, y), center) / maxDist;
                float a = Mathf.Clamp01(1f - d);
                a = a * a;
                tex.SetPixel(x, y, new Color(1f, 1f, 1f, a));
            }
        }

        tex.wrapMode = TextureWrapMode.Clamp;
        tex.Apply();
        _smokeTexture = tex;
        return tex;
    }
}
