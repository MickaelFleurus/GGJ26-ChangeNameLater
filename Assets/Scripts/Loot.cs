using UnityEngine;

public interface IInteractable
{
    void OnInteract();
    int GetValue();
}

public enum LootType
{
    Key,
    Document,
    Battery,
    Generic,
    money
}

public class Loot : MonoBehaviour, IInteractable
{
    [SerializeField] LootType lootType = LootType.Generic;
    [SerializeField] public int Value;

    public LootType LootType => lootType;

    public void OnInteract()
    {
        Debug.Log("[Loot] OnInteract() called. Type=" + lootType + ", Value=" + Value + ", name=" + gameObject.name);
        GameEvents.InvokeLootCollected(Value, lootType);
        GameEvents.InvokePickUpItem();
        Destroy(gameObject);
    }

    public int GetValue() => Value;
}
