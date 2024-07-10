using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

public class BirdsNormalAttackConfigAuthoring : MonoBehaviour
{
    [Header("Tornado Options")] 
   // [SerializeField] private GameObject torndaoPrefab;
    
    [SerializeField] private bool spawnTornados;
    [Tooltip("After how many attacks should a tornado spawn?")]
    [SerializeField] private int attackTornadoSpawnInterval = 6;
    
    [Header("Movement Options")] 
    [Tooltip("How much time it should take for the bird to come back to the player once it has started moving.")]
    [SerializeField] private float timeToCompleteMotion = 1;
    
    [Tooltip("First point on a bezier curve that decides the birds movement.")]
    [SerializeField] private float2 controlPoint1;
    
    [Tooltip("Second point on a bezier curve that decides the birds movement.")]
    [SerializeField] private float2 controlPoint2;

    private void OnValidate()
    {
        if (timeToCompleteMotion <= 0)
        {
            timeToCompleteMotion = 1;
            Debug.LogWarning("'Time To Complete Motion' must have a positive value.", this);
        }

        // if (spawnTornados && torndaoPrefab == null)
        // {
        //     Debug.LogError("Tornado Prefab not assigned to config.", this);
        // }
    }

    class Baker : Baker<BirdsNormalAttackConfigAuthoring>
    {
        public override void Bake(BirdsNormalAttackConfigAuthoring configAuthoring)
        {
            var entity = GetEntity(TransformUsageFlags.Dynamic);

            var cp1 = configAuthoring.controlPoint1;
            var cp2 = configAuthoring.controlPoint2;
            
            AddComponent(entity, new BirdNormalAttackConfig
            {
                controlPoint1 = new float4(cp1.x, 0, cp1.y, 1),
                controlPoint2 = new float4(cp2.x, 0, cp2.y, 1),
                timeToCompleteMovement = configAuthoring.timeToCompleteMotion,
                
             //   TornadoPrefab = GetEntity(configAuthoring.torndaoPrefab, TransformUsageFlags.Dynamic),
                spawnTornados = configAuthoring.spawnTornados,
                attackTornadoSpawnInterval = configAuthoring.attackTornadoSpawnInterval,
                currentIndex = 0
            });
        }
    }
}

public struct BirdNormalAttackConfig : IComponentData
{
    public float4 controlPoint1;
    public float4 controlPoint2;
    public float timeToCompleteMovement;

    public Entity TornadoPrefab;
    public bool spawnTornados;
    public int attackTornadoSpawnInterval;

    public int currentIndex;
}