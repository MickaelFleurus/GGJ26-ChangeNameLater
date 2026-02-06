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

    #region MemberVariables&DefaultVol
    private SoundConfig soundConfig;
    private GameObject audioGameObject;

    private AudioSource sfxSource;
    private AudioSource walkSource;
    private AudioSource musicSource;
    private AudioSource mainMenuSource;
    private AudioSource ambienceSource;
    private AudioSource breathingSource;
    private AudioSource heartbeatSource;

    private float defaultSfxVolume = 0.5f;
    private float defaultMusicVolume = 0.2f;
    private float defaultAmbienceSourceVolume = 0.1f;
    private float defaultBreathingSourceVolume = 0.2f;
    private float defaultMainMenuVolume = 0.1f;
    private float defaultHeartbeatVolume = 0.0f;
    #endregion

    private SoundManager()
    {
        Initialize();
    }

    public void Initialize(SoundConfig config = null)
    {
        if (soundConfig != null)
            return;

        // Load default config if none provided
        if (config == null)
        {
            config = Resources.Load<SoundConfig>("SoundConfig");
        }

        soundConfig = config;
        float masterVolume = GameSettings.Instance.MasterVolume;
        GameSettings.OnMasterVolumeChanged += OnMasterVolumeChanged;

        // Create audio container GameObject
        audioGameObject = new GameObject("SoundManager_AudioSources");
        Object.DontDestroyOnLoad(audioGameObject);

        #region Set up Data
        sfxSource = audioGameObject.AddComponent<AudioSource>();
        sfxSource.spatialBlend = 0f;
        sfxSource.volume = defaultSfxVolume * masterVolume;

        musicSource = audioGameObject.AddComponent<AudioSource>();
        musicSource.spatialBlend = 0f;
        musicSource.clip = soundConfig.musicClip;
        musicSource.loop = true;
        musicSource.volume = defaultMusicVolume * masterVolume;

        ambienceSource = audioGameObject.AddComponent<AudioSource>();
        ambienceSource.spatialBlend = 0f;
        ambienceSource.clip = soundConfig.ambienceClip;
        ambienceSource.loop = true;
        ambienceSource.volume = defaultAmbienceSourceVolume * masterVolume;

        walkSource = audioGameObject.AddComponent<AudioSource>();
        walkSource.clip = soundConfig.walkSound;
        walkSource.loop = true;
        walkSource.spatialBlend = 0f;

        breathingSource = audioGameObject.AddComponent<AudioSource>();
        breathingSource.clip = soundConfig.breathingSound;
        breathingSource.loop = true;
        breathingSource.volume = defaultBreathingSourceVolume * masterVolume;

        mainMenuSource = audioGameObject.AddComponent<AudioSource>();
        mainMenuSource.clip = soundConfig.mainMenuMusic;
        mainMenuSource.loop = true;
        mainMenuSource.volume = defaultMainMenuVolume * masterVolume;
        mainMenuSource.Play();

        heartbeatSource = audioGameObject.AddComponent<AudioSource>();
        heartbeatSource.clip = soundConfig.heartbeatSound;
        heartbeatSource.loop = true;
        heartbeatSource.spatialBlend = 0f;
        heartbeatSource.volume = defaultHeartbeatVolume;
        heartbeatSource.pitch = 1f;
        #endregion

        #region Subscribe to events
        GameEvents.OnPlayerWalking += StartWalkSound;
        GameEvents.OnPlayerNotWalking += StopWalkSound;

        GameEvents.OnMaskEquipped += PlayMaskOnSound;
        GameEvents.OnMaskEquipped += PlayBreathingSound;
        GameEvents.OnMaskOff += PlayMaskOffSound;
        GameEvents.OnMaskOff += StopBreathingSound;

        GameEvents.OnDyingAnimationStart += PlayDeathSound;
        GameEvents.OnGameLost += StopBreathingSound;
        GameEvents.OnGameWon += PlayGameWonSound;
        GameEvents.onLootCollected += PlayMoneySound;

        GameEvents.OnInGame += StartAmbience;
        GameEvents.OnInGame += StartMusic;
        GameEvents.OnInGame += StopMainMenuSound;

        GameEvents.OnUIClick += PlayClickSound;
        #endregion
    }

    public void SetHeartbeatIntensity(float intensity)
    {
        intensity = Mathf.Clamp01(intensity); //This keeps it between 0 - 1 ,very good

        if (heartbeatSource == null)
            return;

        heartbeatSource.volume = Mathf.Lerp(0.0f, 0.8f, intensity);

        heartbeatSource.pitch = Mathf.Lerp(1.0f, 1.8f, intensity);

        if (intensity > 0.05f && !heartbeatSource.isPlaying)
        {
            heartbeatSource.Play();
        }
        else if (intensity <= 0.05f && heartbeatSource.isPlaying)
        {
            heartbeatSource.Stop();
        }
    }

    #region PlayOneShots
    void PlayMoneySound() => sfxSource.PlayOneShot(soundConfig.moneySound);
    void PlayMaskOnSound() => sfxSource.PlayOneShot(soundConfig.maskSound);
    void PlayMaskOffSound() => sfxSource.PlayOneShot(soundConfig.maskSound);
    void PlayGameWonSound() => sfxSource.PlayOneShot(soundConfig.doorOpen);
    void PlayDeathSound() { sfxSource.PlayOneShot(soundConfig.deathSound); }
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

    public void StartWalkSound()
    {
        if (!walkSource.isPlaying)
            walkSource.Play();
    }

    public void StopWalkSound()
    {
        walkSource.Stop();
    }

    public void PlayBreathingSound()
    {
        if (!breathingSource.isPlaying)
            breathingSource.Play();
    }

    public void StopBreathingSound()
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

    public void StopHeartbeat()
    {
        heartbeatSource.Stop();
    }
    #endregion

    #region Volume
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

    public void OnMasterVolumeChanged(float masterVolume)
    {
        masterVolume = masterVolume * masterVolume;
        musicSource.volume = masterVolume * defaultMusicVolume;
        sfxSource.volume = masterVolume * defaultSfxVolume;
        walkSource.volume = masterVolume * defaultSfxVolume;
        breathingSource.volume = masterVolume * defaultBreathingSourceVolume;
        mainMenuSource.volume = masterVolume * defaultMainMenuVolume;
    }

    #endregion
}

