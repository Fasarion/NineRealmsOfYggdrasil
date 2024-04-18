using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ChoiceDataObject", menuName = "ChoiceData/ChoiceDataObject" +
                                                           "")]
public class ChoiceDataScriptableObject : ScriptableObject
{
    public Dictionary<Vector2Int, RoomNode> roomNodeGridMap;
    public void ClearCachedGridMap()
    {
        roomNodeGridMap = null;
    }
}
