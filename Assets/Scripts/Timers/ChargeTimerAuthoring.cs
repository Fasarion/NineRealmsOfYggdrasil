using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

public class ChargeTimerAuthoring : MonoBehaviour
{
    public float maxChargeTime;
    public float currentChargeTime;

    public class ChargeTimerAuthoringBaker : Baker<ChargeTimerAuthoring>
    {
        public override void Bake(ChargeTimerAuthoring authoring)
        {
            var entity = GetEntity(TransformUsageFlags.Dynamic);
            AddComponent(entity,
                new ChargeTimer
                    {
                        maxChargeTime = authoring.maxChargeTime, currentChargeTime = authoring.currentChargeTime
                    });
        }
    }
}

public struct ChargeTimer : IComponentData
{
    public float maxChargeTime;
    public float currentChargeTime;
}
