using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;


public class OptionsPanel
{

    public System.Action CanCloseOptions;
    private VisualElement mOptionPanel;
    private Button mApplyButton;

    private Toggle mFullScreen;
    private Slider mMouseSensitivity;
    private Slider mMasterVolume;

    public OptionsPanel(VisualElement optionPanel, Button applyButton)
    {
        mOptionPanel = optionPanel;
        mApplyButton = applyButton;

        mFullScreen = mOptionPanel.Q<Toggle>("FullScreen");
        mMouseSensitivity = mOptionPanel.Q<Slider>("MouseSensitivity");
        mMasterVolume = mOptionPanel.Q<Slider>("MasterVolume");

        mFullScreen.RegisterValueChangedCallback(_ => CheckSomethingChanged());
        mMouseSensitivity.RegisterValueChangedCallback(_ => CheckSomethingChanged());
        mMasterVolume.RegisterValueChangedCallback(_ => CheckSomethingChanged());

        mApplyButton.clicked += ApplyOptions;
    }

    public void Show()
    {
        mFullScreen.value = Screen.fullScreen;
        mMouseSensitivity.value = GameSettings.Instance.MouseSensitivity;
        mMasterVolume.value = GameSettings.Instance.MasterVolume;
    }

    public void ApplyOptions()
    {
        Screen.fullScreen = mFullScreen.value;
        GameSettings.Instance.mFullScreen = Screen.fullScreen;
        GameSettings.Instance.MouseSensitivity = mMouseSensitivity.value;
        GameSettings.Instance.MasterVolume = mMasterVolume.value;
        CanCloseOptions?.Invoke();
    }

    private void CheckSomethingChanged()
    {
        GameSettings data = GameSettings.Instance;

        if (mFullScreen.value != data.mFullScreen || mMasterVolume.value != data.MasterVolume || mMouseSensitivity.value != data.MouseSensitivity)
        {
            mApplyButton.SetEnabled(true);
        }
        else
        {
            mApplyButton.SetEnabled(false);
        }

    }
}
