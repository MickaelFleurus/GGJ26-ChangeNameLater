using System;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;

public class UI : MonoBehaviour
{
    [SerializeField] public UIDocument UIDocument;


    private Label mHints;
    private Label mAmountCollected;
    private Label mLootValue;

    private Action mOnMaskOffFunc;
    private Action mOnMaskOnFunc;

    private bool mCanSeeLoot = false;

    void Awake()
    {

        mOnMaskOffFunc = () =>
        {
            mCanSeeLoot = false;
        };
        mOnMaskOnFunc = () =>
        {
            mCanSeeLoot = true;
        };

        GameEvents.OnMaskOff += mOnMaskOffFunc;
        GameEvents.OnMaskEquipped += mOnMaskOnFunc;
    }
    void OnDestroy()
    {
        GameEvents.OnMaskOff -= mOnMaskOffFunc;
        GameEvents.OnMaskEquipped -= mOnMaskOnFunc;
    }

    void Start()
    {
        mHints = UIDocument.rootVisualElement.Q<Label>("Hints");
        mAmountCollected = UIDocument.rootVisualElement.Q<VisualElement>("Collected").Q<Label>("Amount");
        mLootValue = UIDocument.rootVisualElement.Q<Label>("ObjectValue");

        mHints.visible = false;
        mLootValue.visible = false;

        mAmountCollected.text = "0";
    }

    void Update()
    {
        bool ePressed = Keyboard.current != null && Keyboard.current[Key.E].wasPressedThisFrame;

        if (!mCanSeeLoot)
        {
            if (ePressed)
                Debug.Log("[UI] E pressed but mask is off (mCanSeeLoot=false). Put mask on first.");
            return;
        }

        if (Camera.main == null)
        {
            if (ePressed)
                Debug.Log("[UI] E pressed but Camera.main is null. Set Main Camera tag.");
            return;
        }

        Ray ray = new Ray(Camera.main.transform.position, Camera.main.transform.forward);
        RaycastHit hit;
        float rayDistance = 5f;

        if (Physics.Raycast(ray, out hit, rayDistance))
        {
            IInteractable interactable = hit.collider.GetComponent<IInteractable>();

            if (interactable != null)
            {
                mLootValue.visible = true;
                mLootValue.text = interactable.GetValue().ToString();
                if (ePressed)
                {
                    Debug.Log("[UI] E pressed, calling OnInteract() on: " + hit.collider.gameObject.name);
                    interactable.OnInteract();
                }
                return;
            }

            if (ePressed)
                Debug.Log("[UI] E pressed but hit object has no IInteractable: " + hit.collider.gameObject.name);
        }
        else
        {
            if (ePressed)
                Debug.Log("[UI] E pressed but raycast hit nothing (look at item within 5m, mask on).");
        }

        if (mLootValue.visible)
        {
            mLootValue.visible = false;
        }
    }

}
