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
        bool playerXpExist = SystemAPI.TryGetSingleton<PlayerXP>(out PlayerXP xp);
        if (!playerXpExist)
        {
            //Debug.Log("No player xp found");
            return;
        }
        
        if (_audioManager == null)
        {
            _audioManager = AudioManager.Instance;
        }

        if (_cachedXP != xp.XPValue && _audioManager)
        {
            _cachedXP = xp.XPValue;
            _audioManager.playerAudio.XpAudio();
        }
    }
}
