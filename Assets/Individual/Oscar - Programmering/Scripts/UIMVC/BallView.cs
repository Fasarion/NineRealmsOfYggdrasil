using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallView : ElementMVC
{
    private void OnCollisionEnter()
    {
        app.Notify(BounceNotification.BallHitGround,this);
    }
    
}
