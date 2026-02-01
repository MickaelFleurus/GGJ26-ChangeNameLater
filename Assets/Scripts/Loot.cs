using UnityEngine;

public interface IInteractable
{
    void OnInteract();
    int GetValue();
    float GetRequiredHoldTime();
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
    private Renderer mRenderer;

    public LootType LootType => lootType;

    void Start()
    {
        mRenderer = GetComponent<Renderer>();
        mRenderer.enabled = false;
        GameEvents.OnMaskEquipped += Show;
        GameEvents.OnMaskOff += Hide;
    }

    public void OnInteract()
    {
        GameEvents.InvokeLootCollected(Value, lootType);  //this can be for telling player loot has been picked and what item
        GameEvents.InvokeLootCollected();//this one is for sound
        GameEvents.InvokePickUpItem();
        Destroy(gameObject);
        GameEvents.OnMaskEquipped -= Show;
        GameEvents.OnMaskOff -= Hide;
    }

    public int GetValue() => Value;
    public float GetRequiredHoldTime() => Value / 2f;

    public void Show()
    {
        mRenderer.enabled = true;
    }

    public void Hide()
    {
        mRenderer.enabled = false;
    }
}
