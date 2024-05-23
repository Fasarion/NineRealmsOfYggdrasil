using System.Collections;
using System.Collections.Generic;
using Movement;
using Patrik;
using Unity.Entities;
using UnityEngine;

public enum AnimationEnemyType
{
    Melee = 0,
    Ranged = 1,
}

public class EnemyAnimatorControllerAuthoring : MonoBehaviour
{
    public AnimationEnemyType EnemyAnimationType;
    
    class Baker : Baker<EnemyAnimatorControllerAuthoring>
    {
        public override void Bake(EnemyAnimatorControllerAuthoring authoring)
        {
            var entity = GetEntity(TransformUsageFlags.Dynamic);
            AddComponent(entity, new EnemyAnimatorControllerComponent
            {
                EnemyType = authoring.EnemyAnimationType
            }); 
            
            AddComponent(entity, new HasSetupEnemyAnimator { });
            SetComponentEnabled<HasSetupEnemyAnimator>(entity, false);
            
        }
    }
}

public struct EnemyAnimatorControllerComponent : IComponentData
{
    public AnimationEnemyType EnemyType;
}

public struct HasSetupEnemyAnimator : IComponentData, IEnableableComponent{}

