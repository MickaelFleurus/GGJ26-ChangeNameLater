using UnityEngine;

public class SoundManager
{
    private static SoundManager instance;
    public static SoundManager Instance
    {
        get
        {
            if (instance == null)
            {
                instance = new SoundManager();
            }
            return instance;
        }
    }

    private SoundConfig soundConfig;
    private GameObject audioGameObject;

    private AudioSource sfxSource;
    private AudioSource walkSource;
    private AudioSource musicSource;
    private AudioSource mainMenuSource;
    private AudioSource ambienceSource;
    private AudioSource breathingSource;

    private float masterVolume = 1.0f;
    private float defaultSfxVolume = 0.5f;
    private float defaultMusicVolume = 0.2f;
    private float defaultAmbienceSourceVolume = 0.1f;
    private float defaultBreathingSourceVolume = 0.2f;
    private float defaultMainMenuVolume = 0.1f;

    private SoundManager()
    {
        Initialize();
    }

    public void Initialize(SoundConfig config = null)
    {
        if (soundConfig != null)
            return; // Already initialized

        // Load default config if none provided
        if (config == null)
        {
            config = Resources.Load<SoundConfig>("SoundConfig");
        }

        soundConfig = config;

        // Create audio container GameObject
        audioGameObject = new GameObject("SoundManager_AudioSources");
        Object.DontDestroyOnLoad(audioGameObject);

        // Setup audio sources
        sfxSource = audioGameObject.AddComponent<AudioSource>();
        sfxSource.spatialBlend = 0f;
        sfxSource.volume = defaultSfxVolume;

        musicSource = audioGameObject.AddComponent<AudioSource>();
        musicSource.spatialBlend = 0f;
        musicSource.clip = soundConfig.musicClip;
        musicSource.loop = true;
        musicSource.volume = defaultMusicVolume;

        ambienceSource = audioGameObject.AddComponent<AudioSource>();
        ambienceSource.spatialBlend = 0f;
        ambienceSource.clip = soundConfig.ambienceClip;
        ambienceSource.loop = true;
        ambienceSource.volume = defaultAmbienceSourceVolume;

        walkSource = audioGameObject.AddComponent<AudioSource>();
        walkSource.clip = soundConfig.walkSound;
        walkSource.loop = true;
        walkSource.spatialBlend = 0f;

        breathingSource = audioGameObject.AddComponent<AudioSource>();
        breathingSource.clip = soundConfig.breathingSound;
        breathingSource.loop = true;
        breathingSource.volume = defaultBreathingSourceVolume;

        mainMenuSource = audioGameObject.AddComponent<AudioSource>();
        mainMenuSource.clip = soundConfig.mainMenuMusic;
        mainMenuSource.loop = true;
        mainMenuSource.volume = defaultMainMenuVolume;
        mainMenuSource.Play();

        // Subscribe to events
        GameEvents.OnPlayerWalking += StartWalkSound;
        GameEvents.OnPlayerNotWalking += StopWalkSound;

        GameEvents.OnMaskEquipped += PlayMaskOnSound;
        GameEvents.OnMaskEquipped += PlayBreathingSound;
        GameEvents.OnMaskOff += PlayMaskOffSound;
        GameEvents.OnMaskOff += StopBreathingSound;

        GameEvents.OnGameLost += PlayDeathSound;
        GameEvents.OnGameLost += StopBreathingSound;
        GameEvents.OnGameWon += PlayGameWonSound;
        GameEvents.onLootCollected += PlayMoneySound;

        GameEvents.OnInGame += StartAmbience;
        GameEvents.OnInGame += StartMusic;
        GameEvents.OnInGame += StopMainMenuSound;

        GameEvents.OnUIClick += PlayClickSound;
    }

    #region PlayOneShots
    void PlayMoneySound() => sfxSource.PlayOneShot(soundConfig.moneySound);
    void PlayMaskOnSound() => sfxSource.PlayOneShot(soundConfig.maskSound);
    void PlayMaskOffSound() => sfxSource.PlayOneShot(soundConfig.maskSound);
    void PlayGameWonSound() => sfxSource.PlayOneShot(soundConfig.doorOpen);
    void PlayDeathSound() => sfxSource.PlayOneShot(soundConfig.deathSound);
    void PlayClickSound() => sfxSource.PlayOneShot(soundConfig.click);
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

    void PlayBreathingSound()
    {
        if (!breathingSource.isPlaying)
            breathingSource.Play();
    }

    void StopBreathingSound()
    {
        breathingSource?.Stop();
    }

    public void StartMainMenuMusic()
    {
        mainMenuSource?.Play();
    }

    void StopMainMenuSound()
    {
        mainMenuSource?.Stop();
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

    public void SetMasterVolume(float volume)
    {
        masterVolume = volume;
        GameEvents.InvokeMasterVolumeChanged();

        musicSource.volume = masterVolume * defaultMusicVolume;
        sfxSource.volume = masterVolume * defaultSfxVolume;
        walkSource.volume = masterVolume * defaultSfxVolume;
        breathingSource.volume = masterVolume * defaultBreathingSourceVolume;
        mainMenuSource.volume = masterVolume * defaultMainMenuVolume;
    }

    public float GetMasterVolume()
    {
        return masterVolume;
    }

    public bool IsSoundOn()
    {
        return masterVolume >= 1.0f;
    }
}

