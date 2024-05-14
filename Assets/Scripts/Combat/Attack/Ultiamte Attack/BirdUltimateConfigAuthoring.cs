using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

public class BirdUltimateConfigAuthoring : MonoBehaviour
{
    [Header("Angular speed")]
    [SerializeField] private float angularSpeed = 2f; 
    
    [Header("Bird Settings")]
    [SerializeField] private int birdCount = 2;
    [SerializeField] private float lifeTime = 2f;
    
    [Header("Radius")]
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
    public int BirdCount;
    
    public float Radius;

    public float AngleStep;
    public float AngularSpeed;
    
    public float LifeTimeTimer;
    public float LifeTime;

    public bool IsActive;
}