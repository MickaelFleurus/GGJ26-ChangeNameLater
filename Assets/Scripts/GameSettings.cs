
using System;
using System.IO;
using UnityEngine;

[CreateAssetMenu(fileName = "GameSettings", menuName = "ScriptableObjects/GameSettings")]
public class GameSettings : ScriptableObject
{
    private static GameSettings instance;
    public static GameSettings Instance
    {
        get
        {
            if (instance == null)
            {
                instance = CreateInstance<GameSettings>();
                instance.Load();
            }
            return instance;
        }
    }


    // Events
    public static event Action<float> OnMasterVolumeChanged;
    public static event Action<float> OnMouseSentivityChanged;
    public static void InvokeMasterVolumeChanged(float volume) => OnMasterVolumeChanged?.Invoke(volume);
    public static void InvokeMouseSentivityChanged(float sensitivity) => OnMouseSentivityChanged?.Invoke(sensitivity);

    [Header("Display Settings")]
    public bool mFullScreen = true;
    public int mScreenWidth = 1920;
    public int mScreenHeight = 1080;

    [Header("Audio Settings")]
    [SerializeField]
    [Range(0f, 1f)]
    private float mMasterVolume = 100f;

    public float MasterVolume
    {
        get => mMasterVolume;
        set
        {
            if (mMasterVolume != value)
            {
                mMasterVolume = Mathf.Clamp(value, 0f, 1f);
                OnMasterVolumeChanged?.Invoke(mMasterVolume);
                Save();
            }
        }
    }


    [Header("Controller Settings")]
    [SerializeField]
    [Range(0f, 1f)]
    private float mMouseSensitivity = 1f;
    public float MouseSensitivity
    {
        get => mMouseSensitivity;
        set
        {
            if (mMouseSensitivity != value)
            {
                mMouseSensitivity = Mathf.Clamp(value, 0f, 1f);
                OnMouseSentivityChanged?.Invoke(mMouseSensitivity);
                Save();
            }
        }
    }

    private GameSettings()
    {
    }

    public void ResetToDefaults()
    {
        mMouseSensitivity = 100f;
        mMasterVolume = 100f;
        mFullScreen = true;
        Save();
    }

    public void Save()
    {
        // Save to JSON file
        string json = JsonUtility.ToJson(this, true);
        string savePath = Path.Combine(Application.persistentDataPath, "GameSettings.json");
        File.WriteAllText(savePath, json);
        Debug.Log($"Settings saved to {savePath}");
    }

    public void Load()
    {
        // Load from JSON file
        string loadPath = Path.Combine(Application.persistentDataPath, "GameSettings.json");
        if (File.Exists(loadPath))
        {
            string json = File.ReadAllText(loadPath);
            JsonUtility.FromJsonOverwrite(json, this);
            Debug.Log($"Settings loaded from {loadPath}");
        }

        Screen.SetResolution(mScreenWidth, mScreenHeight,
        mFullScreen ? FullScreenMode.FullScreenWindow : FullScreenMode.Windowed);
    }
}
