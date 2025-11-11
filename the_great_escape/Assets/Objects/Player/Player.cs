using UnityEngine;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour
{
    public Rigidbody2D rb;
    const float MOVE_VELOCITY = 5.0f;

    private Vector2 target_velocity;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        target_velocity = Vector2.zero;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        var diff = target_velocity - rb.linearVelocity;

        var diff_norm = diff.normalized;
        var diff_mag = diff.magnitude;

        var jump_distance = (diff_mag * 3.0f + 15.0f) * Time.fixedDeltaTime;

        if (jump_distance < diff_mag)
            rb.linearVelocity = rb.linearVelocity + jump_distance * diff_norm;
        else
            rb.linearVelocity = target_velocity;
    }

    public void OnMove(InputAction.CallbackContext context)
    {
        target_velocity = context.ReadValue<Vector2>() * MOVE_VELOCITY;
    }
}
