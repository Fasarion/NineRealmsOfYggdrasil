using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using Random = System.Random;

public class GridManager : MonoBehaviour
{
    
    [SerializeField] private int width;
    [SerializeField] private int height;

    [SerializeField] private Tile tilePrefab;
    [SerializeField] private GameObject targetObject;

    [SerializeField] private List<GameObject> possibleObjects;
    // Start is called before the first frame update
    [SerializeField] private int randomSeed;
    private Random randomizer;
    private void Start()
    {
        
        randomizer = new Random(randomSeed);
        
        var targetBounds = GetMeshBounds(targetObject);
        var tileBounds = GetMeshBounds(tilePrefab.gameObject);
        GenerateGrid(targetBounds.x, targetBounds.z, tileBounds );
        
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
        
        //Checks which index in the grid that is roughly at the centre.
        Vector2 tileCenter = GetTileCenter(gridIntWidth, gridIntHeight);
        
        for (int x = 0; x < gridIntWidth; x++) 
        {
            for (int y = 0; y < gridIntHeight; y++)
            {
                
                
                
                var chosenObjectIndex = randomizer.Next(0, possibleObjects.Count);
                var spawnedTile = Instantiate(possibleObjects[chosenObjectIndex], new Vector3(x * tileSize.x, 0, y * tileSize.z), Quaternion.identity);
                string tileName = "Tile " + x +  "," + y;
            }
        }
    }

    public Vector2Int GetTileCenter(int gridWidth, int gridHeight)
    {
        return new Vector2Int(Mathf.CeilToInt(gridWidth / 2f), Mathf.CeilToInt(gridHeight / 2f)) ;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnDrawGizmosSelected()
    {
        
    } 
}
