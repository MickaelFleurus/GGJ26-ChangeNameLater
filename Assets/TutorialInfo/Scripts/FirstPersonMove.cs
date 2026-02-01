using UnityEngine;
using System;

[RequireComponent(typeof(CharacterController))]
public class FirstPersonMove : MonoBehaviour
{
    public float moveSpeed = 4.0f;
    public float gravity = -9.81f;

    private CharacterController controller;
    private Vector3 velocity;
    private bool mGamePause = false;
    private Action<bool> mGamePausedFunc;

    void Awake()
    {
        mGamePausedFunc = (bool paused) =>
        {
            mGamePause = paused;
        };
        GameEvents.OnGamePausedChanged += mGamePausedFunc;
        controller = GetComponent<CharacterController>();
    }

    void Destroy()
    {
        GameEvents.OnGamePausedChanged -= mGamePausedFunc;
    }

    void Update()
    {
        if (mGamePause) return;
        float x = Input.GetAxis("Horizontal"); // A/D
        float z = Input.GetAxis("Vertical");   // W/S

        Vector3 move = transform.right * x + transform.forward * z;
        controller.Move(move * moveSpeed * Time.deltaTime);

        // Simple gravity
        if (controller.isGrounded && velocity.y < 0)
            velocity.y = -2f;

        velocity.y += gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);
    }
}
