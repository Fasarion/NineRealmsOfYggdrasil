using UnityEngine;
using System;
using System.Collections.Generic;
using UnityEngine.Serialization;


[Serializable]
public class ModifierActionData
{
    //[FormerlySerializedAs("vfxData")] public VfxAction vfxAction;
    //public SoundAction soundAction;
    //public TriggerAction triggerAction;
    //public ScreenshakeAction screenshakeAction;
    
    public int Index;
    public ActionType Type;
    public GameObject Prefab;
    public float Delay;
    public string NameTag;
}