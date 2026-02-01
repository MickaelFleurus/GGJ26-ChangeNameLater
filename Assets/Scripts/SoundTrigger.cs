using UnityEngine;

/// <summary>
/// Invisible trigger that plays a 3D sound from another object
/// </summary>
public class SoundTrigger : MonoBehaviour
{
    [Header("Sound to Play")]
    [Tooltip("Drag the object with Play3DSoundOnce script here")]
    public Add3DOneShotSound soundToPlay;

    [Header("Settings")]
    public bool triggerOnce = true;

    private bool hasTriggered = false;

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (triggerOnce && hasTriggered) return;

            // Play the sound
            if (soundToPlay != null)
            {
                soundToPlay.PlaySound();
                hasTriggered = true;
            }
        }
    }
}