using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance;

    public AudioSource effectsSource;
    public AudioSource musicSource;

    public AudioClip[] levelSong;

    [Range(0f, 1f)] public float masterSfxVolume = 1f;
    [Range(0f, 1f)] public float masterMusicVolume = 1f;

    // ==========================================
    // LIFECYCLE
    // ==========================================

    // Sets up the singleton instance, persists across scenes, and applies the initial music volume.
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
           // DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        if (musicSource != null) musicSource.volume = masterMusicVolume;
    }

    // ==========================================
    // SFX PLAYBACK
    // ==========================================

    // Plays a one-shot SFX clip through the effects source, scaled by master volume and optional per-call scale.
    public void PlaySound(AudioClip clip, float volumeScale = 1f)
    {
        if (effectsSource == null || clip == null) return;
        effectsSource.PlayOneShot(clip, masterSfxVolume * volumeScale);
    }

    // ==========================================
    // MUSIC PLAYBACK
    // ==========================================

    // Plays the given clip on the music source. Does nothing if the same clip is already playing.
    public void PlayMusic(AudioClip clip)
    {
        if (musicSource == null || clip == null) return;
        if (musicSource.clip == clip && musicSource.isPlaying) return;

        musicSource.clip = clip;
        musicSource.volume = masterMusicVolume;
        musicSource.Play();
    }

    // Plays a track from the levelSong array by index with bounds checking.
    public void PlayLevelSong(int levelNumber)
    {
        if (levelSong == null || levelNumber < 0 || levelNumber >= levelSong.Length) return;
        PlayMusic(levelSong[levelNumber]);
    }

    // Stops the current music track if one is playing.
    public void StopMusic()
    {
        if (musicSource != null) musicSource.Stop();
    }

    // Stops any currently playing effects track (useful for looped SFX on the effects source).
    public void StopSound()
    {
        if (effectsSource != null) effectsSource.Stop();
    }

    // ==========================================
    // VOLUME CONTROLS
    // ==========================================

    // Sets the master SFX volume multiplier; typically bound to a UI slider.
    public void SetSfxVolume(float value)
    {
        masterSfxVolume = Mathf.Clamp01(value);
    }

    // Sets the master music volume multiplier and applies it to the music source immediately.
    public void SetMusicVolume(float value)
    {
        masterMusicVolume = Mathf.Clamp01(value);
        if (musicSource != null) musicSource.volume = masterMusicVolume;
    }
}
