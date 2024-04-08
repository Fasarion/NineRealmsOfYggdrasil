using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


[CreateAssetMenu(fileName = "RoomChoiceObject", menuName = "RoomUI/RoomChoiceObject" +
                                                           "")]
public class RoomChoiceObject : ScriptableObject
{
    [Header("--Name--")] 
    [Tooltip("The name of the room in question")]
    public string roomName;

    public Scene scene;
}
