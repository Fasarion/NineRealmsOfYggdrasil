using System;
using System.Collections;
using System.Collections.Generic;
using DS.Windows;
using QS.Elements;
using QS.Enumerations;
using QS.Windows;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

namespace QS.Windows
{
    public class QSSearchWindow : ScriptableObject, ISearchWindowProvider
    {
        private QSGraphView graphView;
        private Texture2D indentationIcon;

        public void Initialize(QSGraphView qsGraphView)
        {
            graphView = qsGraphView;
            indentationIcon = new Texture2D(1, 1);
            indentationIcon.SetPixel(0,0,Color.clear);
            indentationIcon.Apply();
            
        }

        public List<SearchTreeEntry> CreateSearchTree(SearchWindowContext context)
        {
            List<SearchTreeEntry> searchTreeEntries = new List<SearchTreeEntry>()
            {
                new SearchTreeGroupEntry(new GUIContent("Create Element"), 0),
                new SearchTreeGroupEntry(new GUIContent("Nodes"), 1),
                new SearchTreeEntry(new GUIContent("Quest Handler", indentationIcon))
                {
                    level = 2,
                    userData = QSQuestNodeType.QuestHandler
                },
                new SearchTreeEntry(new GUIContent("Dialogue Graph", indentationIcon))
                {
                    level = 2,
                    userData = QSQuestNodeType.DialogueGraph
                },
                new SearchTreeEntry(new GUIContent("Activator", indentationIcon))
                {
                    level = 2,
                    userData = QSQuestNodeType.Activator
                },
                new SearchTreeEntry(new GUIContent("Condition", indentationIcon))
                {
                    level = 2,
                    userData = QSQuestNodeType.Condition
                },
                new SearchTreeEntry(new GUIContent("QuestAccepted", indentationIcon))
                {
                    level = 2,
                    userData = QSQuestNodeType.QuestAccepted
                },
                new SearchTreeEntry(new GUIContent("QuestActivator", indentationIcon))
                {
                    level = 2,
                    userData = QSQuestNodeType.QuestActivator
                },
                new SearchTreeEntry(new GUIContent("ConditionSetter", indentationIcon))
                {
                    level = 2,
                    userData = QSQuestNodeType.ConditionSetter
                }

            };
            return searchTreeEntries;
        }
        
        
        public bool OnSelectEntry(SearchTreeEntry SearchTreeEntry, SearchWindowContext context)
        {
            Vector2 localMousePosition = graphView.GetLocalMousePosition(context.screenMousePosition, true);

            switch (SearchTreeEntry.userData)
            {
                case QSQuestNodeType.QuestHandler:
                {
                    QSQuestHandlerNode questHandlerNode =
                        (QSQuestHandlerNode)graphView.CreateNode("QuestName", 
                            QSQuestNodeType.QuestHandler, localMousePosition);
                    graphView.AddElement(questHandlerNode);
                    return true;
                }
                case QSQuestNodeType.DialogueGraph:
                {
                    
                    QSDialogueGraphNode dialogueGraphNode =
                        (QSDialogueGraphNode)graphView.CreateNode("DialogueGraphNodeName", 
                            QSQuestNodeType.DialogueGraph, localMousePosition);
                    graphView.AddElement(dialogueGraphNode);
                    return true;
                }
                case QSQuestNodeType.Activator:
                {
                    QSActivatorNode activatorNode = (QSActivatorNode) graphView.CreateNode("ActivatorNodeName",
                        QSQuestNodeType.Activator, localMousePosition);
                    graphView.AddElement(activatorNode);
                    return true;
                }
                case QSQuestNodeType.Condition:
                {
                    QSConditionNode conditionNode = (QSConditionNode)graphView.CreateNode("ConditionNodeName",
                        QSQuestNodeType.Condition, localMousePosition);
                    foreach (var node in graphView.nodes)
                    {
                        if (node.GetType() == typeof(QSQuestHandlerNode))
                        {
                            QSQuestHandlerNode questHandlerNode = (QSQuestHandlerNode)node;
                            questHandlerNode.SendQuestHandlerAddedEvent(questHandlerNode);
                        }
                    }
                    graphView.AddElement(conditionNode);
                    return true;
                }
                case QSQuestNodeType.QuestAccepted:
                {
                    QSQuestAcceptedNode questAcceptedNode =
                        (QSQuestAcceptedNode)graphView.CreateNode("QuestAcceptedNodeName",
                            QSQuestNodeType.QuestAccepted, localMousePosition);
                    CheckForQuestHandlerNodes();
                    graphView.AddElement(questAcceptedNode);
                    return true;
                }
                case QSQuestNodeType.QuestActivator:
                {
                    QSQuestActivatorNode questActivatorNode =
                        (QSQuestActivatorNode)graphView.CreateNode("QuestActivatorNodeName",
                            QSQuestNodeType.QuestActivator, localMousePosition);
                    graphView.AddElement(questActivatorNode);
                    return true;
                }
                case QSQuestNodeType.ConditionSetter:
                {
                    QSConditionSetterNode conditionSetterNode =
                        (QSConditionSetterNode)graphView.CreateNode("ConditionSetterNodeName",
                            QSQuestNodeType.ConditionSetter, localMousePosition);
                    graphView.AddElement(conditionSetterNode);
                    return true;
                }
                default:
                {
                    return false;
                }
            }
        }

        public void CheckForQuestHandlerNodes()
        {
            foreach (var node in graphView.nodes)
            {
                if (node.GetType() == typeof(QSQuestHandlerNode))
                {
                    QSQuestHandlerNode questHandlerNode = (QSQuestHandlerNode)node;
                    questHandlerNode.SendQuestHandlerAddedEvent(questHandlerNode);
                }
            }
        }
        
        
    }
}

