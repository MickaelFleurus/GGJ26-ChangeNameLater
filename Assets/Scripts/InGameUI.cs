using System;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;

public class InGameUI : MonoBehaviour
{
    [SerializeField] public UIDocument UIDocument;
    [SerializeField] MaskController maskController;

    private Label mHints;
    private Label mAmountCollected;
    private Label mLootValue;
    private Slider mMaskTimeSlider;

    private Action mOnMaskOffFunc;
    private Action mOnMaskOnFunc;
    private Action<int, LootType> mOnLootCollectedFunc;

    private bool mCanSeeLoot = false;
    private int mTotalCollected;

    // Hold E to pick up: required time = Value / 2 seconds
    private int mHoldTargetId;
    private float mHoldElapsed;
    private float mHoldRequiredTime;
    private IInteractable mHoldInteractable;

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
        if (maskController == null)
            maskController = FindObjectOfType<MaskController>();

        mHints = UIDocument.rootVisualElement.Q<Label>("Hints");
        mAmountCollected = UIDocument.rootVisualElement.Q<VisualElement>("Collected").Q<Label>("Amount");
        mLootValue = UIDocument.rootVisualElement.Q<Label>("ObjectValue");
        mMaskTimeSlider = UIDocument.rootVisualElement.Q<Slider>("MaskTimeSlider");

        if (mMaskTimeSlider != null && maskController != null)
        {
            mMaskTimeSlider.lowValue = 0f;
            mMaskTimeSlider.highValue = maskController.MaxMaskTime;
        }

        mHints.visible = false;
        mLootValue.visible = false;

        mTotalCollected = 0;
        mAmountCollected.text = mTotalCollected.ToString();
    }

    void ResetHoldState()
    {
        mHoldTargetId = 0;
        mHoldElapsed = 0f;
        mHoldRequiredTime = 0f;
        mHoldInteractable = null;
    }

    void Update()
    {
        if (maskController != null && mMaskTimeSlider != null)
        {
            mMaskTimeSlider.highValue = maskController.MaxMaskTime;
            mMaskTimeSlider.value = maskController.MaxMaskTime - maskController.CurrentMaskTime;
        }

        bool eHeld = Keyboard.current != null && Keyboard.current[Key.E].isPressed;

        if (!mCanSeeLoot)
        {
            ResetHoldState();
            return;
        }

        if (Camera.main == null)
        {
            ResetHoldState();
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
                int value = interactable.GetValue();
                float requiredSeconds = value / 2f;

                if (eHeld)
                {
                    int targetId = hit.collider.GetInstanceID();
                    if (mHoldTargetId != targetId)
                    {
                        mHoldTargetId = targetId;
                        mHoldRequiredTime = requiredSeconds;
                        mHoldElapsed = 0f;
                        mHoldInteractable = interactable;
                    }
                    mHoldElapsed += Time.deltaTime;
                    float remaining = Mathf.Max(0f, mHoldRequiredTime - mHoldElapsed);
                    mLootValue.text = string.Format("{0:F1}s", remaining);

                    if (mHoldElapsed >= mHoldRequiredTime && mHoldInteractable != null)
                    {
                        mHoldInteractable.OnInteract();
                        ResetHoldState();
                    }
                }
                else
                {
                    mLootValue.text = value + " (hold E " + requiredSeconds + "s)";
                    ResetHoldState();
                }
                return;
            }
        }

        ResetHoldState();
        if (mLootValue.visible)
            mLootValue.visible = false;
    }

}
