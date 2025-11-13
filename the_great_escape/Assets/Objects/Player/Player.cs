using UnityEngine;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour
{
    public Rigidbody2D rb;
    public GameObject projectile;
    public uint coins;
    const float MOVE_VELOCITY = 5.0f;

    private Vector2 target_velocity;
    private Vector2 shoot_target;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        target_velocity = Vector2.zero;
        coins = 0;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        var diff = target_velocity - rb.linearVelocity;

        var diff_norm = diff.normalized;
        var diff_mag = diff.magnitude;

        var jump_distance = (diff_mag * 6.0f + 20.0f) * Time.fixedDeltaTime;

        if (jump_distance < diff_mag)
            rb.linearVelocity = rb.linearVelocity + jump_distance * diff_norm;
        else
            rb.linearVelocity = target_velocity;
    }

    public void OnMove(InputAction.CallbackContext context)
    {
        target_velocity = context.ReadValue<Vector2>() * MOVE_VELOCITY;
    }

    public void OnShoot(InputAction.CallbackContext context)
    {
        // Debug.Log(context.started);
        if (context.started)
        {
            Debug.Log("shoot");

            Vector2 player_pos = new Vector2(transform.position.x, transform.position.y);

            Vector2 direction = shoot_target - player_pos;

            Debug.Log(direction);

            Vector2 dir_normalized = direction.normalized;

            var spawned_proj = Instantiate(projectile, transform.position + new Vector3(dir_normalized.x, dir_normalized.y), Quaternion.identity);

            spawned_proj.GetComponent<Rigidbody2D>().linearVelocity = dir_normalized * 15.0f;
        }
    }

    public void OnMouseMove(InputAction.CallbackContext context)
    {
        Vector2 mouse_pos = context.ReadValue<Vector2>();

        Vector3 shoot_target_3d = Camera.main.ScreenToWorldPoint(new Vector3(mouse_pos.x, mouse_pos.y, -Camera.main.transform.position.z));
        shoot_target = new Vector2(shoot_target_3d.x, shoot_target_3d.y);
    }

    public void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.layer == 7 /*Collectable*/)
        {
            Destroy(collision.gameObject);
            coins++;
        }
    }
}
