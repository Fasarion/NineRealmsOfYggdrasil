using System.Collections;
using System.Collections.Generic;
using Patrik;
using UnityEngine;

[System.Serializable]
public struct EventFunctions
{
    [Header("Weapon Events")]
    public bool Begin;
    public bool Stop;
    public bool TurnOff;

    [Header("Movement Events")] 
    public MovementInputState movementInputState;

    public enum MovementInputState
    {
        None,
        EnableMovementInput,
        DisableMovementInput
    }
}

[System.Serializable]
public struct Contents
{
    public int combo;

    public EventFunctions OnEnter;
    public EventFunctions OnExit;
}

public class AnimatorAttackController : StateMachineBehaviour
{
    public Contents Contents;
    
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        HandleStateActions(Contents.OnEnter);
    }
    
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
       HandleStateActions(Contents.OnExit);
    }

    void HandleStateActions(EventFunctions eventFunctions)
    {
        // TODO: turn off player controller, lägg till tag
        
        if (eventFunctions.Begin)
        {
            PlayerWeaponManagerBehaviour.Instance.Begin(Contents.combo);
        }

        switch (eventFunctions.movementInputState)
        {
            case EventFunctions.MovementInputState.DisableMovementInput:
                EventManager.OnEnableMovementInput?.Invoke(false);
                break;
            
            case EventFunctions.MovementInputState.EnableMovementInput:
                EventManager.OnEnableMovementInput?.Invoke(true);
                break;
        }

        if (eventFunctions.Stop)
        {
            PlayerWeaponManagerBehaviour.Instance.Stop(Contents.combo);
        }

        if (eventFunctions.TurnOff)
        {
            PlayerWeaponManagerBehaviour.Instance.TurnOff();
        }
    }
}
