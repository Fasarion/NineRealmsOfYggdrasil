using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SMState
{
    private MainSM mainSm;

    private float timer;
    private float timeBeforeStateSwitch;
    public SMState(MainSM mainSm)
    {
        this.mainSm = mainSm;
    }
    public void Enter()
    {
        timer = 0;
        timeBeforeStateSwitch = 5.0f;
        //Do entry stuffs
    }
    
    public void Exit()
    {
        //Do exit stuffs
    }
    
    public void UpdateState()
    {
        //Do stuffs for when the state is being updated.
        timer += Time.deltaTime;
        
        //Do stuffs for when some condition is fullfilled to switch states.
        if (timer >= timeBeforeStateSwitch)
        {
            mainSm.SwapState(StateToSwapTo.WalkLeft);
        }
    }
}
