using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MEC;

public class PlayerBroadcastPosition : MonoBehaviour
{
    private Vector3 playerPos;
    public void Awake()
    {
        playerPos = new Vector3(0, 0, 0);
    }

    public event Action<Vector3> positionBroadcast;
    

    public void OnEnable()
    {
        Timing.RunCoroutine(MoveTowardsPlayer());
    }


    private IEnumerator<float> MoveTowardsPlayer()
    {
        while (true)
        {
            playerPos = transform.position;
            positionBroadcast?.Invoke(playerPos);
            yield return Timing.WaitForSeconds(.1f);

        }
    }
}
