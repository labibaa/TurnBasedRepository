using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class AmbienceLoop : MonoBehaviour
{
    public AudioClip clip;
    [Range(0f, 1f)] public float volume = 0.6f;
    public bool playOnStart = true;
    public bool playOnTriggerEnter = false;
    public float fadeDuration = 0.75f;

    private AudioSource _source;
    private float _fadeTimer;
    private bool _fadingIn;
    private bool _fadingOut;

    // ==========================================
    // LIFECYCLE
    // ==========================================

    // Configures the AudioSource for looped 3D ambience playback with zero initial volume.
    private void Awake()
    {
        _source = GetComponent<AudioSource>();
        _source.clip = clip;
        _source.loop = true;
        _source.playOnAwake = false;
        _source.volume = 0f;
    }

    // Auto-starts the ambience if playOnStart is enabled.
    private void Start()
    {
        if (playOnStart) BeginPlay();
    }

    // Ticks the fade in/out interpolation each frame while a fade is active.
    private void Update()
    {
        if (!_fadingIn && !_fadingOut) return;

        _fadeTimer += Time.deltaTime;
        float t = Mathf.Clamp01(_fadeTimer / fadeDuration);

        if (_fadingIn)
        {
            _source.volume = Mathf.Lerp(0f, volume, t);
            if (t >= 1f) _fadingIn = false;
        }
        else
        {
            _source.volume = Mathf.Lerp(volume, 0f, t);
            if (t >= 1f)
            {
                _source.Stop();
                _fadingOut = false;
            }
        }
    }

    // ==========================================
    // PLAYBACK CONTROL
    // ==========================================

    // Starts the loop (if not already playing) and begins fading volume up to the target level.
    public void BeginPlay()
    {
        if (clip == null) return;
        if (!_source.isPlaying) _source.Play();
        _fadeTimer = 0f;
        _fadingIn = true;
        _fadingOut = false;
    }

    // Begins fading volume down; the source stops once the fade completes.
    public void BeginStop()
    {
        if (!_source.isPlaying) return;
        _fadeTimer = 0f;
        _fadingOut = true;
        _fadingIn = false;
    }

    // ==========================================
    // TRIGGER HOOKS (zone-based ambience)
    // ==========================================

    // Triggers a fade-in when an object enters the collider, if playOnTriggerEnter is enabled.
    private void OnTriggerEnter(Collider other)
    {
        if (playOnTriggerEnter) BeginPlay();
    }

    // Triggers a fade-out when an object leaves the collider, if playOnTriggerEnter is enabled.
    private void OnTriggerExit(Collider other)
    {
        if (playOnTriggerEnter) BeginStop();
    }
}
