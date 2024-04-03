using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class GridManager : MonoBehaviour
{
    
    [SerializeField] private int width;
    [SerializeField] private int height;

    [SerializeField] private Tile tilePrefab;
    [SerializeField] GameObject targetObject;
    // Start is called before the first frame update

    private void Start()
    {
        var targetBounds = GetMeshBounds(targetObject);
        var tileBounds = GetMeshBounds(tilePrefab.gameObject);
        GenerateGrid(targetBounds.x, targetBounds.z, tileBounds);
        
    }

    private Vector3 GetMeshBounds(GameObject meshObject)
    {
        var targetMesh = meshObject.GetComponent<MeshFilter>().sharedMesh;
        var targetScale = meshObject.transform.lossyScale;
        Vector3 targetObjectExtents = new  Vector3(targetMesh.bounds.extents.x * targetScale.x, targetMesh.bounds.extents.y * targetScale.y, targetMesh.bounds.extents.z * targetScale.z) * 2;

        return targetObjectExtents;
    }
    void GenerateGrid(float gridWidth, float gridHeight, Vector3 tileSize)
    {
        
        
        
        int gridIntWidth = Mathf.CeilToInt(gridWidth / tileSize.x);
        int gridIntHeight = Mathf.CeilToInt(gridHeight / tileSize.z);

        for (int x = 0; x < gridIntWidth; x++)
        {
            for (int y = 0; y < gridIntHeight; y++)
            {
                var spawnedTile = Instantiate(tilePrefab, new Vector3(x * tileSize.x, 0, y * tileSize.z), Quaternion.identity);
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
