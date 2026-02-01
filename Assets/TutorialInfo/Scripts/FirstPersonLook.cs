using UnityEngine;
using System;
public class FirstPersonLook : MonoBehaviour
{
    public Transform cameraPivot;
    public float mouseSensitivity = 2f;
    public float minPitch = -80f;
    public float maxPitch = 80f;

    float pitch = 0f;

    private bool mGamePause = false;
    private Action<bool> mGamePausedFunc;

    void Awake()
    {
        mGamePausedFunc = (bool paused) =>
        {
            mGamePause = paused;
        };
        GameEvents.OnGamePausedChanged += mGamePausedFunc;
    }

    void Destroy()
    {
        GameEvents.OnGamePausedChanged -= mGamePausedFunc;
    }

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void Update()
    {
        if (mGamePause) { return; }
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity;

        // Rotate player left/right
        transform.Rotate(Vector3.up * mouseX);

        // Rotate camera up/down
        pitch -= mouseY;
        pitch = Mathf.Clamp(pitch, minPitch, maxPitch);
        cameraPivot.localRotation = Quaternion.Euler(pitch, 0f, 0f);
    }
}
