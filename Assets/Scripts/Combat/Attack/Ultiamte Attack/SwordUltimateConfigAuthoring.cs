using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

public class SwordUltimateConfigAuthoring : MonoBehaviour
{
    [SerializeField] private int numberOfScaledAttacks = 3;
    [SerializeField] private float scaleIncrease = 2f;

    [Header("Beam")] 
    [SerializeField] private GameObject beamPrefab; 
    [SerializeField] private GameObject beamVfx; 
    [SerializeField] private float beamSpawnTimeAfterAttack = 0.3f; 
    [Header("Audio")] 
    [SerializeField] private AudioData onUseAudioData;
    class Baker : Baker<SwordUltimateConfigAuthoring>
    {
        public override void Bake(SwordUltimateConfigAuthoring authoring)
        {
            var entity = GetEntity(TransformUsageFlags.None);
            AddComponent(entity, new SwordUltimateConfig
            {
                NumberOfScaledAttacks = authoring.numberOfScaledAttacks,
                ScaleIncrease = authoring.scaleIncrease,
                
                BeamEntityPrefab = GetEntity(authoring.beamPrefab, TransformUsageFlags.Dynamic),
                BeamVfxPrefab = GetEntity(authoring.beamVfx, TransformUsageFlags.Dynamic),
                
                BeamSpawnTimeAfterAttackStart = authoring.beamSpawnTimeAfterAttack,
                
                onUseAudioData = authoring.onUseAudioData
            });
        }
    }
}

public struct SwordUltimateConfig : IComponentData
{
    public Entity BeamEntityPrefab;
    public Entity BeamVfxPrefab;
    
    public int NumberOfScaledAttacks;
    public float ScaleIncrease;
    
    public bool IsActive;
    public int CurrentAttackCount;

    public float CurrentTime;
    public float BeamSpawnTimeAfterAttackStart;

    public AudioData onUseAudioData;
}
