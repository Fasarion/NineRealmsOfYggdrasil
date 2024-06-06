using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

public class BirdsSpecialAttackConfigAuthoring : MonoBehaviour
{
    [Header("Angular speed")]
    [Tooltip("Speed modifiers that are applied for every charge stage.")]
    [SerializeField] private List<AngularSpeedChargeStageBuff> angularSpeedBuffs;
    [Space]
    [Tooltip("Base value for angular speed when the attack is charging (revolutions per second).")]
    [SerializeField] private float baseAngularSpeedDuringCharge = 2f;
    [Tooltip("Base value for angular speed when the attack is released (revolutions per second).")]
    [SerializeField] private float baseAngularSpeedAfterRelease = 5f;
    
    [Header("Bird Settings")]
    // [Tooltip("How many birds that are spawned in this attack.")]
    // [SerializeField] private int birdCount = 2;
    [Tooltip("For how long should this attack exist (after the charge has been released)?")]
    [SerializeField] private float lifeTimeAfterRelease = 2f;
    
    [Header("Radius")]
    [Tooltip("The radius of the circle when the charge starts.")]
    [SerializeField] private float initialRadius = 2f;
    [Tooltip("Circle's target radius during the charge stage.")]
    [SerializeField] private float targetRadius = 2f;
    [Tooltip("How fast the radius changes.")]
    [SerializeField] private float radiusIncreaseSpeed;
    
    [Header("Layer")]
    [Tooltip("How much space between layers of bird circles.")]
    [SerializeField] private float radiusIncreasePerLayer = 1f;
    [Tooltip("How the speed changes for each layer (0.9 means 10% slower)")]
    [SerializeField] private float angularSpeedModPerLayer = 0.9f;
    

    private void OnValidate()
    {
        // if (birdCount <= 0)
        // {
        //     birdCount = 1;
        //     Debug.LogWarning("Bird Count must be positive.");
        // }
    }

    class Baker : Baker<BirdsSpecialAttackConfigAuthoring>
    {
        public override void Bake(BirdsSpecialAttackConfigAuthoring authoring)
        {
            var entity = GetEntity(TransformUsageFlags.None);
            AddComponent(entity, new BirdsSpecialAttackConfig
            {
                //BirdCount = authoring.birdCount,
                
                TargetRadius = authoring.targetRadius,
                InitialRadius = authoring.initialRadius,
                CurrentRadius = authoring.initialRadius,
                RadiusIncreaseSpeed = authoring.radiusIncreaseSpeed,
                
                BirdLayers = 1,
                BirdLayersRadiusIncrease = authoring.radiusIncreasePerLayer,
                SpeedChangePerLayer = authoring.angularSpeedModPerLayer,
                
                //AngleStep = 360f / authoring.birdCount,
                
                AngularSpeedDuringCharge = authoring.baseAngularSpeedDuringCharge * math.PI * 2,
                AngularSpeedAfterRelease = authoring.baseAngularSpeedAfterRelease * math.PI * 2,
                LifeTimeAfterRelease = authoring.lifeTimeAfterRelease
            });

            var angularSpeedBuffer = AddBuffer<AngularSpeedChargeStageBuffElement>(entity);

            foreach (var speedBuff in authoring.angularSpeedBuffs)
            {
                angularSpeedBuffer.Add(new AngularSpeedChargeStageBuffElement {Value = speedBuff});
            }
        }
    }
}

public struct BirdsSpecialAttackConfig : IComponentData
{
    public Entity CenterPointEntity;
    
    public int BirdCount;
    
    public int BirdLayers;
    public float BirdLayersRadiusIncrease;
    
    public float CurrentRadius;
    public float InitialRadius;
    public float TargetRadius;
    public float RadiusIncreaseSpeed;
    
    public float AngleStep => BirdCount < 0 ? 180f : 360f / BirdCount;
    public float SpeedChangePerLayer { get; set; }

    public float AngularSpeedDuringCharge;
    public float AngularSpeedAfterRelease;
    
    public float lifeTimeTimer;
    public float LifeTimeAfterRelease;
    
    public bool InReleaseState;
    public bool HasReleased;
}

[Serializable]
public struct AngularSpeedChargeStageBuff
{
    [Tooltip("How much the speed is modified during this specific charge stage - during charge up.")]
    public float DuringChargeBuff;
    
    [Tooltip("How much the speed is modified during this specific charge stage - after the charge is released.")]
    public float AfterReleaseBuff;
}

public struct AngularSpeedChargeStageBuffElement : IBufferElementData
{
    public AngularSpeedChargeStageBuff Value;
}


