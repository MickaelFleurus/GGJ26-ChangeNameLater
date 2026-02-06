using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

public class OptionsPanel : INavigation
{
    public System.Action CanCloseOptions;
    private VisualElement mOptionPanel;
    private Button mApplyButton;
    private Button mBackButton;

    private Toggle mFullScreen;
    private Slider mMouseSensitivity;
    private Slider mMasterVolume;

    private bool mHasChanges = false;
    private bool mHasFocus = false;

    public bool HasFocus { get => mHasFocus; }
    public List<List<VisualElement>> Navigation { get; set; }

    public VisualElement LastSelectedElement { get; set; }

    public OptionsPanel(VisualElement optionPanel, Button applyButton, Button backButton)
    {
        mOptionPanel = optionPanel;
        mApplyButton = applyButton;
        mBackButton = backButton;
        LastSelectedElement = backButton;

        mFullScreen = mOptionPanel.Q<Toggle>("FullScreen");
        mMouseSensitivity = mOptionPanel.Q<Slider>("MouseSensitivity");
        mMasterVolume = mOptionPanel.Q<Slider>("MasterVolume");

        mFullScreen.RegisterValueChangedCallback(_ => CheckSomethingChanged());
        mMouseSensitivity.RegisterValueChangedCallback(_ => CheckSomethingChanged());
        mMasterVolume.RegisterValueChangedCallback(OnMasterVolumeChanged);

        mApplyButton.clicked += ApplyOptions;
        mBackButton.clicked += Hide;

        Navigation = new List<List<VisualElement>>
        {
            new List<VisualElement> { mBackButton, mApplyButton },
            new List<VisualElement> { mMouseSensitivity },
            new List<VisualElement> { mFullScreen },
            new List<VisualElement> { mMasterVolume }
        };
    }

    public void OnShow()
    {
        mHasFocus = true;
        mFullScreen.value = Screen.fullScreen;
        mMouseSensitivity.value = GameSettings.Instance.MouseSensitivity;
        mMasterVolume.value = GameSettings.Instance.MasterVolume;
        mMasterVolume.Focus();
        UpdateApplyButtonState();
    }


    public void Hide()
    {
        GameSettings.InvokeMasterVolumeChanged(GameSettings.Instance.MasterVolume);
        mHasFocus = false;
        CanCloseOptions?.Invoke();
    }

    public void ApplyOptions()
    {
        Screen.fullScreen = mFullScreen.value;
        GameSettings.Instance.mFullScreen = Screen.fullScreen;
        GameSettings.Instance.MouseSensitivity = mMouseSensitivity.value;
        GameSettings.Instance.MasterVolume = mMasterVolume.value;
        UpdateApplyButtonState();
        Hide();
    }

    private void CheckSomethingChanged()
    {
        GameSettings data = GameSettings.Instance;

        if (mFullScreen.value != data.mFullScreen || mMasterVolume.value != data.MasterVolume || mMouseSensitivity.value != data.MouseSensitivity)
        {
            mHasChanges = true;
        }
        else
        {
            mHasChanges = false;
        }
        UpdateApplyButtonState();
    }

    private void OnMasterVolumeChanged(ChangeEvent<float> volume)
    {
        GameSettings.InvokeMasterVolumeChanged(volume.newValue);
        CheckSomethingChanged();
    }

    private void UpdateApplyButtonState()
    {
        if (mHasChanges)
        {
            mApplyButton.RemoveFromClassList("button-disabled");
        }
        else
        {
            mApplyButton.AddToClassList("button-disabled");
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
