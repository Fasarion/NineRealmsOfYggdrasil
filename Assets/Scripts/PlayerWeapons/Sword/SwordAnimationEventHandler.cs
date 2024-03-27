using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwordAnimationEventHandler : MonoBehaviour
{
    public event Action onImpact, onRetract, onSwing, onAnimationStart, onAnimationEnd, onAnimationInterval;
    public void Impact()
    {
        onImpact?.Invoke();
    }

    public void Retract()
    {
        onRetract?.Invoke();
    }

    public void Swing()
    {
        onSwing?.Invoke();
    }

    public void AnimationStart()
    {
        onAnimationStart?.Invoke();
    }

    public void AnimationEnd()
    {
        onAnimationEnd?.Invoke();
    }

    public void AnimationInterval()
    {
        onAnimationInterval?.Invoke();
    }
}
