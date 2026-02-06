using UnityEngine;
using Pathfinding;

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
                TriggerDeath("distance");
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


        TriggerDeath("trigger");
    }

    void TriggerDeath(string source)
    {
        s_hasTriggered = true;
        GameEvents.InvokeGameLost();
    }

    bool IsMannequin(GameObject go)
    {
        return go.CompareTag(mannequinTag) || go.GetComponent<IAstarAI>() != null;
    }

}
