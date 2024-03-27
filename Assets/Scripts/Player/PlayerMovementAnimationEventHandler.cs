using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovementAnimationEventHandler : MonoBehaviour
{
    public event Action onFootstep;

    public void OnFootStep()
    {
        onFootstep?.Invoke();
    }
}
