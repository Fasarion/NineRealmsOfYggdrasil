using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using Random = System.Random;

public class GridManager : MonoBehaviour
{
    
    //[SerializeField] private int width;
    //[SerializeField] private int height;

    [SerializeField] private Tile tilePrefab;
    [SerializeField] private GameObject targetObject;

    [SerializeField] private List<GameObject> possibleOuterObjects;

    [SerializeField] private List<GameObject> possibleInnerObjects;
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
        Vector2 centerTile = GetCenterTile(gridIntWidth, gridIntHeight);
        Vector2 mapCenter = new Vector2( centerTile.x,  centerTile.y);
        for (int x = 0; x < gridIntWidth; x++) 
        {
            for (int y = 0; y < gridIntHeight; y++)
            {

                var distanceToTileCenter = Vector2.Distance(new Vector2(x, y), mapCenter);
                
                var chosenOuterObjectIndex = randomizer.Next(0, possibleOuterObjects.Count);
                var chosenInnerObjectIndex = randomizer.Next(0, possibleInnerObjects.Count);

                if (distanceToTileCenter < 5.0f)
                {
                    var spawnedTile = Instantiate(possibleInnerObjects[chosenInnerObjectIndex], new Vector3(x * tileSize.x, 0, y * tileSize.z), Quaternion.identity);

                }
                else
                {
                    var spawnedTile = Instantiate(possibleOuterObjects[chosenOuterObjectIndex], new Vector3(x * tileSize.x, 0, y * tileSize.z), Quaternion.identity);

                }
                string tileName = "Tile " + x +  "," + y;
            }
        }
    }

    public Vector2Int GetCenterTile(int gridWidth, int gridHeight)
    {
        return new Vector2Int(Mathf.FloorToInt(gridWidth / 2f), Mathf.FloorToInt(gridHeight / 2f)) ;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnDrawGizmosSelected()
    {
        
    } 
}
