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
    public Contents Contents;
    
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (Contents.OnEnter.Begin) PlayerWeaponManagerBehaviour.Instance.Begin(Contents.combo);
        if (Contents.OnEnter.Stop) PlayerWeaponManagerBehaviour.Instance.Stop(Contents.combo);
        if (Contents.OnEnter.TurnOff) PlayerWeaponManagerBehaviour.Instance.TurnOff();
    }

    

    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (Contents.OnExit.Begin) PlayerWeaponManagerBehaviour.Instance.Begin(Contents.combo);
        if (Contents.OnExit.Stop) PlayerWeaponManagerBehaviour.Instance.Stop(Contents.combo);
        if (Contents.OnExit.TurnOff) PlayerWeaponManagerBehaviour.Instance.TurnOff();
    }
    
   // HandleStateActions(Co)
}
