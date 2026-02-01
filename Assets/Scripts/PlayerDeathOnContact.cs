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

    MaskController m_maskController;

    void Start()
    {
        s_hasTriggered = false; // reset when game scene loads so new game can trigger again
        m_maskController = FindObjectOfType<MaskController>();
        Debug.Log($"[PlayerDeathOnContact] Start: MaskController={(m_maskController != null ? "found" : "NOT FOUND")}, mannequinTag=\"{mannequinTag}\", contactDistance={contactDistance}");
    }

    void Update()
    {
        if (s_hasTriggered) return;
        if (m_maskController == null || !m_maskController.IsMaskOn) return;

        var mannequins = m_maskController.mannequins;
        if (mannequins == null || mannequins.Length == 0) return;

        Vector3 playerPos = transform.position;
        for (int i = 0; i < mannequins.Length; i++)
        {
            if (mannequins[i] == null) continue;
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
        if (s_hasTriggered)
        {
            return;
        }
        if (m_maskController == null)
        {
            return;
        }
        if (!m_maskController.IsMaskOn)
        {
            return;
        }
        if (!IsMannequin(other.gameObject))
        {
            var go = other.gameObject;
            bool tagMatch = !string.IsNullOrEmpty(mannequinTag) && go.CompareTag(mannequinTag);
            bool hasAI = go.GetComponent<IAstarAI>() != null;
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
        if (go == null) return false;
        if (!string.IsNullOrEmpty(mannequinTag) && go.CompareTag(mannequinTag))
            return true;
        return go.GetComponent<IAstarAI>() != null;
    }

}
