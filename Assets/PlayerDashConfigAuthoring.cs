using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

public class PlayerDashConfigAuthoring : MonoBehaviour
{
    public float dashForce;
    public float dashDuration;
    public float dashCooldown;
    public AudioData Audio; 
    public GameObject DashShieldPrefab;  

    [HideInInspector] public bool isDashing;
    [HideInInspector] public bool isDashOnCooldown;

    public class PlayerDashConfigAuthoringBaker : Baker<PlayerDashConfigAuthoring>
    {
        public override void Bake(PlayerDashConfigAuthoring authoring)
        {
            var entity = GetEntity(TransformUsageFlags.Dynamic);
            AddComponent(entity,
                new PlayerDashConfig
                {
                    DashForce = authoring.dashForce,
                    DashDuration = authoring.dashDuration,
                    DashCooldown = authoring.dashCooldown,
                    IsDashing = authoring.isDashing,
                    IsDashOnCooldown = authoring.isDashOnCooldown,
                    Audio = authoring.Audio,
                    DashShieldPrefab = GetEntity(authoring.DashShieldPrefab, TransformUsageFlags.Dynamic)
                });
        }
    }
}

public struct PlayerDashConfig : IComponentData
{
    public float DashForce;
    public float DashDuration;
    public float DashCooldown;
    public bool IsDashing;
    public bool IsDashOnCooldown;
    public AudioData Audio;
    public Entity DashShieldPrefab;
}
