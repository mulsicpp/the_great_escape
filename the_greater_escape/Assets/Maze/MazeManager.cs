using System.Collections.Generic;
using UnityEngine;
using System;
using Unity.VisualScripting;
using Object = UnityEngine.Object;

public class MazeManager : MonoBehaviour
{
    public Maze maze;
    public MazeGrid grid;

    [SerializeField] public GameObject wallsParent;
    [SerializeField] public int dim;
    [SerializeField] public int seed;
    [SerializeField] public Material wallMaterial;
    [SerializeField] public Material wallMaterial2;
    [SerializeField] public Material wallMaterial3;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        maze = new Maze(dim, seed);
        grid = new MazeGrid(maze);
        LoadMaze(maze, dim);
    }

    // Update is called once per frame
    void Update()
    {
        
    }


    public void LoadMaze(Maze maze, int dim)
    {
        foreach (var cells in maze.wallList)
        {
            var wall = GameObject.CreatePrimitive(PrimitiveType.Cube);
            
            Vector3 pos = (cells.Item1 + cells.Item2) / 2;
            wall.transform.position = pos;
            Vector3 scale = (cells.Item2 - cells.Item1);

            var baseScale = 1.0f;
            if(scale.x < 0.5)
            {
                baseScale = 1.001f;
            } else if (scale.z < 0.5)
            {
                baseScale = 0.999f;
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
            wall.transform.parent = wallsParent.transform;
        }
    }
}
