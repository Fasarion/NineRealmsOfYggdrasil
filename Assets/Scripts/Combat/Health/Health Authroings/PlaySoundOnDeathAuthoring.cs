using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

public class PlaySoundOnDeathAuthoring : MonoBehaviour
{
    [SerializeField] private AudioData audioData;
    
    class Baker : Baker<PlaySoundOnDeathAuthoring>
    {
        public override void Bake(PlaySoundOnDeathAuthoring authoring)
        {
            var entity = GetEntity(TransformUsageFlags.Dynamic);
            AddComponent(entity, new PlaySoundOnDeathComponent
            {
                AudioData = authoring.audioData
            });
        }
    }
}

public struct PlaySoundOnDeathComponent : IComponentData
{
    public AudioData AudioData;
}