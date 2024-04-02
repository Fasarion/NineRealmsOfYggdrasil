using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;
using UnityEngine.Serialization;

public class UpgradeChoiceAuthoring : MonoBehaviour
{
    [HideInInspector] public int choiceIndex;
    [HideInInspector] public bool isHandled = true;

    public class UpgradeChoiceAuthoringBaker : Baker<UpgradeChoiceAuthoring>
    {
        public override void Bake(UpgradeChoiceAuthoring authoring)
        {
            var entity = GetEntity(TransformUsageFlags.Dynamic);
            AddComponent(entity,
                new UpgradeChoice { ChoiceIndex = authoring.choiceIndex, IsHandled = authoring.isHandled });
        }
    }
}

public struct UpgradeChoice : IComponentData
{
    public int ChoiceIndex;
    public bool IsHandled;
}
