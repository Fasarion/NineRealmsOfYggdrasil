using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomNode
{
    public RoomNode previousNode;
    public int nodeLevel;
    public List<RoomNode> childNodes;

    public RoomNode(RoomNode previousNode, int nodeLevel)
    {
        childNodes = new List<RoomNode>();
        this.previousNode = previousNode;
        this.nodeLevel = nodeLevel;
    }
}
