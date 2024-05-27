using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UpdateInputState : StateMachineBehaviour
{
    [Tooltip("How the input state is updated when entering this animation.")]
    public InputUpdateInformation OnEnter;
    
    [Tooltip("How the input state is updated when exiting this animation.")]
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
        [Tooltip("Determines whether or not the player can move (from input) during this attack.")]
        public InputState MovementStateChange;
        
        [Tooltip("Determines whether or not the player can rotate (from input) during this attack.")]
        public InputState RotationStateChange;
        
        [Tooltip("Determines whether or not the player can take damage.")]
        public InputState InvincibilityStateChange;
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
        
        switch (information.InvincibilityStateChange)
        {
            case InputState.Enable:
                EventManager.OnEnablePlayerInvincibility?.Invoke(true);
                break;
            
            case InputState.Disable:
                EventManager.OnEnablePlayerInvincibility?.Invoke(false);
                break;
        }
    }
}
