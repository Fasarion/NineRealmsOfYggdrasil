using System;
using System.Collections;
using System.Collections.Generic;
using DevLocker.Utils;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class RoomChoiceUIBehaviour : ChoiceUIBehaviour
{
    
    
    [SerializeField]private SceneReference roomSceneReference;

    [SerializeField] public RoomNode roomNode;


    public static Action<RoomNode, SceneReference> onRoomChanged;
    // Start is called before the first frame update

    protected override void PopulateDisplayValues(ChoiceObject newRoomChoiceObject)
    {
        RoomChoiceObject roomChoiceObject = (RoomChoiceObject)newRoomChoiceObject;
        roomSceneReference = roomChoiceObject.roomSceneReference;
        base.PopulateDisplayValues(roomChoiceObject);
        
    }
    
    public override void UpdateSelectionDisplay(ChoiceObject newRoomChoiceObject)
    {
        base.UpdateSelectionDisplay(newRoomChoiceObject);
        PopulateDisplayValues(newRoomChoiceObject);
    }
    
    public override void RegisterMouseClick()
    {
        onRoomChanged?.Invoke(roomNode, roomSceneReference);
        //manager.RegisterRoomSelectionClick(roomSceneReference);
    }
}
