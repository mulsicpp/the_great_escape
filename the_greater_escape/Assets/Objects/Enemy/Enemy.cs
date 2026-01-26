using UnityEditor;
using UnityEngine;

public enum EnemyState
{
    Patroling,
    Inspecting,
    Chasing
}

public class Enemy : Entity
{
    public MazeGrid maze_grid;

    public const float PATROLING_SPEED = 0.5f;
    public const float INSPECTING_SPEED = 1.0f;
    public const float CHASE_SPEED = 1.5f;

    public TransformInterpolation interpolation;

    public Vector3Int last_knwon_player_pos;
    public NavGrid nav_grid;
    public EnemyState state;
    public float speed;

    void OnEnable()
    {
        player_transform = new PlayerTransform
        {
            grid_position = Vector3Int.one * (maze_grid.dim - 1),
            forward = Vector3Int.back,
            up = Vector3Int.up
        };

        interpolation = null;

        transform.localPosition = player_transform.Position();
        transform.localRotation = player_transform.Rotation().rotation;

        last_knwon_player_pos = Vector3Int.zero;
        state = EnemyState.Patroling;
    }

    public static int Dot(Vector3Int v1, Vector3Int v2)
    {
        return v1.x * v2.x + v1.y * v2.y + v1.z * v2.z;
    }


    // Update is called once per frame
    void Update()
    {
        switch(state)
        {
            case EnemyState.Patroling:
                speed = PATROLING_SPEED; break;
            case EnemyState.Inspecting:
                speed = INSPECTING_SPEED; break;
            default: 
                speed = CHASE_SPEED; break;
        }
        if (interpolation == null)
        {
            nav_grid = maze_grid.NavigateTo(last_knwon_player_pos);

            var dir = nav_grid.GetDirection(player_transform.grid_position);
            switch (Dot(dir, player_transform.forward))
            {
                case 1:
                    interpolation = new MoveForwardInterpolation();
                    break;
                case -1:
                    interpolation = new RotateRightInterpolation();
                    break;
                default:
                    switch (Dot(dir, player_transform.up))
                    {
                        case 1:
                            interpolation = new RotateUpInterpolation();
                            break;
                        case -1:
                            interpolation = new RotateDownInterpolation();
                            break;
                        default:
                            if (Dot(dir, player_transform.calc_right()) == 1)
                            {
                                interpolation = new RotateRightInterpolation();
                            }
                            else
                            {
                                interpolation = new RotateLeftInterpolation();
                            }
                            break;
                    }
                    break;
            }
        }

        if (interpolation.Step(this, Time.deltaTime * speed))
        {
            interpolation = null;
        }
    }

    public void Alert(Vector3Int position)
    {
        last_knwon_player_pos = position;
        state = EnemyState.Inspecting;
    }
}
