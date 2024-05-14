using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

public class BirdsSpecialAttackConfigAuthoring : MonoBehaviour
{
    [Header("Angular speed")]
    [SerializeField] private List<AngularSpeedChargeStageBuff> angularSpeedBuffs;
    [Space]
    [SerializeField] private float baseAngularSpeedDuringCharge = 2f;
    [SerializeField] private float baseAngularSpeedAfterRelease = 5f;
    
    [Header("Bird Settings")]
    [SerializeField] private int birdCount = 2;
    
    [Header("Radius")]
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
                AngularSpeedDuringCharge = authoring.baseAngularSpeedDuringCharge,
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
    public int BirdCount;
    public float Radius;
    public float AngleStep;
    public float AngularSpeedDuringCharge;
    public float AngularSpeedAfterRelease;
    
    public bool HasStartedInitialChargePhase;
    public bool HasStartedReleasedChargePhase;
}

[Serializable]
public struct AngularSpeedChargeStageBuff
{
    public float DuringChargeBuff;
    public float AfterReleaseBuff;
}

public struct AngularSpeedChargeStageBuffElement : IBufferElementData
{
    public AngularSpeedChargeStageBuff Value;
}


