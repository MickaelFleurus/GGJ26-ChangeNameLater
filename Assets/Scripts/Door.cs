using UnityEngine;

/// <summary>
/// Door that can be opened when player has enough money (mask on, hold E for required time).
/// Add a Collider so the InGameUI raycast can hit it.
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
