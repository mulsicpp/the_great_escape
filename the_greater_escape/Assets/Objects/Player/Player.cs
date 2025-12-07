using UnityEditor.SearchService;
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

    public MazeGrid maze_grid;
    public GameObject wallParent;

    public GameObject graffiti_prefab;
    public int graffiti_count;

    public Texture[] graffiti_textures;
    public Color[] graffiti_colors;
    public AudioClip[] footsteps;
    public AudioSource audiosource;

    public Pause pause;

    private Shader urp;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void Start()
    {
        urp = Shader.Find("Universal Render Pipeline/Lit");

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
            // TODO win game
            Debug.Log("Game won!");
        }
    }

    void SetInterpolation(TransformInterpolation new_interpolation)
    {
        if (interpolation == null)
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
            int random_index = Random.Range(0, footsteps.Length);
            float maxVolume = audiosource.volume;
            float randomVolume = Random.Range(0.3f * maxVolume, maxVolume);
            var clip = footsteps[random_index];
            audiosource.PlayOneShot(clip, randomVolume);
            var target_position = player_transform.grid_position + player_transform.forward;
            if ((maze_grid.Contains(player_transform.grid_position) || maze_grid.Contains(target_position)) && maze_grid.Between(player_transform.grid_position, target_position))
            {
                SetInterpolation(new BumpWallInterpolation());
            }
            else
            {
                SetInterpolation(new MoveForwardInterpolation());
            }
        }
    }

    public void OnPaint(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            var mesh_renderer = graffiti_prefab.GetComponentInChildren<MeshRenderer>();
            var light = graffiti_prefab.GetComponentInChildren<Light>();

            Material material = new Material(urp);

            var texture = graffiti_textures[graffiti_count % graffiti_textures.Length];
            var color = graffiti_colors[graffiti_count % graffiti_colors.Length];

            material.SetFloat("_AlphaClip", 1.0f);
            material.SetFloat("_Cutoff", 0.5f);
            material.EnableKeyword("_ALPHATEST_ON");

            material.SetTexture("_BaseMap", texture);
            material.SetColor("_BaseColor", Color.black);

            material.SetFloat("_Metallic", 0.0f);

            material.EnableKeyword("_EMISSION");
            material.SetTexture("_EmissionMap", texture);
            material.SetColor("_EmissionColor", color);

            light.color = color;

            mesh_renderer.sharedMaterial = material;

            var target_position = player_transform.grid_position + player_transform.forward;
            if (graffiti_count > 0 && (maze_grid.Contains(player_transform.grid_position) || maze_grid.Contains(target_position)) && maze_grid.Between(player_transform.grid_position, target_position))
            {
                Instantiate(graffiti_prefab, player_transform.Position(), player_transform.Rotation().rotation, wallParent.transform);
                graffiti_count--;
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
