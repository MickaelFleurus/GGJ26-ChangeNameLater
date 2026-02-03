
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
            }
            return instance;
        }
    }


    // Events
    public static event Action OnMasterVolumeChanged;
    public static event Action OnMouseSentivityChanged;

    [Header("Display Settings")]
    public bool mFullScreen = true;

    [Header("Audio Settings")]
    [SerializeField]
    [Range(0f, 100f)]
    private float mMasterVolume = 100f;

    public float MasterVolume
    {
        get => mMasterVolume;
        set
        {
            if (mMasterVolume != value)
            {
                mMasterVolume = Mathf.Clamp(value, 0f, 100f);
                OnMasterVolumeChanged?.Invoke();
                Save();
            }
        }
    }


    [Header("Controller Settings")]
    [SerializeField]
    [Range(1f, 100f)]
    private float mMouseSensitivity = 100f;
    public float MouseSensitivity
    {
        get => mMouseSensitivity;
        set
        {
            if (mMouseSensitivity != value)
            {
                mMouseSensitivity = Mathf.Clamp(value, 0f, 100f);
                OnMouseSentivityChanged?.Invoke();
                Save();
            }
        }
    }

    private GameSettings()
    {
    }

    void OnEnable()
    {
        Debug.Log("loading");
        Load();
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
    }
}
