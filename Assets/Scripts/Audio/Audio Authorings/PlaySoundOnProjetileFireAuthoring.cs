using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

public class PlaySoundOnProjetileFireAuthoring : MonoBehaviour
{
    [SerializeField] private AudioData audio;
    [Range(0,1)]
    [SerializeField] private float chanceToPlay = 0.3f;
    
    class Baker : Baker<PlaySoundOnProjetileFireAuthoring>
    {
        public override void Bake(PlaySoundOnProjetileFireAuthoring authoring)
        {
            var entity = GetEntity(TransformUsageFlags.Dynamic);
            AddComponent(entity, new PlaySoundOnProjectileFireComponent
            {
                AudioData = authoring.audio,
                ChanceToPlay = authoring.chanceToPlay
            });
        }
    }
}

public struct PlaySoundOnProjectileFireComponent : IComponentData
{
    public AudioData AudioData;
    public float ChanceToPlay;
}
