using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class MapManager: MonoBehaviour
{ 
    private static readonly int[][] _MAP1 =  {
                new int[]  {1,1,1,1,1, 1,1,1,1,1 ,1,1,1,1,1 ,1,1,1,1,1 ,1,1,1,1,1},
                new int[]  {1,0,0,0,0, 0,0,0,0,0 ,0,0,0,0,0 ,0,0,0,0,0, 0,0,0,0,1},
                new int[]  {1,0,0,0,0, 0,0,0,0,0 ,0,0,0,0,0 ,0,0,0,0,0, 0,0,0,0,1},
                new int[]  {1,0,0,0,0, 0,0,0,0,0 ,0,0,0,0,0 ,0,0,0,0,0, 0,0,0,0,1},
                new int[]  {1,0,0,0,0, 1,1,1,1,1 ,1,1,1,1,1 ,1,1,1,1,1 ,0,0,0,0,1},

                new int[]  {1,0,0,0,0, 0,0,1,0,0 ,0,0,0,0,1 ,0,0,0,0,0 ,0,0,0,0,1},
                new int[]  {1,0,0,0,0, 0,0,0,0,0 ,0,0,0,0,1 ,0,0,0,0,0 ,0,0,0,0,1},
                new int[]  {1,0,0,0,0, 0,0,0,0,0 ,0,0,0,0,1 ,0,0,0,0,1 ,1,1,1,1,1},
                new int[]  {1,0,0,0,1, 0,0,0,0,0 ,0,0,0,0,1 ,1,0,0,0,1, 0,0,0,0,1},
                new int[]  {1,0,0,0,1, 0,0,0,0,0 ,0,0,0,0,0 ,1,0,0,0,1, 0,0,0,0,1},

                new int[]  {1,0,0,0,1, 1,1,0,0,0 ,1,0,0,0,0 ,0,0,0,0,1, 0,0,0,0,1},
                new int[]  {1,0,0,0,0, 0,1,0,0,0 ,1,0,0,0,0 ,0,0,0,0,1, 0,0,0,0,1},
                new int[]  {1,0,0,0,0, 0,1,0,0,0 ,1,0,0,0,0 ,0,0,0,0,1, 0,0,0,0,1},
                new int[]  {1,0,0,0,0, 0,1,0,0,0 ,1,0,0,0,1 ,1,0,0,0,1, 0,0,0,0,1},
                new int[]  {1,0,0,0,0, 0,1,0,0,0 ,1,0,0,0,0 ,0,0,0,0,1, 1,0,0,0,1},

                new int[]  {1,0,0,0,0, 0,1,1,1,1 ,1,0,0,0,0 ,0,0,0,0,0, 1,0,0,0,1},
                new int[]  {1,0,0,0,0, 0,0,0,0,0 ,0,0,0,0,0 ,0,0,0,0,0, 0,0,0,0,1},
                new int[]  {1,0,0,0,0, 0,0,0,0,0 ,0,0,0,0,0 ,0,0,0,0,0, 0,0,0,0,1},
                new int[]  {1,0,0,0,0, 0,0,0,0,0 ,0,0,0,0,0 ,0,0,0,0,0, 0,0,0,0,1},
                new int[]  {1,0,0,0,0, 0,0,0,0,0 ,0,0,0,0,0 ,0,0,0,0,0 ,0,0,0,0,1},

                new int[]  {1,0,0,0,1, 1,1,1,1,1 ,1,1,1,1,1 ,1,1,1,1,1 ,0,0,0,0,1},
                new int[]  {1,0,0,0,0, 0,0,0,0,0 ,0,0,0,0,0 ,0,0,0,0,0, 0,0,0,0,1},
                new int[]  {1,0,0,0,0, 0,0,0,0,0 ,0,0,0,0,0 ,0,0,0,0,0, 0,0,0,0,1},
                new int[]  {1,0,0,0,0, 0,0,0,0,0 ,0,0,0,0,0 ,0,0,0,0,0, 0,0,0,0,1},
                new int[]  {1,0,0,0,0, 0,0,0,0,0 ,0,0,0,0,0 ,0,0,0,0,0 ,0,0,0,0,1},

                new int[]  {1,0,0,0,0, 0,0,0,0,0 ,0,1,1,0,0 ,0,0,0,0,0, 0,0,0,0,1},
                new int[]  {1,0,0,0,0, 0,0,0,0,0 ,0,1,1,0,0 ,0,0,0,0,0, 0,0,0,0,1},
                new int[]  {1,0,0,0,0, 0,0,0,0,0 ,0,0,0,0,0 ,0,0,0,0,0, 0,0,0,0,1},
                new int[]  {1,0,0,0,0, 0,0,0,0,0 ,0,0,0,0,0 ,0,0,0,0,0, 0,0,0,0,1},
                new int[]  {1,0,0,0,0, 0,0,0,0,0 ,0,0,0,0,0 ,0,0,0,0,0, 0,0,0,0,1},

                new int[]  {1,0,0,0,0, 0,0,0,0,0 ,0,0,0,0,0 ,0,0,0,0,0, 0,0,0,0,1},
                new int[]  {1,0,0,0,0, 0,0,0,0,0 ,0,0,0,0,0 ,0,0,0,0,0, 0,0,0,0,1},
                new int[]  {1,0,0,0,0, 0,0,0,0,0 ,0,0,0,0,0 ,0,0,0,0,0, 0,0,0,0,1},
                new int[]  {1,0,0,0,0, 0,0,0,0,0 ,0,0,0,0,0 ,0,0,0,0,0, 0,0,0,0,1},
                new int[]  {1,1,1,1,1, 1,1,1,1,1 ,1,0,0,0,1 ,1,1,1,1,1 ,1,1,1,1,1}};

    public int[][] Map1 { get { return _MAP1; } }
    public int Map1Cells { get { return _MAP1.Length * _MAP1[0].Length; } }

    static readonly int[][] _MAP2 = { new int[] {1,1,1,1,1, 1,1,1,1,1 ,1,1,1,1,1 ,1,1,1,1,1 ,1,1,1,1,1},
                new int[]  {1,0,0,0,0, 0,0,0,0,0 ,0,0,0,0,0 ,0,0,0,0,0, 0,0,0,0,1},
                new int[]  {1,0,0,0,0, 0,0,0,0,0 ,0,0,0,0,0 ,0,0,0,0,0, 0,0,0,0,1},
                new int[]  {1,0,0,0,0, 0,0,0,0,0 ,0,0,0,0,0 ,0,0,0,0,0, 0,0,0,0,1},
                new int[]  {1,0,0,0,1, 1,1,1,1,0 ,0,0,1,1,1 ,1,1,1,1,0, 0,0,0,0,1},

                new int[]  {1,0,0,0,0, 0,0,0,1,0 ,0,0,1,0,0 ,0,0,0,1,0 ,0,0,0,0,1},
                new int[]  {1,0,0,0,0, 0,0,0,1,0 ,0,0,1,0,0 ,0,0,0,1,0 ,0,0,0,0,1},
                new int[]  {1,0,0,0,0, 0,0,0,1,0 ,0,0,1,1,0 ,0,0,0,1,0 ,0,0,0,0,1},
                new int[]  {1,1,1,1,1, 1,1,1,1,0 ,0,0,0,0,0 ,0,0,0,1,0, 0,0,0,0,1},
                new int[]  {1,0,0,0,0, 0,0,0,0,0 ,0,0,0,0,0 ,0,0,0,1,0, 0,0,0,0,1},

                new int[]  {1,0,0,0,0, 0,0,0,0,0 ,0,0,1,0,0 ,0,0,0,1,0, 0,0,0,0,1},
                new int[]  {1,0,0,0,0, 0,0,0,0,0 ,0,0,1,0,0 ,0,0,0,1,0, 0,0,0,0,1},
                new int[]  {1,0,0,0,1, 1,1,1,1,0 ,0,0,1,1,1 ,0,0,0,1,0, 0,0,0,0,1},
                new int[]  {1,0,0,0,1, 0,0,0,1,0 ,0,0,0,0,1 ,0,0,0,1,0, 0,0,0,0,1},
                new int[]  {1,1,1,1,1, 0,0,0,1,0 ,0,0,0,0,1 ,0,0,0,1,0, 0,0,0,0,1},

                new int[]  {1,0,0,0,0, 0,0,0,1,1 ,1,1,1,1,1 ,0,0,0,1,0, 0,0,0,0,1},
                new int[]  {1,0,0,0,0, 0,0,0,0,0 ,0,0,0,0,0 ,0,0,0,1,0, 0,0,0,0,1},
                new int[]  {1,0,0,0,0, 0,0,0,0,0 ,0,0,0,0,0 ,0,0,0,1,0, 0,0,0,0,1},
                new int[]  {1,0,0,0,0, 0,0,0,0,0 ,0,0,0,0,0 ,0,0,0,1,0, 0,0,0,0,1},
                new int[]  {1,0,0,0,0, 1,1,0,0,0 ,1,1,1,1,1 ,1,1,1,1,1 ,1,0,0,0,1},

                new int[]  {1,0,0,0,0, 0,0,0,0,0 ,0,0,0,0,0 ,0,0,0,0,0, 1,0,0,0,1},
                new int[]  {1,0,0,0,0, 0,0,0,0,0 ,0,0,0,0,0 ,0,0,0,0,0, 1,0,0,0,1},
                new int[]  {1,0,0,0,0, 0,0,0,0,0 ,0,0,0,0,0 ,0,0,0,0,0, 1,0,0,0,1},
                new int[]  {1,0,0,0,0, 0,0,0,0,0 ,0,0,0,0,0 ,0,0,0,0,0, 1,0,0,0,1},
                new int[]  {1,0,0,0,1, 1,1,1,1,1 ,1,0,0,0,1 ,1,1,1,1,1 ,1,1,1,1,1},

                new int[]  {1,0,0,0,0, 0,0,0,0,0 ,0,0,0,0,0 ,0,0,0,0,0, 0,0,0,0,1},
                new int[]  {1,0,0,0,0, 0,0,0,0,0 ,0,0,0,0,0 ,0,0,0,0,0, 0,0,0,0,1},
                new int[]  {1,0,0,0,0, 0,0,0,0,0 ,0,0,0,0,0 ,0,0,0,0,0, 0,0,0,0,1},
                new int[]  {1,0,0,0,0, 0,0,0,0,0 ,0,0,0,0,0 ,0,0,0,0,0, 0,0,0,0,1},
                new int[]  {1,0,0,0,0, 0,0,0,0,0 ,0,0,0,0,0 ,0,0,0,0,0, 0,0,0,0,1},

                new int[]  {1,0,0,0,0, 0,0,0,0,0 ,0,0,0,0,0 ,0,0,0,0,0, 0,0,0,0,1},
                new int[]  {1,0,0,0,0, 0,0,0,0,0 ,0,0,0,0,0 ,0,0,0,0,0, 0,0,0,0,1},
                new int[]  {1,0,0,0,0, 0,0,0,0,0 ,0,0,0,0,0 ,0,0,0,0,0, 0,0,0,0,1},
                new int[]  {1,0,0,0,0, 0,0,0,0,0 ,0,0,0,0,0 ,0,0,0,0,0, 0,0,0,0,1},
                new int[]  {1,0,0,0,0, 0,0,0,0,0 ,0,1,1,0,0 ,0,0,0,0,0, 0,0,0,0,1},

                new int[]  {1,0,0,0,0, 0,0,0,0,0 ,0,1,1,0,0 ,0,0,0,0,0, 0,0,0,0,1},
                new int[]  {1,0,0,0,0, 0,0,0,0,0 ,0,0,0,0,0 ,0,0,0,0,0, 0,0,0,0,1},
                new int[]  {1,0,0,0,0, 0,0,0,0,0 ,0,0,0,0,0 ,0,0,0,0,0, 0,0,0,0,1},
                new int[]  {1,0,0,0,0, 0,0,0,0,0 ,0,0,0,0,0 ,0,0,0,0,0, 0,0,0,0,1},
                new int[]  {1,1,1,1,1, 1,1,1,1,1 ,1,0,0,0,1 ,1,1,1,1,1 ,1,1,1,1,1}};

    public int[][] Map2 { get { return _MAP2; } }
    public int Map2Cells { get { return _MAP2.Length * _MAP2[0].Length; } }

    static readonly int[][] _MAP3 =  {
                                                new int[] {0,0,0,0,0,0,0,0,0,1,0,0,0,0},
                                                new int[] {0,0,0,0,0,0,0,0,0,1,0,0,0,0},
                                                new int[] {0,0,0,0,0,0,0,0,0,1,0,0,0,0},
                                                new int[] {0,0,0,0,0,0,0,0,0,1,0,0,0,0},
                                                new int[] {0,0,0,0,0,0,0,0,0,0,0,0,0,0},
                                                new int[] {0,0,0,0,0,0,0,0,0,0,0,0,0,0},
                                                new int[] {0,0,0,0,0,0,0,0,0,0,0,0,0,0},
                                                new int[] {0,0,0,0,0,0,0,0,0,1,0,0,0,0},
                                                new int[] {0,0,0,0,0,0,0,0,0,1,0,0,0,0},
                                                new int[] {0,0,0,0,0,0,0,0,0,1,0,0,0,0},
                                                new int[] {0,0,0,0,0,0,0,0,0,1,0,0,0,0},
                                            };
    public int[][] Map3 { get { return _MAP3; } }
    public int Map3Cells { get { return _MAP3.Length * _MAP3[0].Length; } }
    

    public int levelNumber; // select level number 
   
    private int _X_RESOLUTION; // metres per array-cell in x dimension
    private int _Z_RESOLUTION; // metres per array-cell in y dimension 

    public int ResolutionX { get { return _X_RESOLUTION; } }
    public int ResolutionZ { get { return _Z_RESOLUTION; } }

    private int _X_ORIGIN; // x coordinate of origin of map in array coordinates
    private int _Z_ORIGIN; // y coordinate of origin of map in array coordinates

    public int OriginX { get { return _X_ORIGIN; } }
    public int OriginZ { get { return _Z_ORIGIN; } }

    void Start()
    {
        _X_RESOLUTION = 1;
        _Z_RESOLUTION = 1;

        if (levelNumber == 1)
        {
            _X_ORIGIN = _MAP1[0].Length / 2;
            _Z_ORIGIN = _MAP1.Length / 2;
        }
        else if (levelNumber == 2)
        {
            _X_ORIGIN = _MAP2[0].Length / 2;
            _Z_ORIGIN = _MAP2.Length / 2;
        }
        else if (levelNumber == 3)
        {
            _X_ORIGIN = _MAP3[0].Length / 2;
            _Z_ORIGIN = _MAP3.Length / 2;
        }
    }

    public void ConvertCellToCartesian(int x_c, int z_c, out float x, out float z)
    {
        x = ((x_c - _X_ORIGIN) / _X_RESOLUTION);
        z = ((z_c - _Z_ORIGIN) / (-1 * _Z_RESOLUTION));
    }

    public void ConvertCartesianToCell(float x_m, float z_m, out int x, out int z)
    {
        x = (int)Math.Round(x_m * _X_RESOLUTION + _X_ORIGIN, 0);
        z = (int)Math.Round(-1 * z_m * _Z_RESOLUTION + _Z_ORIGIN, 0);
    }

    public int GetFreeCells(int[][] map)
    {
        // gets the free cells in the map 
        int numberFreeCells = 0;
        for (int i = 0; i < map.Length; i++)
        {
            for (int j = 0; j < map[0].Length; j++)
            {
                if (map[i][j] == 0)
                    numberFreeCells++;
            }
        }
        return numberFreeCells;
    }

    public void InflateObstacleLayer(ref int[][] map)
    {
        // inflates obstacles to avoid path planning too close to obstacles
        // this is done after the map is created, so the inflates obstacles are not instantiated
        // but just used for path planning 
        int[,] toIgnore = new int[map.Length, map[0].Length];

        for (int i = 0; i < map.Length; i++)
        {
            for (int j = 0; j < map[0].Length; j++)
            {
                if (map[i][j] == 1 && toIgnore[i,j] == 0)
                {
                    int[,] neighbours = new int[,] { { 0, -1 }, { 0, 1 }, { -1, 0 },
                        { 1, 0 }, { -1, -1 }, { -1, 1 }, { 1, -1 }, { 1, 1 } };

                    for (int k = 0; k < neighbours.GetLength(0); k++)
                    {
                        int x = j + neighbours[k, 0];
                        int y = i + neighbours[k, 1];

                        if (x < 0 || y < 0 || x >= map[0].Length || y >= map.Length)
                            continue;

                        if (map[y][x] == 0)
                        {
                            toIgnore[y,x] = 1;
                            map[y][x] = 1;
                        }
                    }
                }
            }
        }
    }

}
