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
    private bool _isInitialized;
    private float startUpTimer;
    
    protected override void OnUpdate()
    {
        if (!_isInitialized)
        {
            if (startUpTimer < 1)
            {
                startUpTimer += SystemAPI.Time.DeltaTime;
                return;
            }
            
            _isInitialized = true;
            return;
        }
        
        
        //initialize config
        if (_pool == null)
        {
            var poolObject = GameObject.FindObjectOfType<DamageNumberPool>();
            var poolObjects = GameObject.FindObjectsOfType<DamageNumberPool>();
            if (!poolObject) return;
            
            _pool = poolObject.Pool;
        
            if (_pool == null)
            {
                // No spawner exists
                return;
            }
        }

        SystemAPI.TryGetSingletonBuffer(out DynamicBuffer<DamageNumberBufferElement> buffer);

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
