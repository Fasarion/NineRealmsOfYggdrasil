using System.Collections;
using System.Collections.Generic;
using KKD;
using UnityEngine;

public class QuestInternalGraphBehaviour : MonoBehaviour
{
    public QuestHandler handler;
    private List<InternalQuestNode> allQuestNodes;

    private InternalDialogueGraphNode currentInternalDialogueGraph; 
    // Start is called before the first frame update
    void Awake()
    {
        
    }


    //For use when converting graph nodes into a representation that's usable at runtime.
    public void RetrieveQuestNodesFromGraph()
    {
        
    }

    public void ReceivePlayerBranchChoice(DialogueContainer container)
    {
        currentInternalDialogueGraph.PickPlayerChosenQuestBranch(container);
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
