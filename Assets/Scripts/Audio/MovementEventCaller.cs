using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

[DisableAutoCreation]
public partial class MovementEventCaller : SystemBase
{
    private AudioManager _audioManager;
    private var isMoving;

    protected override void OnUpdate()
    {
        if (_audioManager == null)
        {
            _audioManager = AudioManager.Instance;
        }
        /*isMoving = SystemAPI.GetSingleton<PlayerMoveInput>();

        while (isMoving =! 0)
        {
            //TODO: Play Footsteps
        }*/
    }
}