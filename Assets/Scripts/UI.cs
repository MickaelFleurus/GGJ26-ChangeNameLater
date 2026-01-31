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
    private Action<int, LootType> mOnLootCollectedFunc;

    private bool mCanSeeLoot = false;
    private int mTotalCollected;

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
        mOnLootCollectedFunc = (value, lootType) =>
        {
            mTotalCollected += value;
            if (mAmountCollected != null)
                mAmountCollected.text = mTotalCollected.ToString();
        };

        GameEvents.OnMaskOff += mOnMaskOffFunc;
        GameEvents.OnMaskEquipped += mOnMaskOnFunc;
        GameEvents.OnLootCollectedWithData += mOnLootCollectedFunc;
    }

    void OnDestroy()
    {
        GameEvents.OnMaskOff -= mOnMaskOffFunc;
        GameEvents.OnMaskEquipped -= mOnMaskOnFunc;
        GameEvents.OnLootCollectedWithData -= mOnLootCollectedFunc;
    }

    void Start()
    {
        mHints = UIDocument.rootVisualElement.Q<Label>("Hints");
        mAmountCollected = UIDocument.rootVisualElement.Q<VisualElement>("Collected").Q<Label>("Amount");
        mLootValue = UIDocument.rootVisualElement.Q<Label>("ObjectValue");

        mHints.visible = false;
        mLootValue.visible = false;

        mTotalCollected = 0;
        mAmountCollected.text = mTotalCollected.ToString();
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
