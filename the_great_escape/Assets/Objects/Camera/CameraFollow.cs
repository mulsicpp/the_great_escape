using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Player target;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        var target_pos = new Vector2(target.transform.position.x, target.transform.position.y) + target.rb.linearVelocity * 0.3f;

        var delta_x = target_pos.x - transform.position.x;
        var delta_y = target_pos.y - transform.position.y;

        var delta = new Vector3(delta_x, delta_y, 0.0f);

        transform.position = transform.position + delta * Time.fixedDeltaTime * 3.0f;
    }
}
