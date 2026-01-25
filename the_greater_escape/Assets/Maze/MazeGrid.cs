using System;
using System.Collections.Generic;
using System.ComponentModel;
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
            if (wall.Item3 == 1)
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

    public NavGrid NavigateTo(Vector3Int position)
    {
        if(!Contains(position)) return null;

        NavGrid nav_grid = new NavGrid(dim);

        (Vector3Int, NavTag)[] search_directions = { 
            (Vector3Int.right, NavTag.NegX),
            (Vector3Int.left, NavTag.PosX),
            (Vector3Int.up, NavTag.NegY),
            (Vector3Int.down, NavTag.PosY),
            (Vector3Int.forward, NavTag.NegZ),
            (Vector3Int.back, NavTag.PosZ)
        };

        List<Vector3Int> queue = new List<Vector3Int>();

        nav_grid[position] = NavTag.Arrived;
        queue.Add(position);

        while(queue.Count > 0)
        {
            var next_queue = new List<Vector3Int>();
            foreach(var pos in queue)
            {
                foreach(var (dir, tag) in search_directions)
                {
                    var search_pos = pos + dir;

                    if (!Contains(search_pos) || Between(pos, search_pos)) continue;

                    if (nav_grid[search_pos] == NavTag.None)
                    {
                        nav_grid[search_pos] = tag;
                        next_queue.Add(search_pos);
                    }
                }
            }
            queue = next_queue;
        }

        return nav_grid;
    }

    public List<Vector3Int> OptimalPath(Vector3Int from, Vector3Int to)
    {
        var nav_grid = NavigateTo(to);

        List<Vector3Int> positions = new();

        positions.Add(from);

        Vector3Int pos = from;

        while(pos != to)
        {
            pos += nav_grid.GetDirection(pos);
            positions.Add(pos);
        }

        return positions;
    }
}

public enum NavTag : byte
{
    None,
    PosX,
    NegX,
    PosY,
    NegY,
    PosZ,
    NegZ,
    Arrived
}


public class NavGrid
{
    private readonly Vector3Int[] nav_directions = { 
        Vector3Int.zero,
        Vector3Int.right,
        Vector3Int.left,
        Vector3Int.up,
        Vector3Int.down,
        Vector3Int.forward,
        Vector3Int.back,
        Vector3Int.zero
    };

    public NavTag[,,] nav_grid;
    int dim;

    public NavGrid(int dim)
    {
        this.dim = dim;
        nav_grid = new NavTag[dim, dim, dim];

        for (int i = 0; i < dim; i++)
            for (int j = 0; j < dim; j++)
                for (int k = 0; k < dim; k++)
                    nav_grid[i, j, k] = NavTag.None;
    }

    public void Log()
    {
        for (int i = 0; i < dim; i++)
            for (int j = 0; j < dim; j++)
                for (int k = 0; k < dim; k++)
                    Debug.Log(nav_grid[i, j, k]);
    }

    public NavTag GetTag(Vector3Int pos)
    {
        return nav_grid[pos.x, pos.y, pos.z];
    }

    public void SetTag(Vector3Int pos, NavTag tag)
    {
        nav_grid[pos.x, pos.y, pos.z] = tag;
    }

    public NavTag this[Vector3Int pos]
    {
        get => GetTag(pos);
        set => SetTag(pos, value);
    }

    public Vector3Int GetDirection(Vector3Int pos)
    {
        return nav_directions[(int)nav_grid[pos.x, pos.y, pos.z]];
    }
}

public class WeightedGrid
{
    float[,,,] weight_grid;
    int dim;

    public WeightedGrid(int dim)
    {
        this.dim = dim;
        weight_grid = new float[dim, dim, dim, 6];
    }
}