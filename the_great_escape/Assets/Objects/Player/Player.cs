using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class Player : MonoBehaviour
{
    public Rigidbody2D rb;
    public SpriteRenderer sprite;
    public GameObject projectile;
    public uint ammo;
    public uint health;

    const float MOVE_VELOCITY = 5.0f;
    const uint AMMO_PER_UNIT = 3;
    const uint MAX_HEALTH = 3;

    private Vector2 target_velocity;
    private Vector2 shoot_target;
    public float invisible_frames_time;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        sprite = rb.GetComponent<SpriteRenderer>();
        target_velocity = Vector2.zero;
        ammo = 0;
        health = MAX_HEALTH;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        var target_velocity = this.target_velocity * (invisible_frames_time <= 0.0f ? 1.0f : 1.4f);
        var diff = target_velocity - rb.linearVelocity;

        var diff_norm = diff.normalized;
        var diff_mag = diff.magnitude;

        var jump_distance = (diff_mag * 6.0f + 20.0f) * Time.fixedDeltaTime;

        if (jump_distance < diff_mag)
            rb.linearVelocity = rb.linearVelocity + jump_distance * diff_norm;
        else
            rb.linearVelocity = target_velocity;

        invisible_frames_time = Mathf.Max(invisible_frames_time - Time.fixedDeltaTime, 0.0f);

        var color = sprite.color;
        color.a = Mathf.Cos(Mathf.Log(invisible_frames_time + 1.0f, 2) * 30.0f) * 0.5f + 0.5f;
        sprite.color = color;
    }

    public void OnMove(InputAction.CallbackContext context)
    {
        target_velocity = context.ReadValue<Vector2>() * MOVE_VELOCITY;
    }

    public void OnShoot(InputAction.CallbackContext context)
    {
        // Debug.Log(context.started);
        if (context.started && ammo > 0)
        {
            Debug.Log("shoot");

            Vector2 player_pos = new Vector2(transform.position.x, transform.position.y);

            Vector2 direction = shoot_target - player_pos;

            Debug.Log(direction);

            Vector2 dir_normalized = direction.normalized;

            var spawned_proj = Instantiate(projectile, transform.position + new Vector3(dir_normalized.x, dir_normalized.y), Quaternion.identity);
            ammo--;
            spawned_proj.transform.rotation = Quaternion.FromToRotation(-Vector3.up, new Vector3(dir_normalized.x, dir_normalized.y, 0.0f));

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
            ammo += AMMO_PER_UNIT;
        }
    }

    public void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.layer == 6 /*Enemy*/)
        {
            Destroy(collision.gameObject);

            if(health > 0 && invisible_frames_time <= 0.0f)
            {
                health--;
                invisible_frames_time = 2.0f;
            }
            else
            {
                SceneManager.LoadScene("DeathScreen");
                return;
            }

            invisible_frames_time = 2.0f;
        }
    }
}
