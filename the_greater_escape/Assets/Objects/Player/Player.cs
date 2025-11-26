using UnityEngine;
using UnityEngine.InputSystem;

struct PlayerTransform
{
    public Vector3Int grid_position;
    public Vector3Int forward;
    public Vector3Int up;

    public Vector3Int calc_right()
    {
        return new Vector3Int(up.y * forward.z - up.z * forward.y, up.z * forward.x - up.x * forward.z, up.x * forward.y - up.y * forward.x);
    }
}

public class Player : MonoBehaviour
{
    private PlayerTransform player_transform;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        player_transform = new PlayerTransform
        {
            grid_position = Vector3Int.zero,
            forward = Vector3Int.up,
            up = Vector3Int.forward
        };

        apply_player_transform(player_transform);
    }

    // Update is called once per frame
    void Update()
    {
        apply_player_transform(player_transform);
    }

    void apply_player_transform(PlayerTransform player_transform)
    {
        var right = player_transform.calc_right();

        var right_vec = new Vector4(right.x, right.y, right.z);
        var up_vec = new Vector4(player_transform.up.x, player_transform.up.y, player_transform.up.z);
        var forward_vec = new Vector4(player_transform.forward.x, player_transform.forward.y, player_transform.forward.z);

        var rotation_mat = new Matrix4x4(right_vec, up_vec, forward_vec, new Vector4(0, 0, 0, 1));

        transform.localRotation = rotation_mat.rotation;
        transform.localPosition = new Vector3(player_transform.grid_position.x, player_transform.grid_position.y, player_transform.grid_position.z);
    }

    public void OnTurnLeft(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            player_transform.forward = -player_transform.calc_right();
        }
    }

    public void OnTurnRight(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            player_transform.forward = player_transform.calc_right();
        }
    }

    public void OnTurnDown(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            var down = -player_transform.up;
            player_transform.up = player_transform.forward;
            player_transform.forward = down;
        }
    }

    public void OnTurnUp(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            var backward = -player_transform.forward;
            player_transform.forward = player_transform.up;
            player_transform.up = backward;
        }
    }

    public void OnMoveForward(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            player_transform.grid_position += player_transform.forward;
        }
    }
}
