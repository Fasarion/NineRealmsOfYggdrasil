using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallView : BounceElement
{
    private void OnCollisionEnter()
    {
        app.Notify(BounceNotification.BallHitGround,this);
    }
    
}
