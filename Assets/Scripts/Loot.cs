
using UnityEngine;
public interface IInteractable
{
    void OnInteract();
    int GetValue();
}


public class Loot : MonoBehaviour, IInteractable
{
    [SerializeField] public int Value;

    public void OnInteract() { /* pickup logic */ }
    public int GetValue() { return Value; }
}
