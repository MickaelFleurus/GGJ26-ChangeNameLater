using UnityEngine;

/// <summary>
/// Door that opens when player has enough money and holds E for required time (mask must be on).
/// Door GameObject (or a child) must have a Collider so InGameUI raycast can hit it.
/// </summary>
public class Door : MonoBehaviour, IInteractable
{
    [SerializeField] int requiredMoney = 100;
    [SerializeField] float requiredHoldTime = 2f;

    public int GetValue() => requiredMoney;
    public float GetRequiredHoldTime() => requiredHoldTime;

    public void OnInteract()
    {
        GameEvents.InvokeDoorOpen();
        Destroy(gameObject);
    }
}
