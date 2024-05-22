using Unity.Entities;
using UnityEngine;

public struct SpawnEntityOnDestroyElement : IBufferElementData
{
    public Entity Entity;
    public SpawnSettings Settings;

}

[System.Serializable]
public struct SpawnSettings
{
    [Tooltip("Mark this if the scale should be set to the value \"New Scale\". " +
             "Otherwise, the value defaults to that of the prefab.")]
    public bool SetScale;
    
    [Tooltip("New scale set to the spawned entity.")]
    public float NewScale;

    public SpawnSettings(bool setScale, float newScale)
    {
        SetScale = setScale;
        NewScale = newScale;
    }

    public static SpawnSettings Default => new SpawnSettings(false, 1);
}

[System.Serializable]
public struct SpawnObjectContents
{
    [Tooltip("Object that's spawned is based on this prefab.")]
    public GameObject SpawnPrefab;

    [Tooltip("Settings to modify the spawned object.")]
    public SpawnSettings SpawnSettings;
    
    public SpawnObjectContents(GameObject prefab)
    {
        SpawnPrefab = prefab;
        SpawnSettings = SpawnSettings.Default;
    }

    public static SpawnObjectContents Default => new SpawnObjectContents(default);
}