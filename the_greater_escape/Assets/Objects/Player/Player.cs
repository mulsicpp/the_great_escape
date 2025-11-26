using UnityEngine;
using UnityEngine.InputSystem;

public struct PlayerTransform
{
    public Vector3Int grid_position;
    public Vector3Int forward;
    public Vector3Int up;

    public Vector3Int calc_right()
    {
        return new Vector3Int(up.y * forward.z - up.z * forward.y, up.z * forward.x - up.x * forward.z, up.x * forward.y - up.y * forward.x);
    }

    public Vector3 Position() { return new Vector3(grid_position.x, grid_position.y, grid_position.z); }
    public Matrix4x4 Rotation()
    {
        var right = calc_right();

        var right_vec = new Vector4(right.x, right.y, right.z);
        var up_vec = new Vector4(up.x, up.y, up.z);
        var forward_vec = new Vector4(forward.x, forward.y, forward.z);

        return new Matrix4x4(right_vec, up_vec, forward_vec, new Vector4(0, 0, 0, 1));
    }
}

public class Player : MonoBehaviour
{
    public PlayerTransform player_transform;

    TransformInterpolation interpolation;
    TransformInterpolation buffered_interpolation;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        player_transform = new PlayerTransform
        {
            grid_position = Vector3Int.zero,
            forward = Vector3Int.forward,
            up = Vector3Int.up
        };

        interpolation = null;
    }

    // Update is called once per frame
    void Update()
    {
        if (interpolation != null)
        {
            if (interpolation.Step(this, Time.deltaTime * 3.0f))
            {
                interpolation = buffered_interpolation;
                buffered_interpolation = null;
            }
        }
    }

    void SetInterpolation(TransformInterpolation new_interpolation)
    {
        if (interpolation == null)
        {
            interpolation = new_interpolation;
        }
        else if (interpolation.Time() > 0.6f)
        {
            buffered_interpolation = new_interpolation;
        }
    }

    public void OnTurnLeft(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            SetInterpolation(new RotateLeftInterpolation());
        }
    }

    public void OnTurnRight(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            SetInterpolation(new RotateRightInterpolation());
        }
    }

    public void OnTurnDown(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            SetInterpolation(new RotateDownInterpolation());
        }
    }

    public void OnTurnUp(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            SetInterpolation(new RotateUpInterpolation());
        }
    }

    public void OnMoveForward(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            SetInterpolation(new MoveForwardInterpolation());
        }
    }
}
