using System.Collections.Generic;
using UnityEngine;
using System;
using Unity.VisualScripting;
using Object = UnityEngine.Object;

public class MazeManager : MonoBehaviour
{
    public Maze maze;

    [SerializeField] public GameObject wallsParent;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        maze = new Maze(5, 1);
        LoadMaze(maze, 5);
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
            scale = Vector3.Scale(scale, scale);
            scale = (new Vector3(1, 1, 1) - scale) + (scale * 0.1f);
            wall.transform.localScale = scale;
            wall.transform.parent = wallsParent.transform;
        }

        
        
    }
}
