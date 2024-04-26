using System.Collections;
using System.Collections.Generic;
using Damage;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

public class HammerSpecialAttackConfigAuthoring : MonoBehaviour
{
    [Header("Indicator")]
    [SerializeField] private GameObject indicatorPrefab;
    
    [Header("Charge")]
    [SerializeField] private GameObject electricChargePrefab;
    [SerializeField] private float minTimeBetweenZaps = 0.5f;
    [SerializeField] private float maxTimeBetweenZaps = 1.5f;
    
    [Header("Travel ")]
    [SerializeField] private float distanceToTravel = 20;
    [SerializeField] private float timeToTurnBack = 2f;
    [SerializeField] private float timeToReturnAfterTurning = 2f;
    
    [Header("Catching")]
    [SerializeField] private float distanceFromPlayerToGrab = 1f;

    class Baker : Baker<HammerSpecialAttackConfigAuthoring>
    {
        public override void Bake(HammerSpecialAttackConfigAuthoring authoring)
        {
            var entity = GetEntity(TransformUsageFlags.None);
            AddComponent(entity, new HammerSpecialConfig
            {
                IndicatorPrefab = GetEntity(authoring.indicatorPrefab),
                ElectricChargePrefab = GetEntity(authoring.electricChargePrefab),
                
                DistanceToTravel = authoring.distanceToTravel,
                
                MinTimeBetweenZaps = authoring.minTimeBetweenZaps,
                MaxTimeBetweenZaps = authoring.maxTimeBetweenZaps,
                
                TimeToSwitchBack = authoring.timeToTurnBack,
                TimeToReturnAfterTurning = authoring.timeToReturnAfterTurning,
                
                TravelForwardSpeed = authoring.distanceToTravel / authoring.timeToTurnBack,
                DistanceFromPlayerToGrab = authoring.distanceFromPlayerToGrab,
                
                CurrentDistanceFromPlayer = float.MaxValue
            });
        }
    }
}

public struct HammerSpecialConfig : IComponentData
{
    public Entity IndicatorPrefab;
    public Entity ElectricChargePrefab;
    
    public float DistanceToTravel;
    
    public float MinTimeBetweenZaps;
    public float MaxTimeBetweenZaps;
    public float Timer;

    public bool HasSwitchedBack;
    public bool HasReturned;
    
    public float TimeToSwitchBack;
    public float TimeToReturnAfterTurning;

    public float TravelForwardSpeed;

    public bool HasStarted;
    public float3 DirectionOfTravel;

    public float DistanceFromPlayerToGrab;
    public float CurrentDistanceFromPlayer;

    public KnockDirectionType KnockBackBeforeSpecial;
}
