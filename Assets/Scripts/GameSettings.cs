
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
    private float mMouseSensitivity = 0.15f;
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

    private const string WebGLPrefsKey = "GameSettings";

    public void Save()
    {
        string json = JsonUtility.ToJson(this, true);

#if UNITY_WEBGL && !UNITY_EDITOR
        // WebGL: use PlayerPrefs (LocalStorage) - File I/O can cause "Permissions check failed" in iframes
        PlayerPrefs.SetString(WebGLPrefsKey, json);
        PlayerPrefs.Save();
        Debug.Log("Settings saved (WebGL PlayerPrefs)");
#else
        // Standalone/Editor: save to JSON file
        string savePath = Path.Combine(Application.persistentDataPath, "GameSettings.json");
        File.WriteAllText(savePath, json);
        Debug.Log($"Settings saved to {savePath}");
#endif
    }

    public void Load()
    {
#if UNITY_WEBGL && !UNITY_EDITOR
        // WebGL: load from PlayerPrefs (LocalStorage)
        if (PlayerPrefs.HasKey(WebGLPrefsKey))
        {
            string json = PlayerPrefs.GetString(WebGLPrefsKey);
            JsonUtility.FromJsonOverwrite(json, this);
            Debug.Log("Settings loaded (WebGL PlayerPrefs)");
        }
#else
        // Standalone/Editor: load from JSON file
        string loadPath = Path.Combine(Application.persistentDataPath, "GameSettings.json");
        if (File.Exists(loadPath))
        {
            string json = File.ReadAllText(loadPath);
            JsonUtility.FromJsonOverwrite(json, this);
            Debug.Log($"Settings loaded from {loadPath}");
        }
#endif

        Screen.SetResolution(mScreenWidth, mScreenHeight,
        mFullScreen ? FullScreenMode.FullScreenWindow : FullScreenMode.Windowed);
    }
}
