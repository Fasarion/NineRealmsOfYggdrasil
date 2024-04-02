using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

[UpdateAfter(typeof(UpgradeUISystem))]
public partial class UpgradeApplierSystem : SystemBase
{
    private UpgradePoolManager _pool;
    
    protected override void OnUpdate()
    {
        var choice = SystemAPI.GetSingletonRW<UpgradeChoice>();
        
        if (!choice.ValueRO.IsHandled)
        {
            if (_pool == null)
            {
                _pool = UpgradePoolManager.Instance;
            }

            UpgradeObject upgradeObject = _pool.GetUpgradeObjectReferenceByKey(choice.ValueRO.ChoiceIndex);

            choice.ValueRW.IsHandled = true;
        
            Debug.Log($"Upgrade chosen: {upgradeObject.upgradeTitle}");
        }
    }
}
