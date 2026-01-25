using System.IO;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public enum TelemetryEvent
{
    GAME_WON,
    LEFT,
    RIGHT,
    UP,
    DOWN,
    FORWARD,
    BUMP,
    STICKER
}

public class TelemetryEncoder
{
    const string TELEMETRY_PATH = "Telemetry";


    private readonly FileStream stream;
    private double start_time;


    public TelemetryEncoder(uint seed, uint size)
    {
        Debug.Log(System.IO.Directory.GetCurrentDirectory());

        var time = System.DateTime.Now;

        Directory.CreateDirectory(TELEMETRY_PATH);
        stream = File.Create(TELEMETRY_PATH + "/Game-" + ((ulong)time.ToBinary()).ToString());
        stream.Write(System.BitConverter.GetBytes(seed), 0, 4);
        stream.Write(System.BitConverter.GetBytes(size), 0, 4);
        stream.Flush();

        start_time = Time.realtimeSinceStartupAsDouble;
    }

    public void AddEvent(TelemetryEvent e)
    {
        stream.Write(System.BitConverter.GetBytes((uint)e), 0, 4);
        stream.Write(System.BitConverter.GetBytes((uint)((Time.realtimeSinceStartupAsDouble - start_time) * 1000)), 0, 4);
        stream.Flush();
    }
}

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

public class Player : Entity
{
    TransformInterpolation interpolation;

    public MazeGrid maze_grid;
    public GameObject wallParent;

    public Enemy enemy;

    public GameObject graffiti_prefab;
    public int graffiti_count;

    public int wall_build_count;
    public int material_count = 5;
    public int material_build_cost = 5;

    public Material[] base_materials;
    public Color[] graffiti_colors;
    public AudioClip[] footsteps;
    public AudioClip suprise;
    public AudioSource audiosource;

    public Pause pause;
    public Finish finish;

    public TelemetryEncoder telemetry;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void Start()
    {
        audiosource = GetComponent<AudioSource>();
    }

    void OnEnable()
    {
        player_transform = new PlayerTransform
        {
            grid_position = Vector3Int.zero,
            forward = Vector3Int.forward,
            up = Vector3Int.up
        };

        interpolation = null;

        transform.localPosition = player_transform.Position();
        transform.localRotation = player_transform.Rotation().rotation;
    }


    // Update is called once per frame
    void Update()
    {
        if (interpolation != null)
        {
            if (interpolation.Step(this, Time.deltaTime * 3.0f))
            {
                interpolation = null;
            }
        }

        if (!maze_grid.Contains(player_transform.grid_position))
        {
            telemetry.AddEvent(TelemetryEvent.GAME_WON);
            finish.Show();
        }

        if (player_transform.grid_position == enemy.player_transform.grid_position || (enemy.interpolation is MoveForwardInterpolation && player_transform.grid_position == enemy.player_transform.grid_position + enemy.player_transform.forward))
        {
            finish.Show();
        }
    }

    void SetInterpolation(TransformInterpolation new_interpolation)
    {
        if (interpolation == null && Time.timeScale > 0.5f)
        {
            interpolation = new_interpolation;
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
            int suprise_chance = Random.Range(0, 10);
            int random_index = Random.Range(0, footsteps.Length);
            float maxVolume = audiosource.volume;
            float randomVolume = Random.Range(0.3f * maxVolume, maxVolume);
            var clip = footsteps[random_index];

            var target_position = player_transform.grid_position + player_transform.forward;
            if ((maze_grid.Contains(player_transform.grid_position) || maze_grid.Contains(target_position)) && maze_grid.Between(player_transform.grid_position, target_position))
            {
                SetInterpolation(new BumpWallInterpolation());
                if (suprise_chance == 0)
                {
                    audiosource.PlayOneShot(suprise, randomVolume);
                }
            }
            else
            {
                audiosource.PlayOneShot(clip, randomVolume);
                SetInterpolation(new MoveForwardInterpolation());
            }
        }
    }

    public void OnPaint(InputAction.CallbackContext context)
    {
        if (context.started && Time.timeScale > 0.5)
        {
            var mesh_renderer = graffiti_prefab.GetComponentInChildren<MeshRenderer>();
            var light = graffiti_prefab.GetComponentInChildren<Light>();

            Material material = new Material(base_materials[graffiti_count % base_materials.Length]);
            var color = graffiti_colors[graffiti_count % graffiti_colors.Length];
            material.SetColor("_EmissionColor", color);

            light.color = color;

            mesh_renderer.sharedMaterial = material;

            var target_position = player_transform.grid_position + player_transform.forward;
            if (graffiti_count > 0 && (maze_grid.Contains(player_transform.grid_position) || maze_grid.Contains(target_position)) && maze_grid.Between(player_transform.grid_position, target_position))
            {
                Instantiate(graffiti_prefab, player_transform.Position(), player_transform.Rotation().rotation, wallParent.transform);
                graffiti_count--;
                telemetry.AddEvent(TelemetryEvent.STICKER);
            }
        }
    }

    public void BuildWall(InputAction.CallbackContext context)
    {
        if (context.started && Time.timeScale > 0.5)
        {
            var target_position = player_transform.grid_position + player_transform.forward;
            if (Physics.Raycast(player_transform.Position(), player_transform.forward, out RaycastHit hitInfo, 1f))
            {
                if (!hitInfo.collider.GetComponent<MeshRenderer>().enabled)
                {
                    hitInfo.collider.GetComponent<MeshRenderer>().enabled = true;
                    maze_grid.BuildWall(player_transform.grid_position, target_position);

                    material_count++;
                    if (material_count % material_build_cost == 0)
                    {
                        wall_build_count++;
                    }
                }

            }
        }

    }

    public void DestroyWall(InputAction.CallbackContext context)
    {
        if (context.started && Time.timeScale > 0.5)
        {
            var target_position = player_transform.grid_position + player_transform.forward;
            if (Physics.Raycast(player_transform.Position(), player_transform.forward, out RaycastHit hitInfo, 1f))
            {
                if (hitInfo.collider.GetComponent<MeshRenderer>().enabled && hitInfo.collider.gameObject.tag == "Inner Wall")
                {
                    hitInfo.collider.GetComponent<MeshRenderer>().enabled = false;
                    maze_grid.DestroyWall(player_transform.grid_position, target_position);
                    wall_build_count--;
                }

            }
        }

    }


    public void OnEsc(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            pause.Show();
        }
    }
}
