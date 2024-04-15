using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class SpawningController : MonoBehaviour
{
    [Tooltip("The time (in seconds) after which different checkpoints become active (time based on a global timer).")]
    public List<int> spawningCheckpointTimes;
    [Tooltip("Specialized data for each checkpoint to use. Corresponds to the index in the SpawningCheckpointTimes list!")]
    public List<SpawningTimerCheckpointObject> checkpointData;

    [Tooltip("The shortest possible time between enemy spawns.")]
    public float minTimerTime;
    [Tooltip("The longest possible time between enemy spawns.")]
    public float maxTimerTime;

    [Tooltip("The minimum number of enemies allowed to spawn in a single batch, as a percentage of the target enemy count. Should be a value between 0 and 1!")]
    public float minEnemySpawnPercent;
    [Tooltip("The maximum number of enemies allowed to spawn in a single batch, as a percentage of the target enemy count. Should be a value between 0 and 1!")]
    public float maxEnemySpawnPercent;

    [Tooltip("The inner ring of the spawning area. Place a Transform in the spawningCenterPoint field to see" +
             "a visualization of the area.")]
    public float innerSpawningRadius;
    [Tooltip("The inner ring of the spawning area. Place a Transform in the spawningCenterPoint field to see\" +\n             \"a visualization of the area.")]
    public float outerSpawningRadius;

    [Header("--Debug Values--")] 
    [Tooltip("The checkpoint (based on index in the CheckpointData list) that should be active on game start. " +
             "Only change from 0 for debug purposes!")]
    public int startingCheckpointIndex = 0;

    [Tooltip("The centerpoint for the spawner. This will almost always be the player, so this field is only for" +
             "visualization purposes.")]
    public Transform spawningCenterPoint;
    
    [Tooltip("The THICKNESS of the border of the visualization area. Will only display if a spawning center point " +
             "is set.")]
    [Range(0, 3f)]
    public float VisualizationThickness = 0.05f;
    
    // Static variable to hold the instance
    private static SpawningController _instance;

    // Public property to access the instance
    public static SpawningController Instance
    {
        get
        {
            // Check if the instance is null
            if (_instance == null)
            {
                // Find the instance in the scene
                _instance = FindObjectOfType<SpawningController>();
            }
            // Return the instance
            return _instance;
        }
    }

    void OnDrawGizmosSelected()
    {
        if (spawningCenterPoint == null) return;
        
        Gizmos.color = Color.yellow;

        // Draw the torus
        for (float theta = 0; theta < Mathf.PI * 2; theta += 0.1f)
        {
            // Calculate x and z coordinates for inner and outer radii
            float xInner = spawningCenterPoint.position.x + innerSpawningRadius * Mathf.Cos(theta);
            float zInner = spawningCenterPoint.position.z + innerSpawningRadius * Mathf.Sin(theta);

            float xOuter = spawningCenterPoint.position.x + outerSpawningRadius * Mathf.Cos(theta);
            float zOuter = spawningCenterPoint.position.z + outerSpawningRadius * Mathf.Sin(theta);

            // Draw spheres at each point for inner and outer radii
            Vector3 pointInner = new Vector3(xInner, spawningCenterPoint.position.y, zInner);
            Vector3 pointOuter = new Vector3(xOuter, spawningCenterPoint.position.y, zOuter);
        
            Gizmos.DrawSphere(pointInner, VisualizationThickness);
            Gizmos.DrawSphere(pointOuter, VisualizationThickness);
        }
    }

    private void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
            //DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
}

[CreateAssetMenu]
public class SpawningTimerCheckpointObject : ScriptableObject
{
    [Tooltip("The amount of enemies that ideally should exist during the checkpoint")]
    public int targetEnemyCount;

    [Tooltip("Information about which enemy types are allowed to spawn during this checkpoint" +
             "and their weights. You can add as many items as you wish to this list!")]
    public List<EnemyTypesInformation> enemyTypesInformation;
}

[System.Serializable]
public struct EnemyTypesInformation
{
    [Tooltip("An enemy type that's allowed to spawn during the checkpoint.")]
    public EnemyType enemyType;

    [Tooltip("The weight determining the spawning frequency of an enemy type." +
             "If one enemytype A has a weight of 2 and another enemytype B has a weight of 1 then type A will spawn 66% of the time and type B 33%, and so on." +
             "")]
    public float enemyWeight;
}