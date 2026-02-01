using GLTFast.Schema;
using UnityEngine;

public class Add3DOneShotSound : MonoBehaviour
{
    [Header("3D Sound")]
    public AudioClip soundClip;

    [Range(1f, 10f)]
    public float minDistance = 3f;

    [Range(5f, 50f)]
    public float maxDistance = 15f;

    [Range(0f, 1f)]
    public float volume = 0.8f;

    [Header("Trigger Settings")]
    [Tooltip("Play sound when player enters trigger collider")]
    public bool playOnTriggerEnter = false;

    [Tooltip("Only play once, even if triggered multiple times")]
    public bool playOnlyOnce = true;

    private AudioSource audioSource;
    private bool hasPlayed = false;

    void Start()
    {
        // Create 3D audio source
        audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.clip = soundClip;
        audioSource.loop = false;              // One-shot!
        audioSource.spatialBlend = 1f;         // Full 3D
        audioSource.minDistance = minDistance;
        audioSource.maxDistance = maxDistance;
        audioSource.volume = volume;
        audioSource.playOnAwake = false;
    }

    void OnTriggerEnter(Collider other)
    {
        if (playOnTriggerEnter && other.CompareTag("Player"))
        {
            PlaySound();
        }
    }

    /// <summary>
    /// Call this to play the sound manually
    /// </summary>
    public void PlaySound()
    {
        if (playOnlyOnce && hasPlayed) return;

        if (audioSource != null && soundClip != null)
        {
            audioSource.PlayOneShot(soundClip);
            hasPlayed = true;
        }
    }
}
