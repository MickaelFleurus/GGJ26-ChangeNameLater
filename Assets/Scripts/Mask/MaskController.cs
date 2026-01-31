using UnityEngine;
using UnityEngine.InputSystem;
using Pathfinding;

/// <summary>
/// Controls mask equip/unequip. When mask is on: item collection allowed, mannequins move toward the player (risk).
/// When mask is off: item collection disabled, mannequins stop moving.
/// </summary>
public class MaskController : MonoBehaviour
{
    [Header("Mask Input")]
    public Key toggleMaskKey = Key.M;

    [Header("Mannequins")]
    [Tooltip("Mannequins with AIPath (IAstarAI). They move toward the player when mask is on, stop when mask is off.")]
    public GameObject[] mannequins = new GameObject[0];

    bool isMaskOn;

    /// <summary>True when the player is wearing the mask. Use this for item collection checks.</summary>
    public bool IsMaskOn => isMaskOn;

    void Start()
    {
        SetMannequinMovement(isMaskOn);
    }

    void Update()
    {
        if (Keyboard.current != null && Keyboard.current[toggleMaskKey].wasPressedThisFrame)
        {
            isMaskOn = !isMaskOn;

            if (isMaskOn)
                GameEvents.InvokeMaskEquipped();
            else
                GameEvents.InvokeMaskOff();

            SetMannequinMovement(isMaskOn);
        }
    }

    void SetMannequinMovement(bool canMove)
    {
        if (mannequins == null) return;

        for (int i = 0; i < mannequins.Length; i++)
        {
            if (mannequins[i] == null) continue;

            var ai = mannequins[i].GetComponent<IAstarAI>();
            if (ai != null)
                ai.canMove = canMove;
        }
    }
}
