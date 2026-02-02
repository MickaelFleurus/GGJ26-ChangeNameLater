using UnityEngine;

[CreateAssetMenu(fileName = "SoundConfig", menuName = "Audio/Sound Config")]
public class SoundConfig : ScriptableObject
{
    [Header("Player Sounds")]
    public AudioClip walkSound;
    public AudioClip deathSound;
    public AudioClip maskSound;
    public AudioClip breathingSound;
    public AudioClip heartbeatSound;

    [Header("Item Sounds")]
    public AudioClip moneySound;

    [Header("Music & Ambience")]
    public AudioClip musicClip;
    public AudioClip mainMenuMusic;
    public AudioClip ambienceClip;

    [Header("GameStates")]
    public AudioClip gameLost;
    public AudioClip gameWon;

    [Header("Environment")]
    public AudioClip doorOpen;

    [Header("UI")]
    public AudioClip click;
}
