using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

public partial class XPEventCaller : SystemBase
{
    private int _cachedXP;
    private AudioManager _audioManager;
    
    protected override void OnUpdate()
    {
        var xp = SystemAPI.GetSingleton<PlayerXP>();

        if (_audioManager == null)
        {
            _audioManager = AudioManager.Instance;
        }

        if (_cachedXP != xp.Value)
        {
            _cachedXP = xp.Value;
            _audioManager.weaponAudio.Test();
        }
    }
}
