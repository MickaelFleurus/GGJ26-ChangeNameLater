using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

/// <summary>
/// Listens to OnGameLost: stops BGM/ambience, screen shake, then shows Game Over UI and loads MainMenu after configurable delays.
/// </summary>
public class GameOverController : MonoBehaviour
{
    [Header("Delays (seconds)")]
    [Tooltip("Time after death before showing the Game Over screen.")]
    [SerializeField] float delayBeforeGameOverScreen = 2f;
    [Tooltip("Time the Game Over screen is shown before loading the main menu.")]
    [SerializeField] float delayBeforeMainMenu = 3f;

    [Header("Screen Shake")]
    [SerializeField] float screenShakeDuration = 1f;
    [SerializeField] float screenShakeIntensity = 0.15f;

    bool m_handling;
    float m_shakeTimeLeft;
    float m_shakeIntensity;
    float m_shakeDuration;
    float m_shakeSeed;

    void Start()
    {
        UnityEngine.Cursor.visible = false;
        UnityEngine.Cursor.lockState = CursorLockMode.Locked;
    }

    void OnEnable()
    {
        GameEvents.OnGameLost += HandleGameLost;
    }

    void OnDisable()
    {
        GameEvents.OnGameLost -= HandleGameLost;
    }

    void LateUpdate()
    {
        if (m_shakeTimeLeft <= 0f) return;
        Transform shakeTarget = GetShakeTarget();
        if (shakeTarget == null) return;

        float t = m_shakeTimeLeft / m_shakeDuration;
        float x = (Mathf.PerlinNoise(m_shakeSeed, Time.realtimeSinceStartup * 30f) - 0.5f) * 2f * m_shakeIntensity * t;
        float y = (Mathf.PerlinNoise(m_shakeSeed + 1f, Time.realtimeSinceStartup * 30f) - 0.5f) * 2f * m_shakeIntensity * t;
        shakeTarget.localPosition += new Vector3(x, y, 0f);
        m_shakeTimeLeft -= Time.deltaTime;
    }

    Transform GetShakeTarget()
    {
        var fps = FindObjectOfType<StarterAssets.FirstPersonController>();
        if (fps != null && fps.CinemachineCameraTarget != null)
            return fps.CinemachineCameraTarget.transform;
        var cam = Camera.main;
        return cam != null ? cam.transform : null;
    }

    void HandleGameLost()
    {
        if (m_handling) return;
        m_handling = true;
        StartCoroutine(DeathSequence());
    }

    IEnumerator DeathSequence()
    {
        UnityEngine.Cursor.visible = true;
        UnityEngine.Cursor.lockState = CursorLockMode.None;


        SoundManager.Instance.StopMusic();
        SoundManager.Instance.StopAmbience();


        m_shakeTimeLeft = screenShakeDuration;
        m_shakeIntensity = screenShakeIntensity;
        m_shakeDuration = screenShakeDuration;
        m_shakeSeed = Random.Range(0f, 100f);

        yield return new WaitForSecondsRealtime(screenShakeDuration);

        yield return new WaitForSecondsRealtime(delayBeforeGameOverScreen);

        var inGameUI = FindObjectOfType<InGameUI>();
        if (inGameUI != null && inGameUI.UIDocument != null)
        {
            var panel = inGameUI.UIDocument.rootVisualElement.Q<VisualElement>("GameOverPanel");
            if (panel != null)
                panel.visible = true;
        }

        yield return new WaitForSecondsRealtime(delayBeforeMainMenu);

        SceneManager.LoadScene("MainMenu");
    }
}
