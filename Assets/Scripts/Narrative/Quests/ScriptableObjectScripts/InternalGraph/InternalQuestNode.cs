using System.Collections;
using System.Collections.Generic;
using KKD;
using UnityEngine;

public class InternalQuestNode
{
    private QuestHandler handler;
    private InternalQuestNode nextNode;

    public InternalQuestNode(QuestHandler questHandler, InternalQuestNode nextQuestNode)
    {
        nextNode = nextQuestNode;
        handler = questHandler;
    }

}
