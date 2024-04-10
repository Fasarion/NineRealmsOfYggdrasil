using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomNode
{
    //public RoomNode previousNode;
    public List<RoomNode> childNodes;
    public Vector2Int roomCoordinates;
    public RoomNode parentNode;

    public RoomNode(/*RoomNode previousNode,*/ Vector2Int roomCoordinates)
    {
        childNodes = new List<RoomNode>();
        //this.previousNode = previousNode;
        this.roomCoordinates = roomCoordinates;
    }
}
