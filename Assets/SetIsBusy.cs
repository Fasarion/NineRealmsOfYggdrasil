using System.Collections;
using System.Collections.Generic;
using Patrik;
using UnityEngine;

public class SetIsBusy : StateMachineBehaviour
{
    [System.Serializable]
    public struct StateType
    {
        public bool TurnOn;
        public bool TurnOff;
    }

    public WeaponType WeaponType;
    public AttackType AttackType;

    public StateType OnEnter;
    public StateType OnExit;
    
    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (OnEnter.TurnOn) PlayerWeaponManagerBehaviour.Instance.SetBusy(AttackType, WeaponType);
        if (OnEnter.TurnOff) PlayerWeaponManagerBehaviour.Instance.SetNotBusy();
    }

    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    //override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    
    //}

    // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (OnExit.TurnOn) PlayerWeaponManagerBehaviour.Instance.SetBusy(AttackType, WeaponType);
        if (OnExit.TurnOff) PlayerWeaponManagerBehaviour.Instance.SetNotBusy();
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
