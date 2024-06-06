using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;
using UnityEngine.Serialization;

public class UpgradeChoiceAuthoring : MonoBehaviour
{
    // [HideInInspector] public int choiceIndex;
    // [HideInInspector] public bool isHandled = true;

    public class UpgradeChoiceAuthoringBaker : Baker<UpgradeChoiceAuthoring>
    {
        public override void Bake(UpgradeChoiceAuthoring authoring)
        {
            var entity = GetEntity(TransformUsageFlags.Dynamic);
            AddComponent(entity,
                new UpgradeChoice { ChoiceIndex = 0, IsHandled = true });
        }
    }
}

public struct UpgradeChoice : IComponentData
{
    public int ChoiceIndex;
    public bool IsHandled;
}
