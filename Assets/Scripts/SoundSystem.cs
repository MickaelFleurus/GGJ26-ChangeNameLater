using UnityEngine;

public class SoundManager : MonoBehaviour
{
    // Add this static reference
    private static SoundManager instance;

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

    [Header("Environment")]
    public AudioClip doorOpen;

    [Header("UI")]
    public AudioClip click;

    private AudioSource sfxSource;
    private AudioSource walkSource;
    private AudioSource musicSource;
    private AudioSource ambienceSource;

    void Awake()
    {
        // Singleton pattern - only one SoundManager exists
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;
        DontDestroyOnLoad(gameObject); // Persists across scenes!

        // Create audio sources
        sfxSource = gameObject.AddComponent<AudioSource>();
        sfxSource.spatialBlend = 0f; // 2D

        musicSource = gameObject.AddComponent<AudioSource>();
        musicSource.spatialBlend = 0f; // 2D

        ambienceSource = gameObject.AddComponent<AudioSource>();
        ambienceSource.spatialBlend = 0f; // 2D

        walkSource = gameObject.AddComponent<AudioSource>();
        walkSource.clip = walkSound;
        walkSource.loop = true;
        walkSource.spatialBlend = 0f; // 2D

        // Setup music
        musicSource.clip = musicClip;
        musicSource.loop = true;
        musicSource.volume = 0.5f;
        StartMusic();

        // Setup ambience
        ambienceSource.clip = ambienceClip;
        ambienceSource.loop = true;
        ambienceSource.volume = 0.3f;
        StartAmbience();
    }

    void OnEnable()
    {
        GameEvents.OnPlayerWalking += StartWalkSound;
        GameEvents.OnPlayerNotWalking += StopWalkSound;

        GameEvents.OnMaskEquipped += PlayMaskOnSound;
        GameEvents.OnMaskOff += PlayMaskOffSound;

        GameEvents.OnGameLost += PlayDeathSound;
        GameEvents.OnGameWon += PlayGameWonSound;

        GameEvents.OnPickUpItem += PlayPickupSound;
        GameEvents.OnDoorOpen += PlayDoorOpenSound;


        GameEvents.OnUIClick += PlayClickSound;
    }

    void OnDisable()
    {
        GameEvents.OnPlayerWalking -= StartWalkSound;
        GameEvents.OnPlayerNotWalking -= StopWalkSound;

        GameEvents.OnMaskEquipped -= PlayMaskOnSound;
        GameEvents.OnMaskOff -= PlayMaskOffSound;

        GameEvents.OnGameLost -= PlayDeathSound;
        GameEvents.OnGameWon -= PlayGameWonSound;

        GameEvents.OnPickUpItem -= PlayPickupSound;
        GameEvents.OnDoorOpen -= PlayDoorOpenSound;

        GameEvents.OnUIClick -= PlayClickSound;
    }

    #region PlayOneShots
    void PlayPickupSound() => sfxSource.PlayOneShot(pickupSound);
    void PlayMoneySound() => sfxSource.PlayOneShot(moneySound);
    void PlayMaskOnSound() => sfxSource.PlayOneShot(maskOnSound);
    void PlayMaskOffSound() => sfxSource.PlayOneShot(maskOffSound);
    void PlayGameWonSound() => sfxSource.PlayOneShot(gameWon);
    void PlayDeathSound() => sfxSource.PlayOneShot(deathSound);
    void PlayDoorOpenSound() => sfxSource.PlayOneShot(doorOpen);
    void PlayClickSound() => sfxSource.PlayOneShot(click);
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
