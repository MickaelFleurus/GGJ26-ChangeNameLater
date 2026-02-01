using UnityEngine;

public class MannequinVisibility : MonoBehaviour
{

    private Renderer mRenderer;



    void Start()
    {
        mRenderer = GetComponent<Renderer>();

        GameEvents.OnMaskEquipped += Hide;
        GameEvents.OnMaskOff += Show;
    }

    void OnDestroy()
    {
        GameEvents.OnMaskEquipped -= Hide;
        GameEvents.OnMaskOff -= Show;
    }

    public void Show()
    {
        mRenderer.enabled = true;
    }

    public void Hide()
    {

        mRenderer.enabled = false;
    }

}
