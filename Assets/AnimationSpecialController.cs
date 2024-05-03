using System.Collections;
using System.Collections.Generic;
using Patrik;
using UnityEngine;

public class AnimationSpecialController : StateMachineBehaviour
{
    public int charge;

    public bool InitializeChargeOnStart;
    public bool EndChargeOnEnd;

    public bool SetOngoing;
    public bool SetNone;

    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (InitializeChargeOnStart)
        {
            PlayerWeaponManagerBehaviour.Instance.SetCharge(ChargeState.Start);
        }
        
        
    }

    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (SetOngoing)
        {
            PlayerWeaponManagerBehaviour.Instance.SetCharge(ChargeState.Ongoing);
        }
        
        if (SetNone)
        {
            PlayerWeaponManagerBehaviour.Instance.SetCharge(ChargeState.None);
        }
    }

    // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (EndChargeOnEnd)
        {
            PlayerWeaponManagerBehaviour.Instance.SetCharge(ChargeState.Stop);
        }
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
