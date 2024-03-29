using System.Collections;
using System.Collections.Generic;
using System.Linq;
using KKD;
using QS.Data.Save;
using QS.Elements;
using QS.Enumerations;
using QS.Utilities;
using QS.Windows;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

namespace QS.Elements
{
    public class QSConditionNode : QSNode
    {
        public List<QSQuestHandlerNode> questHandlerNodes;
        public QSQuestHandlerNode currentQuestHandlerNode;
        public QuestHandler questHandler;

        public override void Initialize(string nodeName, QSGraphView qsGraphView, Vector2 position)
        {
            base.Initialize(nodeName, qsGraphView, position);
            questHandlerNodes = new List<QSQuestHandlerNode>();
            QuestNodeType = QSQuestNodeType.Condition;

            QSBranchSaveData branchSaveData = new QSBranchSaveData()
            {
                Text = "NextNode"
            };
            
            branches.Add(branchSaveData);
            
            

            
            graphView.questHandlerAdded += OnQuestHandlerAdded;
            graphView.questHandlerRemoved += OnQuestHandlerRemoved;
            if(graphView.nodes.OfType<QSQuestHandlerNode>().Any())
            {
                graphView.SendConditionNodeAddedEvent(this);
            }
            //qsGraphView.SendConditionHandlerAddedEvent(this);
            //What do about that?
            //It needs to know there's a QuestHandler in the graph and get its node upon its creation. 



        }

        public override void OnDestroy(QSGraphView qsGraphView)
        {
            graphView.questHandlerAdded -= OnQuestHandlerAdded;
            graphView.questHandlerRemoved -= OnQuestHandlerRemoved;
        }

        private void OnQuestHandlerRemoved(QSQuestHandlerNode questHandlerNode)
        {
            questHandlerNodes.Remove(questHandlerNode);
            if (questHandlerNode == currentQuestHandlerNode)
            {
                currentQuestHandlerNode = null;
            }

            if (questHandlerNode.questHandler == questHandler)
            {
                questHandler = null;
            }

            if (questHandlerNodes.Count > 0)
            {
                currentQuestHandlerNode = questHandlerNodes.First();
                questHandler = currentQuestHandlerNode.questHandler;
            }
            RedrawLabels();
        }

        public void OnQuestHandlerAdded(QSQuestHandlerNode questHandlerNode)
        {
            //It feels like it would be easier to just restrict the questHandler node in an earlier stage.
            //If the list of nodes doesn't contain a node that is about to be added, how do we know which node is supposed to be the one we're going to be using?
            if (!questHandlerNodes.Contains(questHandlerNode))
            {
                if (questHandlerNodes.Count > 0)
                {
                    //Debug.Log("Warning, can't add another quest handler node!");
                }
                //else
                //{
                    questHandlerNodes.Add(questHandlerNode);
                    currentQuestHandlerNode = questHandlerNodes.First();
                    questHandler = currentQuestHandlerNode.questHandler;
                //}
            }
            else
            {
                if (questHandlerNode.questHandler == null)
                {
                    questHandler = null;
                }
                else
                {
                    questHandler = questHandlerNode.questHandler;
                }
                //Maybe we can just redraw when the function is called
                //Debug.Log("A quest handler was updated");
            }
            
            //Draw();
            RedrawLabels();

           
        }

        public void RedrawLabels()
        {
           
            
            if (customDataContainer != null)
            {
                customDataContainer.Clear();
                Label label;
                if (questHandlerNodes.Count > 1)
                {
                    label =
                        new Label(
                            "Multiple quest handler nodes! Unable to determine which node to use for the condition."
                        );
                    customDataContainer.Add(label);
                    return;
                }

            
                if (currentQuestHandlerNode == null)
                {
                    label =
                        new Label(
                            "There is no quest handler node in the graph. Add one to see condition here."
                        );
                    customDataContainer.Add(label);
                    return;
                }

                if (questHandler == null)
                {
                    label =
                        new Label(
                            "No quest handler exists on the quest handler node. Add a quest handler node to see the quest condition on this node."
                        );
                    customDataContainer.Add(label);
                    return;
                }
            
                label = 
                    new Label(
                        "Value quest type is: " + questHandler.questType +  "."
                    ); 
                customDataContainer.Add(label);
            }
            
        }

        public override void Draw()
        {
            base.Draw();
            foreach (var branch in branches)
            {
                Port choicePort = this.CreatePort(branch.Text);
                choicePort.userData = branch;
                outputContainer.Add(choicePort);
            }

            RedrawLabels();
            RefreshExpandedState();
            //inputContainer.Clear();
            //outputContainer.Clear();
           

            //Debug.Log("A quest handler exists");

            //How do I get another node from the graph?
            //Check if a quest handler node exists.
            //If the graph is updated, redraw all nodes of this type to verify that a Quest handler node has been added and get a reference to its questHandler.
            //Then use the condition on the quest handler for this node.

            //Maybe I could make an event or callback on the questnode that this node can listen to.


            //Get the questhandler node and access its 
            //Just add the condition to wait for here based on the 
        }
    }
}
