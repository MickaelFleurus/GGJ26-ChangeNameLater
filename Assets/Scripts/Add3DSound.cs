
using UnityEngine;

public class Add3DSound : MonoBehaviour
{
    [Header("3D Sound")]
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

        //Play on start
        audioSource.Play();
    }

    public void StopClock()
    {
        audioSource.Stop();
    }

    public void StartClock()
    {
        if (!audioSource.isPlaying)
            audioSource.Play();
    }

    
}
