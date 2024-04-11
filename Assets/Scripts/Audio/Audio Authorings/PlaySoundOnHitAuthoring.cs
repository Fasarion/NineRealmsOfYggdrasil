using System.Collections;
using System.Collections.Generic;
using FMODUnity;
using Unity.Entities;
using UnityEngine;

public class PlaySoundOnHitAuthoring : MonoBehaviour
{
    [SerializeField] private AudioData audioData;
    
    class Baker : Baker<PlaySoundOnHitAuthoring>
    {
        public override void Bake(PlaySoundOnHitAuthoring authoring)
        {
            var entity = GetEntity(TransformUsageFlags.Dynamic);
            AddComponent(entity, new PlaySoundOnHitComponent
            {
                Value = authoring.audioData,
            });
        }
    }
}

public struct PlaySoundOnHitComponent : IComponentData
{
    public AudioData Value;
}