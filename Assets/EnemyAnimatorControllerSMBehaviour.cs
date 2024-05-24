using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Analytics;

public class EnemyAnimatorControllerSMBehaviour : StateMachineBehaviour
{
    public enum EnemyAnimationBool
    {
        isAttacking,
        isKnockedBack,
        isStunned
    }

    public static Dictionary<EnemyAnimationBool, string> EnemyAnimationParameters
        = new Dictionary<EnemyAnimationBool, string>()
        {
            {EnemyAnimationBool.isAttacking, "isAttacking"},
            {EnemyAnimationBool.isKnockedBack, "isKnockedBack"},
            {EnemyAnimationBool.isStunned, "isStunned"},
        };

    [System.Serializable]
    public struct AnimationStateOptions
    {
        public bool turnOnBool;
        public EnemyAnimationBool boolToSwitch;
    }
    
    [System.Serializable]
    public struct AnimationStateOptionsAfterFrames
    {
        public int FramesBeforeExecution;
        public AnimationStateOptions Options;
    }
    
    public List<AnimationStateOptions> actionsOnStateEnter;
    public List<AnimationStateOptionsAfterFrames> actionsAfterFrames;
    public List<AnimationStateOptions> actionsOnStateExit;

    private int currentFrame;


    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        currentFrame = 0;
        
        OverrideActions(animator, actionsOnStateEnter);
    }
    
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        foreach (var action in actionsAfterFrames)
        {
            if (action.FramesBeforeExecution != currentFrame) continue;
            
            UpdateParamaterBool(animator, action.Options);
        }
        
        currentFrame++;
    }

    

    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        OverrideActions(animator, actionsOnStateExit);
    }
    
    private void OverrideActions(Animator animator, List<AnimationStateOptions> actions)
    {
        foreach (var action in actions)
        {
            UpdateParamaterBool(animator, action);
        }
    }

    private static void UpdateParamaterBool(Animator animator, AnimationStateOptions action)
    {
        animator.SetBool(EnemyAnimationParameters[action.boolToSwitch], action.turnOnBool);
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
