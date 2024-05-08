using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

public class BounceController : BaseControllerMVC
{

    public override void OnNotification(string p_event_path, Object p_target, params object[] p_data)
    {
        switch (p_event_path)
        {
            case BounceNotification.BallHitGround:
            {
                app.bounceModel.bounces++;
                Debug.Log("Bounce" + app.bounceModel.bounces);
                if (app.bounceModel.bounces >= app.bounceModel.winCondition)
                {
                    app.bounceView.ball.enabled = false;
                    app.bounceView.ball.GetComponent<Rigidbody>().isKinematic = true;
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
        

        if (app.bounceModel.bounces >= app.bounceModel.winCondition)
        {
            app.bounceView.ball.enabled = false;
            app.bounceView.ball.GetComponent<Rigidbody>().isKinematic = true;
          
        }
    }

  
}
