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
    [SerializeField] private int beamsPerSwing = 1; 
    [SerializeField] private float degreesBetweenBeams = 20f; 
    [Range(0, 360)]
    [SerializeField] private float maxTotalDegrees = 180f; 
    
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
                BeamsPerSwing = authoring.beamsPerSwing,
                degreesBetweenBeams = authoring.degreesBetweenBeams,
                MaximumTotalDegreesPerSide = authoring.maxTotalDegrees / 2f,
                
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
    public int BeamsPerSwing;
    public float degreesBetweenBeams;
    public float MaximumTotalDegreesPerSide;
    public bool PrepareBeam { get; set; }

    public AudioData onUseAudioData;
}
