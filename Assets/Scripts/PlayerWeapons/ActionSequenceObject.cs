using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

[CreateAssetMenu]
public class ActionSequenceObject : ScriptableObject
{
    public bool isOverride;
    
    public bool stickyWindow;

    public List<ModifierActionData> Actions = new List<ModifierActionData>();

    [FormerlySerializedAs("Sequence")] public List<ModifierAction> sequence = new List<ModifierAction>();
    
    
    //TODO: Only add active enum index
    public void GenerateSequence()
    {
        sequence.Clear();
        
        foreach (var action in Actions)
        {
            switch (action.Type)
            {
                case ActionType.VfxAction:
                    //action.vfxAction.delay = action.Delay;
                    //VfxAction vfxInstance = new VfxAction();
                    //vfxInstance = action.vfxAction;
                    //sequence.Add(vfxInstance);
                    break;
                
                case ActionType.SoundAction:
                    //action.soundAction.delay = action.Delay;
                    //SoundAction soundInstance = new SoundAction();
                    //soundInstance = action.soundAction;
                    //sequence.Add(soundInstance);
                    break;

                case ActionType.TriggerAction:
                    /*action.triggerAction.delay = action.Delay;
                    TriggerAction triggerInstance = new TriggerAction();
                    triggerInstance = action.triggerAction;
                    sequence.Add(triggerInstance);*/
                    break;

                case ActionType.ScreenshakeAction:
                    /*action.screenshakeAction.delay = action.Delay;
                    ScreenshakeAction screenshakeInstance = new ScreenshakeAction();
                    screenshakeInstance = action.screenshakeAction;
                    sequence.Add(screenshakeInstance);*/
                    break;

            }
        }
    }

    public void SetUpSequence(Weapon weapon)
    {
        Debug.Log("setup sequence");
        foreach (var action in sequence)
        {
            action.SetUpAction(weapon);
        }
    }

    public IEnumerator<float> ExecuteSequence()
    {
        for (int i = 0; i < sequence.Count; i++)
        {
            sequence[i].ExecuteAction();
            yield return 0;
        }
    }
}
