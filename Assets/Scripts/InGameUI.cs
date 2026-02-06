using System;

using StarterAssets;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;
using System.Collections.Generic;

public class InGameUI : MonoBehaviour, INavigation
{
    [SerializeField] public UIDocument inGameUIDocument;

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
    private VisualElement mPauseButtons;
    private VisualElement mOptionsPanelVisual;
    private OptionsPanel mOptionPanel;
    private bool mEscapePressed = false;
    private Button mContinueButton;
    private Button mQuitButton;
    private Button mOptionsButton;
    private Button mBackOptionsButton;

    private float mHintTimeLeft;
    private float mHintDuration = 10.0f;
    public List<List<VisualElement>> Navigation { get; set; }
    public VisualElement LastSelectedElement { get; set; }

    void Awake()
    {
        NavigationExtensions.SetupFocusGuard(this, inGameUIDocument.rootVisualElement);
        mPauseButtons = inGameUIDocument.rootVisualElement.Q<VisualElement>("PauseButtons");
        mPauseMenu = inGameUIDocument.rootVisualElement.Q<VisualElement>("PauseMenu");
        mContinueButton = inGameUIDocument.rootVisualElement.Q<Button>("ContinueButton");
        mQuitButton = inGameUIDocument.rootVisualElement.Q<Button>("QuitButton");
        mOptionsButton = inGameUIDocument.rootVisualElement.Q<Button>("OptionsButton");
        mBackOptionsButton = inGameUIDocument.rootVisualElement.Q<Button>("BackOptions");
        mOptionsPanelVisual = inGameUIDocument.rootVisualElement.Q<VisualElement>("OptionsPanel");

        mOptionPanel = new OptionsPanel(mOptionsPanelVisual, inGameUIDocument.rootVisualElement.Q<Button>("Apply"), mBackOptionsButton);
        mOptionPanel.CanCloseOptions += HideOptions;

        mContinueButton.clicked += HidePause;
        mQuitButton.clicked += CloseGame;
        mOptionsButton.clicked += ShowOptions;
        LastSelectedElement = mContinueButton;

        Navigation = new List<List<VisualElement>>
        {
            new List<VisualElement> {mQuitButton},
            new List<VisualElement> {mOptionsButton},
            new List<VisualElement> {mContinueButton}
        };

        mOnMaskOffFunc = () =>
            {
                mCanSeeLoot = false;
                mLootValue.visible = false;
                ResetHoldState();
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

        inGameUIDocument.rootVisualElement.RegisterCallback<NavigationMoveEvent>(OnMove);

        mHints = inGameUIDocument.rootVisualElement.Q<Label>("Hints");
        mAmountCollected = inGameUIDocument.rootVisualElement.Q<VisualElement>("Collected").Q<Label>("Amount");
        mLootValue = inGameUIDocument.rootVisualElement.Q<Label>("ObjectValue");
        mMaskTimeSlider = inGameUIDocument.rootVisualElement.Q<Slider>("MaskTimeSlider");

        mMaskTimeSlider.lowValue = 0f;
        mMaskTimeSlider.highValue = maskController.MaxMaskTime;

        mHints.visible = false;

        var gameOverPanel = inGameUIDocument.rootVisualElement.Q<VisualElement>("GameOverPanel");
        gameOverPanel.style.display = DisplayStyle.None;

        mTotalCollected = 0;
        GameEvents.CurrentMoney = mTotalCollected;
        mAmountCollected.text = mTotalCollected.ToString();
        ShowHint("Press F to put on the mask. You can see and collect item this way. Be careful, the mannequin moves when the mask is on...");
    }

    private void ShowOptions()
    {
        mPauseButtons.style.display = DisplayStyle.None;
        mOptionsPanelVisual.style.display = DisplayStyle.Flex;
        mOptionPanel.OnShow();
    }

    void ResetHoldState()
    {
        if (mHoldInteractable != null)
        {
            mHoldTargetId = 0;
            mHoldElapsed = 0f;
            mHoldRequiredTime = 0f;
            mHoldInteractable = null;
        }
    }

    void Update()
    {
        if (mHints.visible)
        {
            mHintTimeLeft = mHintTimeLeft - Time.unscaledDeltaTime;
            if (mHintTimeLeft <= 0.0f)
            {
                mHints.visible = false;
            }
        }

        // Handle pause menu input (works even when paused)
        HandlePauseInput();

        if (mPauseMenu.style.display == DisplayStyle.Flex) return;

        mMaskTimeSlider.highValue = maskController.MaxMaskTime;
        mMaskTimeSlider.value = maskController.MaxMaskTime - maskController.CurrentMaskTime;

        if (!mCanSeeLoot)
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
                    }
                    else if (GameEvents.CurrentMoney >= door.GetValue() && !isUnlocked)
                    {
                        GameEvents.InvokeDoorUnlocked();
                        isUnlocked = true;
                    }
                    return;
                }

                bool eHeld = Keyboard.current != null && Keyboard.current[Key.E].isPressed;
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


    private void HandlePauseInput()
    {
        if (Keyboard.current == null)
            return;

        bool escapeCurrentlyPressed = Keyboard.current[Key.Escape].isPressed;

        // Detect key press (transition from not pressed to pressed)
        if (escapeCurrentlyPressed && !mEscapePressed)
        {
            if (mPauseMenu.style.display != DisplayStyle.Flex)
                ShowPause();
            else if (mOptionPanel.HasFocus)
            {
                mOptionPanel.Hide();
                HideOptions();
            }
            else
                HidePause();
        }

        mEscapePressed = escapeCurrentlyPressed;
    }

    private void ShowPause()
    {
        mPauseMenu.style.display = DisplayStyle.Flex;
        mContinueButton.focusable = true;
        GameEvents.InvokeGamePaused(true);
        Time.timeScale = 0f;

        UnityEngine.Cursor.lockState = CursorLockMode.Confined;
        UnityEngine.Cursor.visible = true;
        mContinueButton.schedule.Execute(() =>
        {
            mContinueButton.Focus();
        });
    }

    private void HidePause()
    {
        mPauseMenu.style.display = DisplayStyle.None;
        GameEvents.InvokeGamePaused(false);
        Time.timeScale = 1f;

        UnityEngine.Cursor.lockState = CursorLockMode.Locked;
        UnityEngine.Cursor.visible = false;

        mEscapePressed = false;
    }

    private void CloseGame()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }

    void HideOptions()
    {
        mPauseButtons.style.display = DisplayStyle.Flex;
        mOptionsPanelVisual.style.display = DisplayStyle.None;
        mContinueButton.schedule.Execute(() =>
        {
            mContinueButton.Focus();
        });
    }

    void OnMove(NavigationMoveEvent evt)
    {
        if (mOptionPanel.HasFocus)
        {
            mOptionPanel.MoveFocus(evt);
        }
        else
        {
            MoveFocus(evt);
        }
    }

    public (int row, int col, bool found) GetFocusedElementPosition()
    {
        return NavigationExtensions.GetFocusedElementPosition(this);
    }

    public void SetFocusAt(VisualElement element)
    {
        NavigationExtensions.SetFocusAt(element);
    }

    public void SetFocusAt(int row, int col)
    {
        NavigationExtensions.SetFocusAt(this, row, col);
    }

    public void MoveFocus(NavigationMoveEvent evt)
    {
        NavigationExtensions.MoveFocus(this, evt);
    }
}
