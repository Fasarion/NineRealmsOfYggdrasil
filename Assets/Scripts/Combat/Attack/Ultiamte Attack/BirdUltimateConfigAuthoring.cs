using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

public class BirdUltimateConfigAuthoring : MonoBehaviour
{
    [Header("Tornado")]
    [Tooltip("Prefab of the tornado entity.")]
    [SerializeField] private GameObject tornadoEntityPrefab; 
    
    [Header("Angular speed")]
    [Tooltip("How fast the birds spin.")]
    [SerializeField] private float angularSpeed = 2f; 
    
    [Header("Bird Settings")]
    [Tooltip("How many birds that are spawned in this attack.")]
    [SerializeField] private int birdCount = 2;
    [Tooltip("How long this attack lasts.")]
    [SerializeField] private float lifeTime = 2f;
    
    [Header("Radius")]
    [Tooltip("Radius of the circle that the birds circle around in.")]
    [SerializeField] private float circleRadius = 2f;

    private void OnValidate()
    {
        if (birdCount <= 0)
        {
            birdCount = 1;
            Debug.LogWarning("Bird Count must be positive.");
        }
    }

    class Baker : Baker<BirdUltimateConfigAuthoring>
    {
        public override void Bake(BirdUltimateConfigAuthoring authoring)
        {
            var entity = GetEntity(TransformUsageFlags.None);
            AddComponent(entity, new BirdsUltimateAttackConfig
            {
                TornadoPrefab = GetEntity(authoring.tornadoEntityPrefab, TransformUsageFlags.Dynamic),
                
                BirdCount = authoring.birdCount,
                
                Radius = authoring.circleRadius,
                
                AngleStep = 360f / authoring.birdCount,
                AngularSpeed = authoring.angularSpeed,
                LifeTime = authoring.lifeTime
            });
        }
    }
}

public struct BirdsUltimateAttackConfig : IComponentData
{
    public Entity TornadoPrefab;
    public Entity CenterPointEntity;
    
    public int BirdCount;
    
    public float Radius;

    public float AngleStep;
    public float AngularSpeed;
    
    public float LifeTimeTimer;
    public float LifeTime;

    public bool IsActive;
}