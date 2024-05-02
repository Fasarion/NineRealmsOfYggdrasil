using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

public class PlayerLevelingAuthoring : MonoBehaviour
{
    [Header("--Tweakable Values--")] [Tooltip("Required xp for the initial level up, from level 0 -> 1")]
    public int baseXPNeeded = 5;

    [Tooltip(
        "The added amount of xp needed per level. Example if baseXPNeeded is = 5: level 1 requires 25 xp, level 2 requires 65 xp, level 3 105 xp, and so on...")]
    public int addedXPNeededPerLevel = 20;
    
    
    
    [Header("--Debug Values--")]
    [Tooltip("Starting XP for the player in a scene. Only change for testing purposes!")]
    public int playerStartingXP = 0;
    [Tooltip("Starting level for the player in a scene. Only change for testing purposes!")]
    public int playerStartingLevel = 0;
    [Tooltip("Starting amount of available skillpoints. Only change for testing purposes!")]
    public int playerStartingSkillpoints = 0;

    public class PlayerLevelingAuthoringBaker : Baker<PlayerLevelingAuthoring>
    {
        public override void Bake(PlayerLevelingAuthoring authoring)
        {
            var entity = GetEntity(TransformUsageFlags.None);
            AddComponent(entity,
                new PlayerLevelingConfig
                    {
                        PlayerStartingXp = authoring.playerStartingXP,
                        PlayerStartingLevel = authoring.playerStartingLevel,
                        BaseXPNeeded = authoring.baseXPNeeded,
                        AddedXPNeededPerLevel = authoring.addedXPNeededPerLevel,
                        PlayerStartingSkillpoints = authoring.playerStartingSkillpoints
                    });
            AddComponent(entity, new PlayerXP
            {
                XPValue = authoring.playerStartingXP
            });
            AddComponent(entity, new PlayerLevel
            {
                Value = authoring.playerStartingLevel
            });
            AddComponent(entity, new PlayerSkillpoints
            {
                Value = authoring.playerStartingSkillpoints
            });

        }
    }
}

public struct PlayerLevelingConfig : IComponentData
{
    public int PlayerStartingXp;
    public int PlayerStartingLevel;
    public int BaseXPNeeded;
    public int AddedXPNeededPerLevel;
    public int PlayerStartingSkillpoints;
}

public struct PlayerXP : IComponentData
{
    public int XPValue;
    public int XPNeededToLevelUp;
}

public struct PlayerLevel : IComponentData
{
    public int Value;
}

public struct PlayerSkillpoints : IComponentData
{
    public int Value;
}
