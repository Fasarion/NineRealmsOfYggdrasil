using System.Collections;
using System.Collections.Generic;
using Patrik;
using UnityEngine;

[System.Serializable]
public struct EventFunctions
{
    public bool Begin;
    public bool Stop;
    public bool TurnOff;
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
    // public int combo;
    //
    // public EventFunctions OnEnter;
    // public EventFunctions OnExit;

    public Contents Contents;
    
    // public bool callStartOnStateEnter = true;
    // public bool callStopOnStateExit = true;
    // public bool callTurnOffOnStateExit = true;
    
    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (Contents.OnEnter.Begin) PlayerWeaponManagerBehaviour.Instance.Begin(Contents.combo);
        if (Contents.OnEnter.Stop) PlayerWeaponManagerBehaviour.Instance.Stop(Contents.combo);
        if (Contents.OnEnter.TurnOff) PlayerWeaponManagerBehaviour.Instance.TurnOff();

        
        // if (callStartOnStateEnter)
        // {
        //     PlayerWeaponManagerBehaviour.Instance.Begin(combo);
        // }
    }

    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    //override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    // {
    //     
    // }

    // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (Contents.OnExit.Begin) PlayerWeaponManagerBehaviour.Instance.Begin(Contents.combo);
        if (Contents.OnExit.Stop) PlayerWeaponManagerBehaviour.Instance.Stop(Contents.combo);
        if (Contents.OnExit.TurnOff) PlayerWeaponManagerBehaviour.Instance.TurnOff();
        
        //
        // if (callStopOnStateExit)
        // {
        //     PlayerWeaponManagerBehaviour.Instance.Stop(combo);
        // }
        //
        // if (callTurnOffOnStateExit)
        // {
        //     PlayerWeaponManagerBehaviour.Instance.TurnOff();
        // }
    }

    // OnStateMove is called right after Animator.OnAnimatorMove()
    //override public void OnStateMove(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    // Implement code that processes and affects root motion
    //}

    // OnStateIK is called right after Animator.OnAnimatorIK()
    //override public void OnStateIK(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    // Implement code that sets up animation IK (inverse kinematics)
    //}
}
