using Movement;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;
using Weapon;

[DisableAutoCreation]
public partial struct ResetProjectileBufferSystem : ISystem
{
    public void OnUpdate(ref SystemState state)
    {
        var entityManager = state.EntityManager;

        // Spawn projectile
        foreach (var (projectileBuffer, entity)
            in SystemAPI.Query<DynamicBuffer<ProjectileBufferElement>>()
                .WithEntityAccess())
        {
            for (int i = projectileBuffer.Length - 1; i >= 0; i--)
            {
                var projectileElement = projectileBuffer.ElementAt(i);
                if (projectileElement.Projectile == null)
                {
                    Debug.Log("Invalid"); 
                }
            }
        }
    }
}