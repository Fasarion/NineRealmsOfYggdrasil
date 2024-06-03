using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

public class ShouldRecordSwordTrajectoryComponentAuthoring : MonoBehaviour
{
    public class
        ShouldRecordSwordTrajectoryComponentAuthoringBaker : Baker<ShouldRecordSwordTrajectoryComponentAuthoring>
    {
        public override void Bake(ShouldRecordSwordTrajectoryComponentAuthoring authoring)
        {
            var entity = GetEntity(TransformUsageFlags.Dynamic);
            AddComponent(entity, new ShouldRecordSwordTrajectoryComponent());
        }
    }
}

public struct ShouldRecordSwordTrajectoryComponent : IComponentData
{
}
