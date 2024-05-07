using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

public class BounceController : BounceElement
{

    public void OnNotification(string p_event_path, Object p_target, params object[] p_data)
    {
        switch (p_event_path)
        {
            case BounceNotification.BallHitGround:
            {
                app.model.bounces++;
                Debug.Log("Bounce" + app.model.bounces);
                if (app.model.bounces >= app.model.winCondition)
                {
                    app.view.ball.enabled = false;
                    app.view.ball.GetComponent<Rigidbody>().isKinematic = true;
                    app.Notify(BounceNotification.GameComplete, this);
                }
                break;
            }
            case BounceNotification.GameComplete:
            {
                Debug.Log("Victory!");
                break;
            }
        }
    }
    public void OnBallGroundHit()
    {
        

        if (app.model.bounces >= app.model.winCondition)
        {
            app.view.ball.enabled = false;
            app.view.ball.GetComponent<Rigidbody>().isKinematic = true;
          
        }
    }

  
}
