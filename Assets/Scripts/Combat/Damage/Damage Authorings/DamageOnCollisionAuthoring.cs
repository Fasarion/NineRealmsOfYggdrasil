using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

namespace Damage
{
   public class DamageOnCollisionAuthoring : MonoBehaviour
   {
      [SerializeField] private float damageAmount;

      class Baker : Baker<DamageOnCollisionAuthoring>
      {
         public override void Bake(DamageOnCollisionAuthoring authoring)
         {
            var entity = GetEntity(TransformUsageFlags.Dynamic);
            AddComponent(entity, new DamageOnCollisionComponent
            {
               Amount = authoring.damageAmount
            });
         }
      }
   }
}