using UnityEngine;
using Pathfinding;
using StarterAssets;

/// <summary>
/// When the player (mask ON) touches a mannequin, invokes GameEvents.InvokeGameLost() once per game.
/// Uses both OnTriggerEnter and a per-frame distance check so fast movement doesn't tunnel past.
/// </summary>
public class PlayerDeathOnContact : MonoBehaviour
{
    [Tooltip("Tag used on mannequin root/body. If empty, mannequins are detected by IAstarAI component.")]
    [SerializeField] string mannequinTag = "Mannequin";
    [Tooltip("Max distance to mannequin to count as contact (used when moving fast so trigger might miss).")]
    [SerializeField] float contactDistance = 1.5f;

    static bool s_hasTriggered;

    [SerializeField] MaskController m_maskController;
    [SerializeField] FirstPersonController m_playerController;

    void Start()
    {
        s_hasTriggered = false; // reset when game scene loads so new game can trigger again
    }

    void Update()
    {
        if (s_hasTriggered) return;
        if (!m_maskController.IsMaskOn) return;

        var mannequins = m_maskController.mannequins;
        if (mannequins.Length == 0) return;

        Vector3 playerPos = transform.position;
        for (int i = 0; i < mannequins.Length; i++)
        {
            float d = Vector3.Distance(playerPos, mannequins[i].transform.position);
            if (d <= contactDistance)
            {
                TriggerDeath("distance", mannequins[i]);
                return;
            }
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (s_hasTriggered || !m_maskController.IsMaskOn || !IsMannequin(other.gameObject))
        {
            return;
        }


        TriggerDeath("trigger", other.gameObject);
    }

    void TriggerDeath(string source, GameObject mannequin)
    {
        s_hasTriggered = true;
        var mannequins = m_maskController.mannequins;
        for (int i = 0; i < mannequins.Length; i++)
        { mannequins[i].GetComponent<IAstarAI>().canMove = false; }
        m_maskController.MaskOff();
        m_playerController.TriggerDeathAnimation(mannequin);
        GameEvents.InvokeDyingAnimationStart();
    }

    bool IsMannequin(GameObject go)
    {
        return go.CompareTag(mannequinTag) || go.GetComponent<IAstarAI>() != null;
    }

}
