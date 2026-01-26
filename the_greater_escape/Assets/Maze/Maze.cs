using System;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.Rendering.DebugUI.Table;
using Random = System.Random;

public class Maze
{
    //Nur zum Berechnen
    private HashSet<Vector3>[,,] cellSets;

    //Liste an Wänden, die Zwischen Zellen mit Koordinaten xyz Verlaufen Wand = Tuple(Zelle1, Zelle2);  Zelle = Triple(x,y,z)
    public List<Tuple<Vector3, Vector3, int>> wallList;
    public Vector3 exit;
    public int dim;


    public Maze(int dim, int seed)
    {
        cells(dim);
        walls(dim);
        Random rand = new Random(seed);
        RandomizeList(rand);
        kruskal();
        outerwalls(dim, rand);

        this.dim = dim;
    }



    private void cells(int dim)
    {
        cellSets = new HashSet<Vector3>[dim, dim, dim];


        for (int row = 0; row < dim; row++)
        {
            for (int col = 0; col < dim; col++)
            {
                for (int d = 0; d < dim; d++)
                {
                    cellSets[row, col, d] = new HashSet<Vector3>();
                    cellSets[row, col, d].Add(new Vector3(row, col, d));
                }
            }
        }


    }

    private void walls(int dim)
    {
        wallList = new List<Tuple<Vector3, Vector3, int>>();


        for (int row = 0; row < dim; row++)
        {
            for (int col = 0; col < dim; col++)
            {
                for (int d = 0; d < dim; d++)
                {


                    if (d != dim - 1) //right wall
                    {
                        wallList.Add(new Tuple<Vector3, Vector3, int>(new Vector3(row, col, d), new Vector3(row, col, d + 1), 1));
                    }

                    if (col != dim - 1) //front wall
                    {
                        wallList.Add(new Tuple<Vector3, Vector3, int>(new Vector3(row, col, d), new Vector3(row, col + 1, d), 1));
                    }

                    if (row != dim - 1) //down wall
                    {
                        wallList.Add(new Tuple<Vector3, Vector3, int>(new Vector3(row, col, d), new Vector3(row + 1, col, d),1 ));
                    }


                }
            }
        }


    }

    private void RandomizeList(Random seed)
    {
        int k;
        for (int i = 0; i < wallList.Count; i++)
        {
            k = seed.Next(wallList.Count - i);
            (wallList[i], wallList[k]) = (wallList[k], wallList[i]);
        }
    }

    private void kruskal()
    {
        for (var index = wallList.Count - 1; index >= 0; index--)
        {
            Tuple<Vector3, Vector3, int> t = wallList[index];
            Vector3 t1 = t.Item1;
            Vector3 t2 = t.Item2;
            if (!cellSets[(int)t1.x, (int)t1.y, (int)t1.z].Equals(cellSets[(int)t2.x, (int)t2.y, (int)t2.z]))
            {
                cellSets[(int)t1.x, (int)t1.y, (int)t1.z].UnionWith(cellSets[(int)t2.x, (int)t2.y, (int)t2.z]);

                foreach (Vector3 v in cellSets[(int)t1.x, (int)t1.y, (int)t1.z])
                {
                    if (v != t1)
                    {
                        cellSets[(int)v.x, (int)v.y, (int)v.z] = cellSets[(int)t1.x, (int)t1.y, (int)t1.z];
                    }
                }

                wallList[index] = new Tuple<Vector3, Vector3, int>(t1, t2, 0);
                //wallList.Remove(t);

            }
        }
    }

    private void outerwalls(int dim, Random seed)
    {

        List<Tuple<Vector3, Vector3, int>> outerwallList = new List<Tuple<Vector3, Vector3, int>>();

        for (int row = 0; row < dim; row++)
        {
            for (int col = 0; col < dim; col++)
            {
                outerwallList.Add(new Tuple<Vector3, Vector3, int>(new Vector3(0, row, col), new Vector3(-1, row, col), 2));
                outerwallList.Add(new Tuple<Vector3, Vector3, int>(new Vector3(dim - 1, row, col), new Vector3(dim, row, col), 2));
                outerwallList.Add(new Tuple<Vector3, Vector3, int>(new Vector3(row, 0, col), new Vector3(row, -1, col), 2));
                outerwallList.Add(new Tuple<Vector3, Vector3, int>(new Vector3(row, dim - 1, col), new Vector3(row, dim, col), 2));
                outerwallList.Add(new Tuple<Vector3, Vector3, int>(new Vector3(row, col, 0), new Vector3(row, col, -1), 2));
                outerwallList.Add(new Tuple<Vector3, Vector3, int>(new Vector3(row, col, dim - 1), new Vector3(row, col, dim), 2));
            }
        }
        

        Tuple<Vector3, Vector3, int> exitWall = new Tuple<Vector3, Vector3, int>(new Vector3(dim - 1, dim - 1, dim - 1), new Vector3(dim - 1, dim - 1, dim), 2);
        exit = exitWall.Item1 + (exitWall.Item2 - exitWall.Item1);
        outerwallList.Remove(exitWall);
        wallList.AddRange(outerwallList);

    }
}
