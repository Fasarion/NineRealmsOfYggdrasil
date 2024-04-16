using System.Collections;
using System.Collections.Generic;
using DevLocker.Utils;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;


[CreateAssetMenu(fileName = "RoomChoiceObject", menuName = "ChoiceUI/ChoiceObjects/RoomChoiceObject" +
                                                           "")]
public class RoomChoiceObject : ChoiceObject
{
    [Tooltip("Reference to the scene being sent to")]
    public SceneReference roomSceneReference;
}
