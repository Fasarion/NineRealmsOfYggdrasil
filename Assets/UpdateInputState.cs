using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UpdateInputState : StateMachineBehaviour
{
    public InputUpdateInformation OnEnter;
    public InputUpdateInformation OnExit;

    public enum InputState
    {
        None,
        Enable,
        Disable
    }

    [Serializable]
    public struct InputUpdateInformation
    {
        public InputState MovementStateChange;
        public InputState RotationStateChange;
    }
    
    
    
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        HandleStateChange(OnEnter);
    }
    

    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        HandleStateChange(OnExit);
    }

    void HandleStateChange(InputUpdateInformation information)
    {
        switch (information.MovementStateChange)
        {
            case InputState.Disable:
                EventManager.OnEnableMovementInput?.Invoke(false);
                break;
            
            case InputState.Enable:
                EventManager.OnEnableMovementInput?.Invoke(true);
                break;
        }
        
        switch (information.RotationStateChange)
        {
            case InputState.Disable:
                EventManager.OnEnableRotationInput?.Invoke(false);
                break;
            
            case InputState.Enable:
                EventManager.OnEnableRotationInput?.Invoke(true);
                break;
        }
    }
}
