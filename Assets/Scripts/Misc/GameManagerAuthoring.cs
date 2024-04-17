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
                GameState = GameState.Combat,
                CombatState = CombatState.Normal
            });
        }
    }
}

public partial struct GameManagerSingleton : IComponentData
{
    public GameState GameState;
    public CombatState CombatState;
}

public enum GameState
{
    None,
    Combat,
}

public enum CombatState
{
    None,
    Normal,
    ActivatingUltimate
}
