using System.Collections;
using System.Collections.Generic;
using FMODUnity;
using Unity.Entities;
using UnityEngine;

public class PlaySoundOnBeingHitAuthoring : MonoBehaviour
{
    [SerializeField] private AudioData audioData;
    
    class Baker : Baker<PlaySoundOnBeingHitAuthoring>
    {
        public override void Bake(PlaySoundOnBeingHitAuthoring authoring)
        {
            var entity = GetEntity(TransformUsageFlags.Dynamic);
            AddComponent(entity, new PlaySoundOnBeingHitComponent
            {
                Value = authoring.audioData,
            });
        }
    }
}

public struct PlaySoundOnBeingHitComponent : IComponentData
{
    public AudioData Value;
}