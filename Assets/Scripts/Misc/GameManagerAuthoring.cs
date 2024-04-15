using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

public class GameManagerAuthoring : MonoBehaviour
{
    class Baker : Baker<GameManagerAuthoring>
    {
        public override void Bake(GameManagerAuthoring authoring)
        {
            var entity = GetEntity(TransformUsageFlags.None);
            AddComponent(entity, new GameManagerSingleton
            {
                CanAttack = true
            });
        }
    }
}

public partial struct GameManagerSingleton : IComponentData
{
    public bool CanAttack;
}
