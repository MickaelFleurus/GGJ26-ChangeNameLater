using Pathfinding;
using UnityEngine;

public class MannequinVisibility : MonoBehaviour
{
    private Renderer mRenderer;
    [SerializeField] private AIPath aiPath;



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
        if (aiPath.velocity.magnitude > 0.001f)
        {
            Vector3 directionToCamera = (Camera.main.transform.position - transform.position).normalized;

            Quaternion targetRotation = Quaternion.LookRotation(directionToCamera);
            transform.rotation = Quaternion.Euler(transform.eulerAngles.x, targetRotation.eulerAngles.y + 90.0f, transform.eulerAngles.z);
        }
        mRenderer.enabled = true;
    }

    public void Hide()
    {
        mRenderer.enabled = false;
    }
}
