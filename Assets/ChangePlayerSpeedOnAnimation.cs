using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct MoveSpeedChangeData
{
    public float Factor;
    public float Duration;
}

public class ChangePlayerSpeedOnAnimation : StateMachineBehaviour 
{
    [Tooltip("Actions to be applied when entering this state.")]
    [SerializeField] private StateActions OnEnter;
    [Space]
    
    [Tooltip("Actions to be applied when exiting this state.")]
    [SerializeField] private StateActions OnExit;

    enum SpeedChangeOption
    {
        None,
        ChangeSpeed,
        ResetSpeed
    }
    
    [System.Serializable]
    struct StateActions
    {
        [Tooltip("How this state changes speed." +
                 "\n\nNone: Does nothing." +
                 "\n\nChangeSpeed: Changes the player speed with the values defined in the inspector below." +
                 "\n\nResetSpeed: Resets the player speed to the base value.")]
        public SpeedChangeOption SpeedChangeOption;
        [Tooltip("The factor to change the speed with (0.7 means 70% of current speed).")]
        public float SpeedChangeFactor;
        [Tooltip("How long the speed change should be applied. (If ResetSpeed is chosen on a state after this on, the duration will be cut short.")]
        public float SpeedChangeDuration;
    }

    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        HandleStateActions(OnEnter);
    }
    
    // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        HandleStateActions(OnExit);
    }

    private void HandleStateActions(StateActions stateActions)
    {
        switch (stateActions.SpeedChangeOption)
        {
            case SpeedChangeOption.ChangeSpeed:
                float speedChangeFactor = stateActions.SpeedChangeFactor > 0 ? stateActions.SpeedChangeFactor : 1;
            
                MoveSpeedChangeData data = new MoveSpeedChangeData
                {
                    Duration = stateActions.SpeedChangeDuration,
                    Factor = speedChangeFactor
                };

                EventManager.OnChangeMoveSpeed?.Invoke(data);
                break;
            
            case SpeedChangeOption.ResetSpeed:
                EventManager.OnResetMoveSpeed?.Invoke();
                break;
        }
    }
}
