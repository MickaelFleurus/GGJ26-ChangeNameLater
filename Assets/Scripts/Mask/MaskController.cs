using UnityEngine;
using UnityEngine.InputSystem;
using Pathfinding;
using System;

/// <summary>
/// Controls mask equip/unequip. When mask is on: item collection allowed, mannequins move toward the player (risk).
/// When mask is off: item collection disabled, mannequins stop moving.
/// </summary>
public class MaskController : MonoBehaviour
{
    [Header("Mask Input")]
    public Key toggleMaskKey = Key.M;

    [Header("Mannequins")]
    [Tooltip("Mannequins with AIPath (IAstarAI). They move toward the player when mask is on, stop when mask is off.")]
    public GameObject[] mannequins = new GameObject[0];

    [Header("Mask Time")]
    [Tooltip("Max seconds when mask is off. Refills when mask is on. Decreases 1/s when off, increases 1/s when on.")]
    public float maxMaskOffTime = 100f;

    float mCurrentMaskTime;
    float mAccumulator;
    bool mHasTriggeredGameOver;

    bool isMaskOn;

    /// <summary>True when the player is wearing the mask. Use this for item collection checks.</summary>
    public bool IsMaskOn => isMaskOn;

    public float CurrentMaskTime => mCurrentMaskTime;
    public float MaxMaskTime => maxMaskOffTime;

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
        mCurrentMaskTime = maxMaskOffTime;
        SetMannequinMovement(isMaskOn);
    }

    void Update()
    {
        if (mGamePause) { return; }
        if (Keyboard.current != null && Keyboard.current[toggleMaskKey].wasPressedThisFrame)
        {
            isMaskOn = !isMaskOn;

            if (isMaskOn)
                GameEvents.InvokeMaskEquipped();
            else
                GameEvents.InvokeMaskOff();

            SetMannequinMovement(isMaskOn);
        }

        mAccumulator += Time.deltaTime;
        while (mAccumulator >= 1f)
        {
            mAccumulator -= 1f;
            if (isMaskOn)
            {
                mCurrentMaskTime = Mathf.Min(maxMaskOffTime, mCurrentMaskTime + 1f);
                if (mCurrentMaskTime > 0f)
                    mHasTriggeredGameOver = false;
            }
            else
            {
                mCurrentMaskTime = Mathf.Max(0f, mCurrentMaskTime - 1f);
                if (mCurrentMaskTime <= 0f && !mHasTriggeredGameOver)
                {
                    mHasTriggeredGameOver = true;
                    GameEvents.InvokeGameLost();
                }
            }
        }
    }

    void SetMannequinMovement(bool canMove)
    {
        if (mannequins == null) return;

        for (int i = 0; i < mannequins.Length; i++)
        {
            if (mannequins[i] == null) continue;

            var ai = mannequins[i].GetComponent<IAstarAI>();
            if (ai != null)
                ai.canMove = canMove;
        }
    }
}
