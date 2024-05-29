using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;
using UnityEngine.PlayerLoop;

public partial struct SlowTimeApplierSystem : ISystem
{
    private bool _isFadingIn;
    private bool _isFadingOut;
    private float _tValue;
    private bool _isSlowingTime;
    private float _cachedTimeStamp;
    
    
    public void OnUpdate(ref SystemState state)
    {
        //var config = SystemAPI.GetSingletonRW<SlowTimeSingleton>();
        SystemAPI.TryGetSingletonRW(out RefRW<SlowTimeSingleton> config);
        if (!config.IsValid)
        {
            return;
        }
        if (!config.ValueRO.ShouldSlowTime && !_isSlowingTime) return;

        if (!_isSlowingTime)
        {
            _isFadingIn = true;
            _isSlowingTime = true;
            _cachedTimeStamp = Time.unscaledTime;
        }

        if (_isFadingIn)
        {
            _tValue += Time.unscaledTime - _cachedTimeStamp;
            Time.timeScale = Mathf.Lerp(1, config.ValueRO.SlowTargetDuration, _tValue);
        }
        
    }
}
