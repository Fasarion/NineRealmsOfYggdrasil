using System.Collections;
using System.Collections.Generic;
using DevLocker.Utils;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;


[CreateAssetMenu(fileName = "RoomChoiceObject", menuName = "RoomUI/RoomChoiceObject" +
                                                           "")]
public class RoomChoiceObject : ScriptableObject
{
    [Header("--Name--")] 
    [Tooltip("The name of the room in question")]
    public string roomName;

    //public SceneAsset scene;
 
    public SceneReference sceneReference;
}
