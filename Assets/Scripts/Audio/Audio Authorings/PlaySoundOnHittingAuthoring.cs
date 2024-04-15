using Unity.Entities;
using UnityEngine;

public class PlaySoundOnHittingAuthoring : MonoBehaviour
{
    [SerializeField] private AudioData audioData;
    
    class Baker : Baker<PlaySoundOnHittingAuthoring>
    {
        public override void Bake(PlaySoundOnHittingAuthoring authoring) 
        {
            var entity = GetEntity(TransformUsageFlags.Dynamic);
            AddComponent(entity, new PlaySoundOnHittingComponent
            {
                Value = authoring.audioData,
            });
        }
    }
}

public struct PlaySoundOnHittingComponent : IComponentData
{
    public AudioData Value;
}