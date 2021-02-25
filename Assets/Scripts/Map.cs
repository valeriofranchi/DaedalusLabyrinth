using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Map : MonoBehaviour
{
    public Wall wallPrefab;
    public Floor floorPrefab;

    public Material wallMaterial;
    public Material floorMaterial;

    public MapManager mapManager;

    void Start()
    {

        if (mapManager.levelNumber == 1)
        {
            GenerateMapSimple(mapManager.Map1);
        }
        else if (mapManager.levelNumber == 2)
        {
            GenerateMapSimple(mapManager.Map2);
        }
		else
        {
            Debug.LogError("Level Number Incomplete");
        }
    }

    void GenerateMapSimple(int[][] mapArray)
    {
        // generates map from 2d array with prefabs 
        for (int i = 0; i < mapArray.Length; i++)
        {
            for (int j = 0; j < mapArray[i].Length; j++)
            {
                if (mapArray[i][j] == 1)
                {
                    float x, z;
                    mapManager.ConvertCellToCartesian(j, i, out x, out z);
                    Wall w = Instantiate(wallPrefab, new Vector3(x, 2.50f, z), Quaternion.identity, transform);
                    MeshRenderer rend1 = w.GetComponent<MeshRenderer>();
                    rend1.material = wallMaterial;
                    w.name = "WallSection " + j + "," + i ;
                    w.gameObject.tag = "Obstacle";
                    w.gameObject.layer = LayerMask.NameToLayer("Obstacle");
                }

                float x2, z2;
                mapManager.ConvertCellToCartesian(j, i, out x2, out z2);
                Floor f = Instantiate(floorPrefab, new Vector3(x2, 0.0f, z2), Quaternion.Euler(90.0f, 0.0f, 0.0f), transform);
                MeshRenderer rend2 = f.GetComponent<MeshRenderer>();
                rend2.material = floorMaterial;
                f.name = "FloorSection " + j + "," + i;
                f.gameObject.tag = "Floor";
                f.gameObject.layer = LayerMask.NameToLayer("Floor");
            }
        }
    }
}