using GLTFast.Schema;
using UnityEngine;
using UnityEngine.Android;

public class Enemy3DWalkingSound : MonoBehaviour
{
    [Header("Enemy 3D Sound")]
    public AudioClip soundClip;
    public float minDistance = 3f;   // Full volume within 3 meters
    public float maxDistance = 9f;  // Silent beyond 15 meters
    public float volume = 0.5f;

    private AudioSource audioSource;

    void Start()
    {
        // clocks 3d sound
        audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.clip = soundClip;
        audioSource.loop = true;               // Always looping
        audioSource.spatialBlend = 1f;         // Full 3D, importanto
        audioSource.minDistance = minDistance;
        audioSource.maxDistance = maxDistance;
        audioSource.volume = volume;
        audioSource.playOnAwake = true;


        GameEvents.OnMaskEquipped += StartWalking;
        GameEvents.OnMaskOff += StopWalking;
    }

    void OnDestroy()
    {
        GameEvents.OnMaskEquipped -= StartWalking;
        GameEvents.OnMaskOff -= StopWalking;
    }

    public void StopWalking()
    {
        if (audioSource == null || !this) return;
        audioSource.Stop();
    }

    public void StartWalking()
    {
        if (audioSource == null || !this) return;
        if (!audioSource.isPlaying)
            audioSource.Play();
    }

}
