using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

public class BounceController : BaseControllerMVC
{

    public override void OnNotification(string p_event_path, Object p_target, params object[] p_data)
    {
        /*switch (p_event_path)
        {
            case BounceNotification.BallHitGround:
            {
                app.model.bounceModel.bounces++;
                Debug.Log("Bounce" + app.model.bounceModel.bounces);
                if (app.model.bounceModel.bounces >= app.model.bounceModel.winCondition)
                {
                    app.model.bounceModel.enabled = false;
                    app.view.ballView.GetComponent<Rigidbody>().isKinematic = true;
                    app.Notify(BounceNotification.GameComplete, this);
                }
                break;
            }
            case BounceNotification.GameComplete:
            {
                Debug.Log("Victory!");
                break;
            }
        }*/
    }
    public void OnBallGroundHit()
    {
        /*if (app.model.bounceModel.bounces >= app.model.bounceModel.winCondition)
        {
            app.view.ballView.enabled = false;
            app.view.ballView.GetComponent<Rigidbody>().isKinematic = true;
          
        }*/
    }

  
}
