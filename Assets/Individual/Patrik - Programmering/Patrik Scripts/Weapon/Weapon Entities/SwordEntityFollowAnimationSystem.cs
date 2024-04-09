using Unity.Entities;
using Unity.Transforms;
using UnityEngine;

namespace Patrik
{
    public partial class SwordEntityFollowAnimationSystem : SystemBase
    {
        protected override void OnUpdate()
        {
            // foreach (var (transform, sword) in 
            //     SystemAPI.Query<RefRW<LocalTransform>, SwordComponent>())
            // {
            //     Debug.Log("Follow");
            //     
            //     var swordInstance = SwordBehaviour.Instance;
            //     if (swordInstance)
            //     {
            //         transform.ValueRW.Position = swordInstance.transform.position;
            //         transform.ValueRW.Rotation = swordInstance.transform.rotation;
            //
            //     }
            //     
            //     
            // }
        }
    }
}