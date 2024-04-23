using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class RoomNode
{
    //Having both child and parent nodes stored causes circular referencing leading to memory leaks
    public List<RoomNode> childNodes;
    public Vector2Int roomCoordinates;
    [HideInInspector]public List<RoomNode> parentNodes;
    public int chosenRoomTypeIndex;
    public RoomNode(/*RoomNode previousNode,*/ Vector2Int roomCoordinates)
    {
        childNodes = new List<RoomNode>();
        parentNodes = new List<RoomNode>();
        //this.previousNode = previousNode;
        this.roomCoordinates = roomCoordinates;
    }
}
