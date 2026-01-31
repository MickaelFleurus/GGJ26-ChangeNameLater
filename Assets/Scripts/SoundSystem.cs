using System.Data.SqlTypes;
using UnityEngine;

/// <summary>
/// This is only for 2D sounds, for 3D sounds we keep
/// that on the objects
/// </summary>
public class SoundManager : MonoBehaviour
{
    [Header("Player Sounds")]
    public AudioClip walkSound;
    public AudioClip deathSound;
    public AudioClip maskOnSound;
    public AudioClip maskOffSound;

    [Header("Item Sounds")]
    public AudioClip pickupSound;
    public AudioClip moneySound;

    [Header("Music & Ambience")]
    public AudioClip musicClip;
    public AudioClip ambienceClip;

    [Header("GameStates")]
    public AudioClip playerDeath;
    public AudioClip gameWon;

    [Header("Enviroment")]
    public AudioClip doorOpen;

    private AudioSource sfxSource;      // For sound effects
    private AudioSource walkSource;
    private AudioSource musicSource;    // For music
    private AudioSource ambienceSource; // For ambience

    void Awake()
    {
        // Create 3 audio sources
        sfxSource = gameObject.AddComponent<AudioSource>();
        musicSource = gameObject.AddComponent<AudioSource>();
        ambienceSource = gameObject.AddComponent<AudioSource>();
        walkSource = gameObject.AddComponent<AudioSource>();

        walkSource.clip = walkSound;
        walkSource.loop = true;
        walkSource.spatialBlend = 0f; // 2D

        // Setup music
        musicSource.clip = musicClip;
        musicSource.loop = true;
        musicSource.volume = 0.5f;
        musicSource.Play();

        // Setup ambience
        ambienceSource.clip = ambienceClip;
        ambienceSource.loop = true;
        ambienceSource.volume = 0.3f;
        ambienceSource.Play();
    }

    void OnEnable()
    {
        GameEvents.OnPlayerWalking += StartWalkSound; ;
        GameEvents.OnPlayerNotWalking += StopWalkSound;

        GameEvents.OnMaskEquipped += PlayMaskOnSound;
        GameEvents.OnMaskOff += PlayMaskOffSound;


        GameEvents.OnGameLost += PlayDeathSound;
        GameEvents.OnGameWon += PlayGameWonSound;

      
        GameEvents.OnPickUpItem += PlayPickupSound;
        GameEvents.OnDoorOpen += PlayDoorOpenSound;

    }

    void OnDisable()
    {
       
    }

    #region PlayOneShots
    // void PlayWalkSound() => sfxSource.PlayOneShot(walkSound);

    void PlayPickupSound() => sfxSource.PlayOneShot(pickupSound);
    void PlayMoneySound() => sfxSource.PlayOneShot(moneySound);

    void PlayMaskOnSound() => sfxSource.PlayOneShot(maskOnSound);
    void PlayMaskOffSound() => sfxSource.PlayOneShot(maskOffSound);

    void PlayGameWonSound() => sfxSource.PlayOneShot(gameWon);
    void PlayDeathSound() => sfxSource.PlayOneShot(deathSound);

    void PlayDoorOpenSound() => sfxSource.PlayOneShot(doorOpen);
    #endregion

    #region start&stop functions for looping sounds
    public void StopMusic()
    {
        musicSource.Stop();
    }

    public void StartMusic()
    {
        if (!musicSource.isPlaying)
            musicSource.Play();
    }

    public void StopAmbience()
    {
        ambienceSource.Stop();
    }

    public void StartAmbience()
    {
        if (!ambienceSource.isPlaying)
            ambienceSource.Play();
    }

    void StartWalkSound()
    {
        if (!walkSource.isPlaying)
            walkSource.Play();
    }

    void StopWalkSound()
    {
        walkSource.Stop();
    }
    #endregion

    public void SetMusicVolume(float volume)
    {
        musicSource.volume = Mathf.Clamp01(volume);
    }

    public void SetAmbienceVolume(float volume)
    {
        ambienceSource.volume = Mathf.Clamp01(volume);
    }

    public void SetSFXVolume(float volume)
    {
        sfxSource.volume = Mathf.Clamp01(volume);
        walkSource.volume = Mathf.Clamp01(volume);
    }

}
