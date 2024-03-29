using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MEC;


public enum ActionType { VfxAction, SoundAction, PositionAction, TriggerAction, MovementAction, ScreenshakeAction }
[System.Serializable]
public class ModifierAction
{
    public float delay;

    public virtual void ExecuteAction()
    {
        
    }
    
    public virtual IEnumerator<float> RunAction()
    {
        Debug.Log("execute");
        yield return Timing.WaitForSeconds(delay);
    }

    public virtual void SetUpAction(WeaponBehaviour weapon)
    {
        
    }
}
