using System.Collections;
using System.Collections.Generic;
using Damage;
using Unity.Entities;
using Unity.Transforms;
using UnityEngine;
using UnityEngine.Pool;

[UpdateAfter(typeof(ApplyDamageSystem))]
[UpdateBefore(typeof(TransformSystemGroup))]
public partial class DamageNumberSystem : SystemBase
{
    private ObjectPool<DamagePopup> _pool;
    
    protected override void OnUpdate()
    {
        //initialize config
        if (_pool == null)
        {
            _pool = GameObject.FindObjectOfType<DamageNumberPool>().Pool;

            if (_pool == null)
            {
                // No spawner exists
                return;
            }
        }

        var buffer = SystemAPI.GetSingletonBuffer<DamageNumberBufferElement>();

        if (buffer.IsEmpty)
        {
            return;
        }

        foreach (var element in buffer)
        {
            var damageNumber = _pool.Get();
            damageNumber.Setup((int)element.damage, element.position, element.isCritical);
        }


        buffer.Clear();
    }
}
