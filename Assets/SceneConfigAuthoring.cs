using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

public enum SceneType
{
    None,
    CombatScene
}

public class SceneConfigAuthoring : MonoBehaviour
{
    public SceneType CurrentScene = SceneType.CombatScene;
    
    class Baker : Baker<SceneConfigAuthoring>
    {
        public override void Bake(SceneConfigAuthoring authoring)
        {
            Entity entity = GetEntity(TransformUsageFlags.None);

            switch (authoring.CurrentScene)
            {
                case SceneType.CombatScene:
                    AddComponent(entity, new CombatSceneComponent());
                    break;
            }
        }
    }
}

public struct CombatSceneComponent : IComponentData{}
