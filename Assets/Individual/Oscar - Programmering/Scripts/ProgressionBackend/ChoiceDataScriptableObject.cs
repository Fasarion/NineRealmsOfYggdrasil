using System;
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
    public SerializableDictionary<Vector2Int, RoomNode> roomNodeGridMapSO;

    public List<Vector2Int> roomNodeCoordinates;
    
    public void Awake()
    {
        
    }

    //public List<int> testList;
    public void ClearCachedData()
    {
        roomNodeGridMapSO = null;
        currentRoomNode = null;
        //keys = {null;
    }

    public RoomNode GetNodeFromGridMap(Vector2Int keyRoomNodeCoordinates)
    {
        roomNodeGridMapSO.TryGetValue(keyRoomNodeCoordinates, out var value);
        return value;
    }
}
