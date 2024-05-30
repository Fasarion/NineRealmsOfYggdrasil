using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

public class PlayerDashConfigAuthoring : MonoBehaviour
{
  //  public float dashForce;
  //  public float dashDuration;
    public float dashCooldown;
    public AudioData Audio; 
    public GameObject DashShieldPrefab;
    public int maxDashes = 1;

    [HideInInspector] public bool isDashing;
    [HideInInspector] public bool isDashOnCooldown;

    private void OnValidate()
    {
        if (maxDashes < 0)
            maxDashes = 0;
    }

    public class PlayerDashConfigAuthoringBaker : Baker<PlayerDashConfigAuthoring>
    {
        public override void Bake(PlayerDashConfigAuthoring authoring)
        {
            var entity = GetEntity(TransformUsageFlags.Dynamic);
            AddComponent(entity,
                new PlayerDashConfig
                {
                 //   DashForce = authoring.dashForce,
                 //   DashDuration = authoring.dashDuration,
                    DashCooldown = authoring.dashCooldown,
                    IsDashing = authoring.isDashing,
                    IsDashOnCooldown = authoring.isDashOnCooldown,
                    Audio = authoring.Audio,
                    DashShieldPrefab = GetEntity(authoring.DashShieldPrefab, TransformUsageFlags.Dynamic),
                    MaxDashes = authoring.maxDashes
                });

            var dashBuffer = AddBuffer<DashInfoElement>(entity);
            for (int i = 0; i < authoring.maxDashes; i++)
            {
                dashBuffer.Add(new DashInfoElement
                {
                    Value = new DashInfo
                    {
                        CooldownTime = authoring.dashCooldown,
                        CurrentTime = authoring.dashCooldown,
                    }
                });
            }
        }
    }
}

public struct PlayerDashConfig : IComponentData
{
  //  public float DashForce;
  //  public float DashDuration;
    public float DashCooldown;
    public bool IsDashing;
    public bool IsDashOnCooldown;
    public AudioData Audio;
    public Entity DashShieldPrefab;
    public int MaxDashes;
}

public struct DashInfoElement : IBufferElementData
{
    public DashInfo Value;
}

public struct DashInfo
{
    public float CurrentTime;
    public float CooldownTime;
    public bool Ready => CurrentTime >= CooldownTime;
}
