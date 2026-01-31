using UnityEngine;

public class SoundManager : MonoBehaviour
{
    [Header("Player Sounds")]
    public AudioClip walkSound;
    public AudioClip deathSound;

    [Header("Enemy Sounds")]
    public AudioClip enemyMoveSound;
    public AudioClip enemyHeadSound;

    [Header("Item Sounds")]
    public AudioClip pickupSound;
    public AudioClip moneySound;

    [Header("Music & Ambience")]
    public AudioClip musicClip;
    public AudioClip ambienceClip;

    private AudioSource sfxSource;      // For sound effects
    private AudioSource musicSource;    // For music
    private AudioSource ambienceSource; // For ambience

    void Awake()
    {
        // Create 3 audio sources
        sfxSource = gameObject.AddComponent<AudioSource>();
        musicSource = gameObject.AddComponent<AudioSource>();
        ambienceSource = gameObject.AddComponent<AudioSource>();

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

    //void OnEnable()
    //{
    //    GameEvents.OnMaskEquipped += PlayMaskSound
    //    GameEvents.OnPlayerWalking += PlayWalkSound;
    //    GameEvents.OnPlayerDeath += PlayDeathSound;
    //    GameEvents.OnEnemyMoveHead += PlayEnemyHeadSound;
    //    GameEvents.OnPickUpItem += PlayPickupSound;

    //}

    //void OnDisable()
    //{
    //    GameEvents.OnPlayerWalkStep -= PlayWalkSound;
    //    GameEvents.OnPlayerDeath -= PlayDeathSound;
    //    GameEvents.OnEnemyMove -= PlayEnemyMoveSound;
    //    GameEvents.OnEnemyHeadMove -= PlayEnemyHeadSound;
    //    GameEvents.OnItemPickup -= PlayPickupSound;
    //    GameEvents.OnMoneyGained -= PlayMoneySound;
    //}

    //void PlayWalkSound() => sfxSource.PlayOneShot(walkSound);
    //void PlayDeathSound() => sfxSource.PlayOneShot(deathSound);
    //void PlayEnemyMoveSound() => sfxSource.PlayOneShot(enemyMoveSound);
    //void PlayEnemyHeadSound() => sfxSource.PlayOneShot(enemyHeadSound);
    //void PlayPickupSound() => sfxSource.PlayOneShot(pickupSound);
    //void PlayMoneySound() => sfxSource.PlayOneShot(moneySound);
}
