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

    [Tooltip("A description of the room in question")]
    public string roomDescription;
    //public SceneAsset scene;
    [Tooltip("Reference to the scene being sent to")]
    public SceneReference roomSceneReference;

    [Tooltip("Sprite showing what the room selected will be like")]
    public Sprite roomSprite;
}
