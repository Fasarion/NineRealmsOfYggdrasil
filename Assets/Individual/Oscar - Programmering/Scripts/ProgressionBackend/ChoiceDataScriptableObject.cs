using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ChoiceDataObject", menuName = "ChoiceData/ChoiceDataObject" +
                                                           "")]
public class ChoiceDataScriptableObject : ScriptableObject
{
    //public List<Vector2Int> keys;
    public bool saveGeneratedGridMap = false;
    public bool resetNodeProgression = false;
    
    public RoomNode currentRoomNode;
    [HideInInspector] public SerializableDictionary<Vector2Int, RoomNode> roomNodeGridMapSO;
    //public List<int> testList;
    public void ClearCachedData()
    {
        roomNodeGridMapSO = null;
        currentRoomNode = null;
        //keys = null;
    }
}
