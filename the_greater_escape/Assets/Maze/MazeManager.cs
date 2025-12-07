using System.Collections.Generic;
using UnityEngine;
using System;
using Unity.VisualScripting;
using Object = UnityEngine.Object;

public class MazeManager : MonoBehaviour
{
    public Maze maze;
    public MazeGrid grid;

    [SerializeField] public int dim;
    [SerializeField] public int seed;
    [SerializeField] public Material wallMaterial;
    [SerializeField] public Material wallMaterial2;
    [SerializeField] public Material wallMaterial3;

    public GameObject exitLight;

    private GameObject wallParent;

    private void OnEnable()
    {
        Destroy(wallParent);
        Spawn();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void Spawn()
    {
        maze = new Maze(dim, seed);
        grid = new MazeGrid(maze);
        LoadMaze(maze, dim);

        var player = GetComponentInChildren<Player>();
        player.maze_grid = grid;
        player.graffiti_count = (dim * dim * dim) / 10 + 1;
        player.wallParent = wallParent;
    }


    public void LoadMaze(Maze maze, int dim)
    {
        wallParent = Instantiate(new GameObject("Walls"), transform);
        foreach (var cells in maze.wallList)
        {
            var wall = GameObject.CreatePrimitive(PrimitiveType.Cube);
            
            Vector3 pos = (cells.Item1 + cells.Item2) / 2;
            wall.transform.position = pos;
            Vector3 scale = (cells.Item2 - cells.Item1);

            var baseScale = 1.1f;
            if(Mathf.Abs(scale.x) > 0.5)
            {
                baseScale = 1.102f;
            } else if (Mathf.Abs(scale.z) > 0.5)
            {
                baseScale = 1.098f;
            }
            scale = Vector3.Scale(scale, scale);
            scale = new Vector3(baseScale, baseScale, baseScale) - scale;
            wall.transform.localScale = scale;
            if(wall.transform.localScale.y < 1f)
                wall.GetComponent<Renderer>().material = wallMaterial;
            else if (wall.transform.localScale.x < 1f)
                wall.GetComponent<Renderer>().material = wallMaterial2;
            else
                wall.GetComponent<Renderer>().material = wallMaterial3;
            wall.transform.parent = wallParent.transform;

        }
        exitLight.transform.position =  new Vector3(dim - 1, dim - 1, dim - 0.5f);
    }
}
