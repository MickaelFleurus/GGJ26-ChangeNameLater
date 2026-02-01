using UnityEngine;

/// <summary>
/// Door that opens when player has enough money and holds E for required time (mask must be on).
/// Door GameObject (or a child) must have a Collider so InGameUI raycast can hit it.
/// </summary>
public class Door : MonoBehaviour, IInteractable
{
    [SerializeField] int requiredMoney = 100;
    [SerializeField] float requiredHoldTime = 2f;

    #region 3DThings
    [Header("3D Sound")]
    public AudioClip soundClip;
    public float minDistance = 3f;   // Full volume within 3 meters
    public float maxDistance = 9f;  // Silent beyond 15 meters
    public float volume = 0.5f;

    private AudioSource audioSource;

    void Start()
    {
        // Door 3d sound
        audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.clip = soundClip;
        audioSource.loop = false;              
        audioSource.spatialBlend = 1f;         // Full 3D, importanto
        audioSource.minDistance = minDistance;
        audioSource.maxDistance = maxDistance;
        audioSource.volume = volume;
   

       // GameEvents.OnDoorUnlocked += PlayDoorUnlockedSound;
    }
    #endregion

    public void Update()
    {
        
        if(GameEvents.CurrentMoney >= requiredMoney)
            PlayDoorUnlockedSound();
    }

    public int GetValue() => requiredMoney;
    public float GetRequiredHoldTime() => requiredHoldTime;

    public void PlayDoorUnlockedSound()
    {
        if (audioSource != null)
            audioSource.Play();
    }


    public void OnInteract()
    {
       
            GameEvents.InvokeDoorOpen();
            GameEvents.InvokeGameWon();
            Destroy(gameObject);
    }
}
