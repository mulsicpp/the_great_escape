using UnityEngine;

public class Enemy : Entity
{
    public MazeGrid maze_grid;

    void OnEnable()
    {
        player_transform = new PlayerTransform
        {
            grid_position = Vector3Int.one * (maze_grid.dim - 1),
            forward = Vector3Int.back,
            up = Vector3Int.up
        };

        transform.localPosition = player_transform.Position();
        transform.localRotation = player_transform.Rotation().rotation;
    }


    // Update is called once per frame
    void Update()
    {

    }
}
