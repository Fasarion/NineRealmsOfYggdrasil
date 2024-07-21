using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetCanDash : StateMachineBehaviour
{
    [System.Serializable]
    public struct StateType
    {
        public bool TurnOn;
        public bool TurnOff;
    }
    
    public StateType OnEnter;
    public StateType OnExit;
    
    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (OnEnter.TurnOn) EventManager.OnCanDash?.Invoke(true);
        if (OnEnter.TurnOff) EventManager.OnCanDash?.Invoke(false);
    }

    // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (OnEnter.TurnOn) EventManager.OnCanDash?.Invoke(true);
        if (OnEnter.TurnOff) EventManager.OnCanDash?.Invoke(false);
    }
}
