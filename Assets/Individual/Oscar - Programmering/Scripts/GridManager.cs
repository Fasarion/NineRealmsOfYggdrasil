using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridManager : MonoBehaviour
{
    
    [SerializeField] private int width;
    [SerializeField] private int height;

    [SerializeField] private Tile tilePrefab;
    [SerializeField] Mesh targetMesh;
    // Start is called before the first frame update

    private void Start()
    {
        GenerateGrid();
    }

    private void GetMeshBounds()
    {
        var xExtents = targetMesh.bounds.extents.x;
        var zExtents = targetMesh.bounds.extents.z;
        
        Debug.Log("xExtents: "+ xExtents);
        Debug.Log("zExtents: "+ zExtents);
    }
    void GenerateGrid()
    {
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                var spawnedTile = Instantiate(tilePrefab, new Vector3(x, 0, y), Quaternion.identity);
                string tileName = "Tile " + x +  "," + y;
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnDrawGizmosSelected()
    {
        
    } 
}
