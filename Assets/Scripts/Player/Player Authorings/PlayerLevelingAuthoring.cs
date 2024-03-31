using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

public class PlayerLevelingAuthoring : MonoBehaviour
{
    [Header("--Debug Values--")]
    [Tooltip("Starting XP for the player in a scene. Only change for testing purposes!")]
    public int playerStartingXP = 0;
    [Tooltip("Starting level for the player in a scene. Only change for testing purposes!")]
    public int playerStartingLevel = 0;
    
    //TODO: level scaling, etc

    public class PlayerLevelingAuthoringBaker : Baker<PlayerLevelingAuthoring>
    {
        public override void Bake(PlayerLevelingAuthoring authoring)
        {
            var entity = GetEntity(TransformUsageFlags.None);
            AddComponent(entity,
                new PlayerLevelingConfig
                    {
                        PlayerStartingXp = authoring.playerStartingXP,
                        PlayerStartingLevel = authoring.playerStartingLevel
                    });
            AddComponent(entity, new PlayerXP
            {
                Value = authoring.playerStartingXP
            });
            AddComponent(entity, new PlayerLevel
            {
                Value = authoring.playerStartingLevel
            });
            
        }
    }
}

public struct PlayerLevelingConfig : IComponentData
{
    public int PlayerStartingXp;
    public int PlayerStartingLevel;
}

public struct PlayerXP : IComponentData
{
    public int Value;
}

public struct PlayerLevel : IComponentData
{
    public int Value;
}
