using System;
using System.Collections;
using System.Collections.Generic;
using Damage;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

public class HammerSpecialAttackConfigAuthoring : MonoBehaviour
{
    [Header("Indicator")]
    [Tooltip("Indicator that spawns when hammer starts to charge")]
    [SerializeField] private GameObject indicatorPrefab;
    
    [Header("Zap")]
    [Tooltip("Zap object that spawns when the hammer travels.")]
    [SerializeField] private GameObject electricZapPrefab;
    [Tooltip("Minimum time between zaps.")]
    [SerializeField] private float minTimeBetweenZaps = 0.5f;
    [Tooltip("Maximum time between zaps.")]
    [SerializeField] private float maxTimeBetweenZaps = 1.5f;
    
    [Header("Travel")]
    [Tooltip("How far the hammer travels. Can be modified by charge stages (see component below).")]
    [SerializeField] private float baseDistanceToTravel = 20;
    [Tooltip("After how many seconds should the hammer turn back?")]
    [SerializeField] private float timeToTurnBack = 2f;
    [Tooltip("After how many seconds should the hammer return to the player after it has turned around?")]
    [SerializeField] private float timeToReturnAfterTurning = 2f;
    
    [Header("Rotation")]
    [Tooltip("How fast the hammer spins.")]
    [SerializeField] private float revolutionsPerSecond = 2f;
    [Tooltip("In what direction does the hammer rotate? ([0,1,0] means it rotates along its y-axis.)")]
    [SerializeField] private float3 rotationAxis = new float3(0,0,1);
    
    [Header("Catching")]
    [Tooltip("How far from the player should the hammer be grabbed?")]
    [SerializeField] private float distanceFromPlayerToGrab = 1f;

    [Header("Canceling")] 
    [Tooltip("How many seconds after throwing the hammer can the attack be canceled?")]
    [SerializeField] private float cancelDelayTime = 0.5f;

    [Header("Audio")] 
    [SerializeField] private AudioData throwImpactAudioData;
    [SerializeField] private AudioData throwingAudioData; 

    class Baker : Baker<HammerSpecialAttackConfigAuthoring>
    {
        public override void Bake(HammerSpecialAttackConfigAuthoring authoring)
        {
            var entity = GetEntity(TransformUsageFlags.None);
            AddComponent(entity, new HammerSpecialConfig
            {
                IndicatorPrefab = GetEntity(authoring.indicatorPrefab, TransformUsageFlags.None),
                ElectricChargePrefab = GetEntity(authoring.electricZapPrefab, TransformUsageFlags.None),
                
                BaseDistanceToTravel = authoring.baseDistanceToTravel,
                DistanceToTravel = authoring.baseDistanceToTravel,
                
                MinTimeBetweenZaps = authoring.minTimeBetweenZaps,
                MaxTimeBetweenZaps = authoring.maxTimeBetweenZaps,
                
                TimeToSwitchBack = authoring.timeToTurnBack,
                TimeToReturnAfterTurning = authoring.timeToReturnAfterTurning,
                
                DistanceFromPlayerToGrab = authoring.distanceFromPlayerToGrab,
                
                RotationDegreesPerSecond = math.radians(authoring.revolutionsPerSecond) * 360f,
                RotationVector = math.normalizesafe(authoring.rotationAxis),
                
                CurrentDistanceFromPlayer = float.MaxValue,
                
                CancelDelayTime = authoring.cancelDelayTime,
                
                throwImpactAudioData = authoring.throwImpactAudioData,
                throwingAudioData = authoring.throwingAudioData
            });
        }
    }
}

public struct HammerSpecialConfig : IComponentData
{
    public Entity IndicatorPrefab;
    public Entity ElectricChargePrefab;
    
    public float BaseDistanceToTravel;
    public float DistanceToTravel;
    
    public float MinTimeBetweenZaps;
    public float MaxTimeBetweenZaps;
    public float NextTimeBetweenZaps;
    
    public float TimeOfLastZap;
    public float Timer;

    public float RotationDegreesPerSecond;
    public float3 RotationVector;

    public bool HasSwitchedBack;
    public bool HasReturned;
    
    public float TimeToSwitchBack;
    public float TimeToReturnAfterTurning;
    public float CancelDelayTime;

    public float TravelForwardSpeed => DistanceToTravel / TimeToSwitchBack;


    public bool HasStarted;
    public float3 DirectionOfTravel;

    public float DistanceFromPlayerToGrab;
    public float CurrentDistanceFromPlayer;

    public KnockDirectionType KnockBackBeforeSpecial;

    public AudioData throwImpactAudioData;
    public AudioData throwingAudioData;
    
    public AudioData originalImpactAudio;
}
