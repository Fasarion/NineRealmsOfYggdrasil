using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ChoiceDataObject", menuName = "ChoiceData/ChoiceDataObject" +
                                                           "")]
public class ChoiceDataScriptableObject : ScriptableObject
{
    //public List<Vector2Int> keys;
    public SerializableDictionary<Vector2Int, RoomNode> roomNodeGridMapSO;
    public bool saveGeneratedGridMap = false;

    //public List<int> testList;
    public void ClearCachedGridMap()
    {
        roomNodeGridMapSO = null;
        //keys = null;
    }
}
