using UnityEngine;

/// <summary>
/// Syncs Q_Vignette_Single's mainScale with mask state. Mask on/off scales are configurable in the Inspector.
/// Does not modify Q_Vignette_Single; only sets its mainScale and calls its public methods.
/// </summary>
public class VignetteMaskScale : MonoBehaviour
{
    [Header("Vignette")]
    [Tooltip("Assign Q_Vignette_Single. If empty, will try to find one in the scene.")]
    [SerializeField] Q_Vignette_Single vignette;

    [Header("Scale when mask is ON")]
    [SerializeField] [Range(0f, 5f)] float maskOnScale = 0.4f;

    [Header("Scale when mask is OFF")]
    [SerializeField] [Range(0f, 5f)] float maskOffScale = 0.6f;

    void Awake()
    {
        if (vignette == null)
            vignette = FindObjectOfType<Q_Vignette_Single>();
    }

    void Start()
    {
        GameEvents.OnMaskEquipped += OnMaskOn;
        GameEvents.OnMaskOff += OnMaskOff;

        var maskController = FindObjectOfType<MaskController>();
        if (maskController != null)
            ApplyScale(maskController.IsMaskOn ? maskOnScale : maskOffScale);
    }

    void OnDestroy()
    {
        GameEvents.OnMaskEquipped -= OnMaskOn;
        GameEvents.OnMaskOff -= OnMaskOff;
    }

    void OnMaskOn() => ApplyScale(maskOnScale);
    void OnMaskOff() => ApplyScale(maskOffScale);

    void ApplyScale(float scale)
    {
        if (vignette == null) return;
        scale = Mathf.Clamp(scale, 0f, 5f);
        vignette.mainScale = scale;
        vignette.SetVignetteMainScale(scale);
        vignette.SetVignetteSkyScale(scale);
    }
}
