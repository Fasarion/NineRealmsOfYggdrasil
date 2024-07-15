using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;




public class BirdnadoSpawnerAuthoring : MonoBehaviour
{
    [Header("Tornado")]
    [Tooltip("Prefab of the tornado entity.")]
    [SerializeField] private GameObject tornadoEntityPrefab;
    [Tooltip("How the mid point of the tornado is offset from the bird circle. Will have an effect on suction direction.")]
    [SerializeField] private float3 tornadoOffset = new float3(0, 3, 0);
    [Tooltip("How much delay between each suction towards the tornado mid point.")]
    [SerializeField] private float timeBetweenSuctions = 0.3f;
    [Tooltip("Radius of the tornado.")]
    [SerializeField] private float tornadoRadius = 2f;
    [Tooltip("The tornado's final damage will be modified by this amount.")]
    [SerializeField] private float tornadoDamageModifier = 0.2f;
    
    [Tooltip("How long this attack lasts.")]
    [SerializeField] private float lifeTime = 2f;

    // [Tooltip("Context of the birdnado spawn. Used to differentiate different birdnados.")] 
    // [SerializeField] private BirdnadoSpawnerType birdnadoType;
    
    [Header("Audio")] 
    [SerializeField] private AudioData tornadoSound;

    [SerializeField] private bool useMouse;
    

    class Baker : Baker<BirdnadoSpawnerAuthoring>
    {
        public override void Bake(BirdnadoSpawnerAuthoring authoring)
        {
            var entity = GetEntity(TransformUsageFlags.None);
            AddComponent(entity, new BirdnadoSpawnerComponent
            {
                TornadoPrefab = GetEntity(authoring.tornadoEntityPrefab, TransformUsageFlags.Dynamic),
                TornadoOffset = authoring.tornadoOffset,
                TimeBetweenSuctions = authoring.timeBetweenSuctions,
                TornadoRadius = authoring.tornadoRadius,
                TornadoDamageMod = authoring.tornadoDamageModifier,
                
                LifeTime = authoring.lifeTime,
                TornadoSound = authoring.tornadoSound,
            });
            
            AddComponent(entity, new ShouldSpawnBirdnado());
            SetComponentEnabled<ShouldSpawnBirdnado>(entity, false);
        }
    }
}

public struct BirdnadoSpawnerComponent : IComponentData
{
    public Entity TornadoPrefab;
    public float3 TornadoOffset;
    public float TimeBetweenSuctions;
    public float TornadoRadius;
    public float TornadoDamageMod;
    
    public Entity CenterPointEntity;

    // public BirdnadoSpawnerType BirdnadoType;
    
    public float LifeTime;

    public bool IsActive;

    public AudioData TornadoSound;
}

public struct ShouldSpawnBirdnado : IComponentData, IEnableableComponent{}
