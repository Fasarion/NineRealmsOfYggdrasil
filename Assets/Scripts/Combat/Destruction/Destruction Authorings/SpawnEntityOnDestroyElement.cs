using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

public struct SpawnEntityOnDestroyElement : IBufferElementData
{
    public Entity Entity;
    public SpawnSettings Settings;
}

[System.Serializable]
public struct SpawnSettings 
{
    [Header("Scale")]
    [Tooltip("Mark this if the scale should be set to the value \"New Scale\". " +
             "Otherwise, the value defaults to that of the prefab.")]
    public bool SetScale;
    
    [Tooltip("New scale set to the spawned entity.")]
    public float NewScale;
    
    [Header("Y Position")]
    [Tooltip("Forces the y position of the spawned object to have a fixed value.")]
    public bool SetYPosition;
    
    [Tooltip("Offset added to spawn position.")]
    public float YPosition; 
}

[System.Serializable]
public struct SpawnObjectContents
{
    [Tooltip("Object that's spawned is based on this prefab.")]
    public GameObject SpawnPrefab;

    [Tooltip("Settings to modify the spawned object.")]
    public SpawnSettings SpawnSettings;
}