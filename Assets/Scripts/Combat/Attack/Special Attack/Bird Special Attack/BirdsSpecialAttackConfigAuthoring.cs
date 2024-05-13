using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

public class BirdsSpecialAttackConfigAuthoring : MonoBehaviour
{

    [SerializeField] private int birdCount = 2;
    [SerializeField] private float radius = 2f;

    private void OnValidate()
    {
        if (birdCount <= 0)
        {
            birdCount = 1;
            Debug.LogWarning("Bird Count must be positive.");
        }
    }

    class Baker : Baker<BirdsSpecialAttackConfigAuthoring>
    {
        public override void Bake(BirdsSpecialAttackConfigAuthoring authoring)
        {
            var entity = GetEntity(TransformUsageFlags.None);
            AddComponent(entity, new BirdsSpecialAttackConfig
            {
                BirdCount = authoring.birdCount,
                Radius = authoring.radius,
                AngleStep = 360f / authoring.birdCount,
            });
        }
    }
}

public struct BirdsSpecialAttackConfig : IComponentData
{
    public int BirdCount;
    public float Radius;
    public float AngleStep;
    
    public bool HasStartedInitialChargePhase;
    public bool HasStartedReleasedChargePhase;
}
