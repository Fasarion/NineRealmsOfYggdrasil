using Unity.Entities;
using Unity.Transforms;

namespace Patrik
{
    public partial class SwordEntityFollowAnimationSystem : SystemBase
    {
        protected override void OnUpdate()
        {
            foreach (var (transform, sword) in 
                SystemAPI.Query<RefRW<LocalTransform>, SwordComponent>())
            {
                transform.ValueRW.Position = SwordBehaviour.Instance.Tip.position;
            }
        }
    }
}