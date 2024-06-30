using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class ResetStateMachineVariableBehaviour : StateMachineBehaviour
{
    [Header("State machine variable")]
    public string variableName = "hammerReturn";
    
    [Header("Time")]
    public bool resetAfterSeconds = true;
    public float secondsToReset = 4f;
    private float currentSeconds = 0f;

    [Header("Clicks")]
    public bool resetAfterClicks = true;
    public int clicksToReset = 3;
    [SerializeField] private int currentClicks = 0;

    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        ResetCurrentVariables();
    }

    private void ResetCurrentVariables()
    {
        currentClicks = 0;
        currentSeconds = 0f;
    }

    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (resetAfterSeconds)
        {
            currentSeconds += Time.deltaTime;
            if (currentSeconds >= secondsToReset)
            {
                ResetStateMachineVariable(animator);
            }
        }

        if (resetAfterClicks && (Input.GetMouseButtonDown(0) || Input.GetKeyDown(KeyCode.R)))
        {
            currentClicks++;
            if (currentClicks > clicksToReset)
            {
                ResetStateMachineVariable(animator);
            }
        }
    }

    private void ResetStateMachineVariable(Animator animator)
    {
        ResetCurrentVariables();
        
        animator.SetTrigger(variableName);
        animator.SetBool(variableName, true);
        
        //animator.SetFloat("attackSpeed", 0f);
    }
}
