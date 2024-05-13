using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum StateToSwapTo
{
    WalkLeft,
    WalkRight
}

public class MainSM : MonoBehaviour
{
    private SMState currentState;
    public void Start()
    {
        currentState = new SMState(this);
        currentState.Enter();
    }

    public void Update()
    {
        currentState.UpdateState();
    }

    public void SwapState(StateToSwapTo newState)
    {
        
        
        currentState.Exit();
        switch (newState)
        {
            case StateToSwapTo.WalkLeft:
            {
                //currentState = walkLeftState
                break;
            }
            case StateToSwapTo.WalkRight:
            {
                //currentState = walkRightState
                break;
            }
        }
        currentState.Enter();
    }
}
