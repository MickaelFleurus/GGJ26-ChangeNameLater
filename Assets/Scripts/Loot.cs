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

    public LootType LootType => lootType;

    Renderer[] m_Renderers;

    void Start()
    {
        m_Renderers = GetComponentsInChildren<Renderer>(true);
        GameEvents.OnMaskEquipped += Show;
        GameEvents.OnMaskOff += Hide;

        var maskController = FindObjectOfType<MaskController>();
        SetVisible(maskController != null && maskController.IsMaskOn);
    }

    void OnDestroy()
    {
        GameEvents.OnMaskEquipped -= Show;
        GameEvents.OnMaskOff -= Hide;
    }

    void Show() => SetVisible(true);
    void Hide() => SetVisible(false);

    void SetVisible(bool visible)
    {
        if (m_Renderers == null) return;
        for (int i = 0; i < m_Renderers.Length; i++)
        {
            if (m_Renderers[i] != null)
                m_Renderers[i].enabled = visible;
        }
    }

    public void OnInteract()
    {
        Debug.Log("[Loot] OnInteract() called. Type=" + lootType + ", Value=" + Value + ", name=" + gameObject.name);
        GameEvents.InvokeLootCollected(Value, lootType);  //this can be for telling player loot has been picked and what item
        GameEvents.InvokeLootCollected();//this one is for sound
        GameEvents.InvokePickUpItem();
        Destroy(gameObject);
    }

    public int GetValue() => Value;
    public float GetRequiredHoldTime() => Value / 2f;
}
