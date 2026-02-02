using System;

using StarterAssets;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;

public class InGameUI : MonoBehaviour, MenuInputs.IMenuActions
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
    private Toggle mMusicOnToggle;
    private Slider mMasterVolumeSlider;
    private Slider mMouseSensitivitySlider;
    private MenuInputs mInputs;
    private int? mSelectedPauseIndex = 0;
    private bool mJustUnpaused = false; // Hacky solution since we use both modern inputs and old

    private float mHintTimeLeft;
    private float mHintDuration = 10.0f;

    void OnEnable()
    {
        mInputs.Menu.SetCallbacks(this);
    }

    void OnDisable()
    {
        mInputs.Menu.Disable();
    }

    void Awake()
    {
        mPauseMenu = UIDocument.rootVisualElement.Q<VisualElement>("PauseMenu");
        mInputs = new MenuInputs();
        mPauseButtons = new VisualElement[4];
        mPauseButtons[0] = mPauseMenu.Q<VisualElement>("SoundOn");
        mMusicOnToggle = mPauseMenu.Q<Toggle>("SoundOn");
        mPauseButtons[1] = mPauseMenu.Q<VisualElement>("SoundVolume");
        mMasterVolumeSlider = mPauseMenu.Q<Slider>("SoundVolume");
        mPauseButtons[2] = mPauseMenu.Q<VisualElement>("MouseSensitivity");
        mMouseSensitivitySlider = mPauseMenu.Q<Slider>("MouseSensitivity");
        mPauseButtons[3] = mPauseMenu.Q<VisualElement>("Quit");
        mPauseMenu.visible = false;
        mMasterVolumeSlider.lowValue = 0.0f;
        mMasterVolumeSlider.highValue = 1.0f;
        mMasterVolumeSlider.value = 1.0f;
        mMasterVolumeSlider.RegisterValueChangedCallback(ChangeMasterVolume);

        mMouseSensitivitySlider.lowValue = 1.0f;
        mMouseSensitivitySlider.highValue = 50.0f;
        mMouseSensitivitySlider.value = controller.RotationSpeed;
        mMouseSensitivitySlider.RegisterValueChangedCallback(ChangeMouseSensitivity);


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
        mInputs.Dispose();
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
        ShowHint("Press F to put on the mask. You can see and collect item this way. Be careful, the mannequin moves when the mask is on...");
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

    private void ResolveActive()
    {
        if (!mSelectedPauseIndex.HasValue)
        {
            foreach (VisualElement btn in mPauseButtons)
            {
                btn.AddToClassList("unselected");
                btn.RemoveFromClassList("selected");
                btn.RemoveFromClassList("pressed");
            }
        }

        for (int i = 0; i < mPauseButtons.Length; i++)
        {
            if (i == mSelectedPauseIndex)
            {
                if (i == 3) // Quit button special case
                {

                    mPauseButtons[i].AddToClassList("selected");
                    mPauseButtons[i].RemoveFromClassList("unselected");
                }
                else
                {
                    mPauseButtons[i].AddToClassList("pause_selected");
                }
            }
            else
            {
                if (i == 3) // Quit button special case
                {
                    mPauseButtons[i].RemoveFromClassList("selected");
                    mPauseButtons[i].AddToClassList("unselected");
                }
                else
                {
                    mPauseButtons[i].RemoveFromClassList("pause_selected");
                }
            }

        }
    }



    public void OnUp(InputAction.CallbackContext context)
    {
        if (!context.performed) return;

        if (!mSelectedPauseIndex.HasValue) { mSelectedPauseIndex = 0; }
        else
        {
            mSelectedPauseIndex--;
            if (mSelectedPauseIndex < 0)
            {
                mSelectedPauseIndex = mPauseButtons.Length - 1;
            }
        }
        ResolveActive();
    }

    public void OnDown(InputAction.CallbackContext context)
    {
        if (!context.performed) return;

        if (!mSelectedPauseIndex.HasValue) { mSelectedPauseIndex = 0; }
        else
        {
            mSelectedPauseIndex++;
            if (mSelectedPauseIndex >= mPauseButtons.Length)
            {
                mSelectedPauseIndex = 0;
            }
        }
        ResolveActive();
    }

    public void OnSelect(InputAction.CallbackContext context)
    {
        if (context.control.device is Mouse)
        {
            return;
        }
        if (!mSelectedPauseIndex.HasValue)
        {
            return;
        }

        switch (mSelectedPauseIndex)
        {
            case 0:

                mMusicOnToggle.value = !mMusicOnToggle.value;
                if (!mMusicOnToggle.value)
                {
                    mMasterVolumeSlider.value = 0.0f;
                }
                else
                {
                    mMasterVolumeSlider.value = 0.5f;
                }
                break;
            case 3: // Quit
#if UNITY_EDITOR
                UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
                break;
        }
    }

    public void OnBack(InputAction.CallbackContext context)
    {
        if (!context.performed) return;
        if (mPauseMenu.visible)
        {
            HidePause();
        }
        else
        {
            ShowPause();
        }
    }

    public void OnLower(InputAction.CallbackContext context)
    {
        if (!mSelectedPauseIndex.HasValue) return;
        if (context.ReadValue<float>() > 0)
        {
            switch (mSelectedPauseIndex)
            {
                case 1: // Volume
                    mMasterVolumeSlider.value = mMasterVolumeSlider.value - 0.1f;
                    break;
                case 2: // Mouse sensitivity
                    mMouseSensitivitySlider.value = mMouseSensitivitySlider.value - 1f;
                    break;
                default:
                    break;
            }
        }

    }
    public void OnIncrease(InputAction.CallbackContext context)
    {
        if (!mSelectedPauseIndex.HasValue) return;

        if (context.ReadValue<float>() > 0)
        {
            switch (mSelectedPauseIndex)
            {
                case 1: // Volume
                    mMasterVolumeSlider.value = mMasterVolumeSlider.value + 0.1f;
                    break;
                case 2: // Mouse sensitivity
                    mMouseSensitivitySlider.value = mMouseSensitivitySlider.value + 1f;
                    break;
                default:
                    break;
            }
        }
    }

    private void ShowPause()
    {
        Time.timeScale = 0f;
        mPauseMenu.visible = true;
        mInputs.Enable();
        GameEvents.InvokeGamePaused(true);
    }

    private void HidePause()
    {
        Time.timeScale = 1f;
        mPauseMenu.visible = false;
        mInputs.Disable();
        GameEvents.InvokeGamePaused(false);
        mJustUnpaused = true;
    }

    private void ChangeMasterVolume(ChangeEvent<float> evt)
    {
        SoundManager.Instance.SetMasterVolume(evt.newValue);

        mMusicOnToggle.value = evt.newValue > 0.0f;
    }

    private void ChangeMouseSensitivity(ChangeEvent<float> evt)
    {
        controller.RotationSpeed = evt.newValue;
    }

}
