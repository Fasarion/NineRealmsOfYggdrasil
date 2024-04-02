using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

public class PlayerGameObjectPrefab : IComponentData
{
    public GameObject Value;
}

public class PlayerAnimatorReference : ICleanupComponentData
{
    public Animator Value;
}

public class AnimationTestAuthoring : MonoBehaviour
{
    public GameObject PlayerGameObjectPrefab;

    public class PlayerGameObjectPrefabBaker : Baker<AnimationTestAuthoring>
    {
        public override void Bake(AnimationTestAuthoring authoring)
        {
            var entity = GetEntity(TransformUsageFlags.Dynamic);
            AddComponentObject(entity, new PlayerGameObjectPrefab{Value = authoring.PlayerGameObjectPrefab});
        }
    }
}
