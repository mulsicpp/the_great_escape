using System;
using UnityEngine;

public class MazeGrid
{
    private bool[,,] x_walls;
    private bool[,,] y_walls;
    private bool[,,] z_walls;

    public readonly int dim;

    public MazeGrid(Maze maze)
    {
        x_walls = new bool[maze.dim + 1, maze.dim, maze.dim];
        y_walls = new bool[maze.dim, maze.dim + 1, maze.dim];
        z_walls = new bool[maze.dim, maze.dim, maze.dim + 1];

        dim = maze.dim;

        foreach (var wall in maze.wallList)
        {
            var cell1 = Vector3Int.RoundToInt(wall.Item1);
            var cell2 = Vector3Int.RoundToInt(wall.Item2);

            //Between(cell1, cell2) = true;
            if(wall.Item3 == 1)
            {
                ref var wall_ref = ref Between(cell1, cell2);
                wall_ref = true;
            }
        }
    }

    public ref bool Between(Vector3Int cell1, Vector3Int cell2)
    {
        var diff = cell2 - cell1;

        var diff_tup = (diff.x, diff.y, diff.z);

        var x = cell1.x;
        var y = cell1.y;
        var z = cell1.z;

        switch (diff_tup)
        {
            case (1, 0, 0):
                return ref x_walls[x + 1, y, z];
            case (-1, 0, 0):
                return ref x_walls[x, y, z];
            case (0, 1, 0):
                return ref y_walls[x, y + 1, z];
            case (0, -1, 0):
                return ref y_walls[x, y, z];
            case (0, 0, 1):
                return ref z_walls[x, y, z + 1];
            case (0, 0, -1):
                return ref z_walls[x, y, z];
        }

        throw new ArgumentException("Specified cells are not neighbors");
    }

    public void BuildWall(Vector3Int cell1, Vector3Int cell2)
    {
        ref var wall_ref = ref Between(cell1, cell2);
        wall_ref = true;
    }
    public void DestroyWall(Vector3Int cell1, Vector3Int cell2)
    {
        ref var wall_ref = ref Between(cell1, cell2);
        wall_ref = false;
    }

    public bool Contains(Vector3Int position)
    {
        return position.x >= 0 && position.x < dim && position.y >= 0 && position.y < dim && position.z >= 0 && position.z < dim;
    }
}