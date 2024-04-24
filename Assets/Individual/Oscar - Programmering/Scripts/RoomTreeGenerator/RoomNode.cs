using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class RoomNode
{
    //Having both child and parent nodes stored causes circular referencing leading to memory leaks.
    //Don't use the childNode and parentNode lists, use the childNode and parentNode coordinate lists
    //as keys in the grid map dictionary.
    //I want to refactor this one day.

    [NonSerialized]public List<RoomNode> childNodes;
    public Vector2Int roomCoordinates;
    [NonSerialized]public List<RoomNode> parentNodes;
    public int chosenRoomTypeIndex;

    public List<Vector2Int> childNodeCoordinates;
    public List<Vector2Int> parentNodeCoordinates;
    public RoomNode(/*RoomNode previousNode,*/ Vector2Int roomCoordinates)
    {
        childNodes = new List<RoomNode>();
        parentNodes = new List<RoomNode>();
        childNodeCoordinates = new List<Vector2Int>();
        parentNodeCoordinates = new List<Vector2Int>();
        //this.previousNode = previousNode;
        this.roomCoordinates = roomCoordinates;
    }
}
