using System;

using StarterAssets;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;

public class InGameUI : MonoBehaviour
{
    [SerializeField] public UIDocument UIDocument;

    [SerializeField] public FirstPersonController controller;
    [SerializeField] MaskController maskController;

    private Label mHints;
    private Label mAmountCollected;
    private Label mLootValue;
    private Slider mMaskTimeSlider;

    private Action mOnMaskOffFunc;
    private Action mOnMaskOnFunc;
    private Action<int, LootType> mOnLootCollectedFunc;

    private bool mCanSeeLoot = false;
    private bool isUnlocked = false;
    private int mTotalCollected;

    // Hold E to pick up: required time = Value / 2 seconds
    private int mHoldTargetId;
    private float mHoldElapsed;
    private float mHoldRequiredTime;
    private IInteractable mHoldInteractable;

    // Pause menu
    private VisualElement mPauseMenu;
    private VisualElement[] mPauseButtons;
    private bool mJustUnpaused = false; // Hacky solution since we use both modern inputs and old

    private float mHintTimeLeft;
    private float mHintDuration = 10.0f;

    void Awake()
    {
        mPauseMenu = UIDocument.rootVisualElement.Q<VisualElement>("PauseMenu");


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
            GameEvents.CurrentMoney = mTotalCollected;
            if (mAmountCollected != null)
                mAmountCollected.text = mTotalCollected.ToString();
        };

        GameEvents.OnMaskOff += mOnMaskOffFunc;
        GameEvents.OnMaskEquipped += mOnMaskOnFunc;
        GameEvents.OnLootCollectedWithData += mOnLootCollectedFunc;

    }

    void ShowHint(string hint)
    {
        mHintTimeLeft = mHintDuration;
        mHints.visible = true;
        mHints.text = hint;
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

        var gameOverPanel = UIDocument.rootVisualElement.Q<VisualElement>("GameOverPanel");
        if (gameOverPanel != null)
            gameOverPanel.visible = false;

        mTotalCollected = 0;
        GameEvents.CurrentMoney = mTotalCollected;
        mAmountCollected.text = mTotalCollected.ToString();
        ShowHint("Press M to put on the mask. You can see and collect item this way. Be careful, the mannequin moves when the mask is on...");
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
        if (mHints.visible)
        {
            mHintDuration = mHintDuration - Time.deltaTime;
            if (mHintDuration <= 0.0f)
            {
                mHints.visible = false;
            }
        }
        if (mPauseMenu.visible) return;
        // Handle pause menu showing
        if (!mJustUnpaused && Keyboard.current != null && Keyboard.current[Key.Escape].wasPressedThisFrame)
        {
            ShowPause();
            return;
        }

        mJustUnpaused = false;
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
        float rayDistance = 6f;

        if (Physics.Raycast(ray, out hit, rayDistance))
        {
            IInteractable interactable = hit.collider.GetComponentInChildren<IInteractable>();

            if (interactable != null)
            {
                mLootValue.visible = true;
                float requiredSeconds = interactable.GetRequiredHoldTime();

                // Door: require enough money; show "Need X money" and block hold if not
                if (interactable is Door door)
                {
                    if (GameEvents.CurrentMoney < door.GetValue())
                    {
                        mLootValue.text = "Need " + door.GetValue() + " money";
                        ResetHoldState();
                        return;
                    }
                    else if (GameEvents.CurrentMoney >= door.GetValue() && !isUnlocked)
                    {
                        GameEvents.InvokeDoorUnlocked();
                        isUnlocked = true;
                        return;
                    }

                }

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
                    mLootValue.text = interactable.GetValue() + " (hold E " + requiredSeconds + "s)";
                    ResetHoldState();
                }
                return;
            }
        }

        ResetHoldState();
        if (mLootValue.visible)
            mLootValue.visible = false;
    }


    // Pause menu handling logic


    private void ShowPause()
    {
        Time.timeScale = 0f;
        mPauseMenu.visible = true;
        GameEvents.InvokeGamePaused(true);
    }

    private void HidePause()
    {
        Time.timeScale = 1f;
        mPauseMenu.visible = false;
        GameEvents.InvokeGamePaused(false);
        mJustUnpaused = true;
    }
}
