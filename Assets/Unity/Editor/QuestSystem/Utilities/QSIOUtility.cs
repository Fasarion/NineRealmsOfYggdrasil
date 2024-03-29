using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DS.Data.Save;
using DS.Elements;
using QS.Data.Save;
using QS.Elements;
using QS.Enumerations;
using QS.Windows;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class QSIOUtility : MonoBehaviour
{
    private static QSGraphView graphView;
    private static string graphFileName;
    private static string originalGraphFileName;
    private static string containerFolderPath;
    private static List<QSNode> allNodes;
    private static List<QSQuestHandlerNode> questHandlerNodes;
    private static List<QSDialogueGraphNode> dialogueGraphNodes;
    private static List<QSActivatorNode> activatorNodes;
    private static List<QSConditionNode> conditionNodes;
    private static List<QSQuestAcceptedNode> questAcceptedNodes;
    private static List<QSQuestActivatorNode> questActivatorNodes;
    private static List<QSConditionSetterNode> conditionSetterNodes;

    private static Dictionary<string, QSQuestHandlerSO> createdQuestHandlers;
    private static Dictionary<string, QSActivatorSO> createdActivators;
    private static Dictionary<string, QSDialogueGraphSO> createdDialogueGraphs;
    private static Dictionary<string, QSConditionSO> createdConditions;
    private static Dictionary<string, QSQuestAcceptedSO> createdQuestAcceptors;
    private static Dictionary<string, QSQuestSO> createdNodes;
    private static Dictionary<string, QSQuestActivatorSO> createdQuestActivators;
    private static Dictionary<string, QSConditionSetterSO> createdConditionSetters;

    public static Dictionary<string, QSNode> loadedNodes;
    public static Dictionary<string, QSDialogueGraphNode> loadedDialogueGraphs;
    public static Dictionary<string, QSQuestHandlerNode> loadedQuestHandlers;
    
    private static QSGraphSaveDataSO graphData;
    
    //private static bool savedButNotloaded;

    private static List<ConvertDialogueSOToQSDialogueNodeSO> dialogueToNodeConverters;


    public static void Initialize(QSGraphView qsGraphView, string graphName, bool updateFolderPath = false)
    {
        graphView = qsGraphView;
        graphFileName = graphName;
        if (updateFolderPath)
        {
            containerFolderPath = $"Assets/QuestSystem/Quests/{graphFileName}";
        }
        
        dialogueToNodeConverters = new List<ConvertDialogueSOToQSDialogueNodeSO>();

        allNodes = new List<QSNode>();
        questHandlerNodes = new List<QSQuestHandlerNode>();
        dialogueGraphNodes = new List<QSDialogueGraphNode>();
        activatorNodes = new List<QSActivatorNode>();
        conditionNodes = new List<QSConditionNode>();
        questAcceptedNodes = new List<QSQuestAcceptedNode>();
        questActivatorNodes = new List<QSQuestActivatorNode>();
        conditionSetterNodes = new List<QSConditionSetterNode>();

        createdQuestHandlers = new Dictionary<string, QSQuestHandlerSO>();
        createdDialogueGraphs = new Dictionary<string, QSDialogueGraphSO>();
        createdActivators = new Dictionary<string, QSActivatorSO>();
        createdConditions = new Dictionary<string, QSConditionSO>();
        createdQuestAcceptors = new Dictionary<string, QSQuestAcceptedSO>();
        createdQuestActivators= new Dictionary<string, QSQuestActivatorSO>();
        createdNodes = new Dictionary<string, QSQuestSO>();
        createdConditionSetters = new Dictionary<string, QSConditionSetterSO>();

        loadedNodes = new Dictionary<string, QSNode>();
        loadedDialogueGraphs = new Dictionary<string, QSDialogueGraphNode>();
        loadedQuestHandlers = new Dictionary<string, QSQuestHandlerNode>();

        graphView.ClearSubscriptions();
    }
    
    #region SaveMethods

    public static void Save()
    {
        CreateStaticFolders();
        GetElementsFromGraphView();
        
        graphData = CreateAsset<QSGraphSaveDataSO>("Assets/Editor/QuestSystem/Graphs", $"{graphFileName}Graph");
        graphData.Initialize(graphFileName);

        QSQuestDataContainerSO questDataContainer = CreateAsset<QSQuestDataContainerSO>(containerFolderPath, graphFileName);
        
        questDataContainer.Initialize(graphFileName);
        
        SaveNodes(graphData, questDataContainer);
        
        SaveAsset(graphData);
        SaveAsset(questDataContainer);
        Debug.Log("Saving");
        Debug.Log("File name:" + graphFileName);
        Debug.Log("Container name:" + containerFolderPath);
        //Might have to try moving this up
        //savedButNotloaded = true;
    }
    
    private static void GetElementsFromGraphView()
    {
        graphView.graphElements.ForEach(graphElement =>
        {
            
            if (graphElement is QSNode node)
            {
                allNodes.Add(node);
                if (graphElement is QSDialogueGraphNode dialogueGraphNode)
                {
                    dialogueGraphNodes.Add(dialogueGraphNode);
                }
                else if (graphElement is QSQuestHandlerNode questHandlerNode)
                {
                    questHandlerNodes.Add(questHandlerNode);
                }
                else if (graphElement is QSActivatorNode activatorNode)
                {
                    activatorNodes.Add(activatorNode);
                }
                else if (graphElement is QSConditionNode conditionNode)
                {
                    conditionNodes.Add(conditionNode);
                }
                else if (graphElement is QSQuestAcceptedNode questAcceptedNode)
                {
                    questAcceptedNodes.Add(questAcceptedNode);
                }
                else if (graphElement is QSQuestActivatorNode questActivatorNode)
                {
                    questActivatorNodes.Add(questActivatorNode);
                }
                else if (graphElement is QSConditionSetterNode conditionSetterSo)
                {
                    conditionSetterNodes.Add(conditionSetterSo);
                }
                return;
            }
        });
    }

    #region Nodes

     private static void SaveNodes(QSGraphSaveDataSO graphSaveData, QSQuestDataContainerSO questData)
    {
        List<string> questHandlerNodeNames = new List<string>();
        List<string> dialogueGraphNodeNames = new List<string>();
        List<string> activatorNodeNames = new List<string>();
        List<string> conditionNodeNames = new List<string>();
        List<string> questAcceptedNodeNames = new List<string>();
        List<string> questActivatorNodeNames = new List<string>();
        List<string> conditionSetterNodeNames = new List<string>();
        /*SerializableDictionary<string, List<string>> groupedNodeNames =
            new SerializableDictionary<string, List<string>>();
        List<string> ungroupedNodeNames = new List<string>();*/
        foreach (QSNode node in allNodes)
        {
            
            SaveNodeToGraph(node, graphSaveData);
            SaveNodeToScriptableObject(node, questData);
            
            
            switch (node.QuestNodeType)
            {
                case QSQuestNodeType.QuestHandler:
                {
                    questHandlerNodeNames.Add(node.NodeName);
                    break;
                }
                case QSQuestNodeType.DialogueGraph:
                {
                    dialogueGraphNodeNames.Add(node.NodeName);
                    break;
                }
                case QSQuestNodeType.Activator:
                {
                    activatorNodeNames.Add(node.NodeName);
                    break;
                }
                case QSQuestNodeType.Condition:
                {
                    conditionNodeNames.Add(node.NodeName);
                    break;
                }
                case QSQuestNodeType.QuestAccepted:
                {
                    questAcceptedNodeNames.Add(node.NodeName);
                    break;
                }
                case QSQuestNodeType.QuestActivator:
                {
                    questActivatorNodeNames.Add(node.NodeName);
                    break;
                }
                case QSQuestNodeType.ConditionSetter:
                {
                    conditionNodeNames.Add(node.NodeName);
                    break;
                }

            }
        }

        UpdateQuestBranchesConnections();
        UpdateOldQuestHandlerNodes(questHandlerNodeNames,graphSaveData);
        UpdateOldDialogueGraphNodes(dialogueGraphNodeNames,graphSaveData);
        UpdateOldActivatorNodes(activatorNodeNames, graphSaveData);
        UpdateOldConditionNodes(conditionNodeNames, graphSaveData);
        UpdateOldQuestAcceptedNodes(questAcceptedNodeNames, graphSaveData);
        UpdateOldQuestActivatorNodes(questActivatorNodeNames, graphSaveData);
        UpdateOldQuestConditionSetterNodes(conditionSetterNodeNames, graphSaveData);
       
     
        //UpdateOldUngroupedNodes(ungroupedNodeNames, graphData);

    }

     

     private static void SaveNodeToGraph(QSNode node, QSGraphSaveDataSO graphData)
    {
        
        //List<DSChoiceSaveData> choices = CloneNodeChoices(node.choices);
        List<QSBranchSaveData> branches = CloneNodeBranches(node.branches);
        //This might be where I need to create special methods to save specific nodes, unless they can be successfully stored as their derived type

        switch (node.QuestNodeType)
        {
            case QSQuestNodeType.QuestHandler:
            {
                var questHandlerNode = (QSQuestHandlerNode) node;
                //List<QSBranchSaveData> questBranches = CloneNodeBranches(node.branches);
                QSQuestHandlerNodeSaveData nodeData = new QSQuestHandlerNodeSaveData()
                {
                    ID = questHandlerNode.ID,
                    Name = questHandlerNode.NodeName,
                    Branches = branches,
                    QuestHandler = questHandlerNode.questHandler,
                    QuestNodeType = questHandlerNode.QuestNodeType,
                    Position = questHandlerNode.GetPosition().position
                };
            
                //Might not be enough that these are saved in the graphData.Nodes list. Might have to save each node type into separate lists
                graphData.Nodes.Add(nodeData);
                break;
            }
            case QSQuestNodeType.DialogueGraph:
            {
                var dialogueGraphNode = (QSDialogueGraphNode) node;
           
                QSDialogueGraphNodeSaveData nodeData = new QSDialogueGraphNodeSaveData()
                {
                    ID = dialogueGraphNode.ID,
                    Name = dialogueGraphNode.NodeName,
                    Branches = branches,
                    DialogueContainerSO = dialogueGraphNode.dialogueContainerSO,
                    QuestNodeType = dialogueGraphNode.QuestNodeType,
                    Position = dialogueGraphNode.GetPosition().position
                };
                graphData.Nodes.Add(nodeData);
                break;
            }
            case QSQuestNodeType.Activator:
            {
                var activatorGraphNode = (QSActivatorNode) node;
           
                QSActivatorNodeSaveData nodeData = new QSActivatorNodeSaveData()
                {
                    ID = activatorGraphNode.ID,
                    Name = activatorGraphNode.NodeName,
                    Branches = branches,
                    GameObjectsToActivateNames =  activatorGraphNode.GetObjectsToActivateNames(),
                    GameObjectToDeactivateNames = activatorGraphNode.GetObjectsToDeactivateNames(),
                    QuestNodeType = activatorGraphNode.QuestNodeType,
                    Position = activatorGraphNode.GetPosition().position
                };
                graphData.Nodes.Add(nodeData);
                break;
            }
            case QSQuestNodeType.Condition:
            {
                var conditionGraphNode = (QSConditionNode)node;
                QSConditionNodeSaveData nodeData = new QSConditionNodeSaveData()
                {
                    ID = conditionGraphNode.ID,
                    Name = conditionGraphNode.NodeName,
                    Branches = branches,
                    QuestNodeType = conditionGraphNode.QuestNodeType,
                    Position = conditionGraphNode.GetPosition().position,
                };
               
                graphData.Nodes.Add(nodeData);
                break;
            }
            case QSQuestNodeType.ConditionSetter:
            {
                var conditionSetterNode = (QSConditionSetterNode) node;
                QSConditionSetterNodeSaveData nodeData = new QSConditionSetterNodeSaveData()
                {
                    ID = conditionSetterNode.ID,
                    Name = conditionSetterNode.NodeName,
                    Branches = branches,
                    QuestNodeType = conditionSetterNode.QuestNodeType,
                    Position = conditionSetterNode.GetPosition().position
                };
                graphData.Nodes.Add(nodeData);
                break;
            }
            case QSQuestNodeType.QuestAccepted:
            {
                var questAcceptedNode = (QSQuestAcceptedNode) node;
                QSQuestAcceptedNodeSaveData nodeData = new QSQuestAcceptedNodeSaveData()
                {
                    ID = questAcceptedNode.ID,
                    Name = questAcceptedNode.NodeName,
                    Branches = branches,
                    QuestNodeType = questAcceptedNode.QuestNodeType,
                    Position = questAcceptedNode.GetPosition().position
                };
                graphData.Nodes.Add(nodeData);
                break;
            }
            
            case QSQuestNodeType.QuestActivator:
            {
                var questActivatorNode = (QSQuestActivatorNode)node;
                QSQuestActivatorNodeSaveData nodeData = new QSQuestActivatorNodeSaveData()
                {
                    ID = questActivatorNode.ID,
                    Name = questActivatorNode.NodeName,
                    Branches = branches,
                    QuestHandler = questActivatorNode.questHandler,
                    QuestNodeType = questActivatorNode.QuestNodeType,
                    Position = questActivatorNode.GetPosition().position
                };
                graphData.Nodes.Add(nodeData);
                break;
            }
        }
    }
    
     private static void SaveNodeToScriptableObject(QSNode node, QSQuestDataContainerSO questDataContainer)
    {
        
        switch (node.QuestNodeType)
        {
              
            case QSQuestNodeType.QuestHandler:
            {
                var questHandlerNode = (QSQuestHandlerNode)node;
                QSQuestHandlerSO questHandler = CreateAsset<QSQuestHandlerSO>($"{containerFolderPath}/QuestHandlers",questHandlerNode.NodeName);
                questDataContainer.questHandlerSOs.Add(questHandler);
                
                questHandler.Initialize(questHandlerNode.NodeName,ConvertQuestBranchesToQuestBranchData(questHandlerNode.branches), questHandlerNode.questHandler,QSQuestNodeType.QuestHandler, questHandlerNode.IsStartingNode());
                createdQuestHandlers.Add(questHandlerNode.ID, questHandler);
                createdNodes.Add(questHandlerNode.ID, questHandler);
                SaveAsset(questHandler);
                break;
            }
            case QSQuestNodeType.DialogueGraph:
            {
                var dialogueGraphNode = (QSDialogueGraphNode)node;
                QSDialogueGraphSO dialogueGraph = CreateAsset<QSDialogueGraphSO>($"{containerFolderPath}/DialogueGraphs",dialogueGraphNode.NodeName);
                questDataContainer.questDialogueGraphSOs.Add(dialogueGraph);
                dialogueGraph.Initialize(dialogueGraphNode.NodeName,ConvertQuestBranchesToQuestBranchData(dialogueGraphNode.branches), dialogueGraphNode.dialogueContainerSO, QSQuestNodeType.DialogueGraph, dialogueGraphNode.IsStartingNode());
                createdDialogueGraphs.Add(dialogueGraphNode.ID, dialogueGraph);
                createdNodes.Add(dialogueGraphNode.ID,dialogueGraph);
                if (dialogueGraphNode.IsStartingNode())
                {
                    questDataContainer.startingNode = dialogueGraph;
                }
                SaveAsset(dialogueGraph);
               
                
                break;
            }
            case QSQuestNodeType.Activator:
            {
                var activatorNode = (QSActivatorNode) node;
                QSActivatorSO activator = CreateAsset<QSActivatorSO>($"{containerFolderPath}/Activators",activatorNode.NodeName);
                questDataContainer.activatorSOs.Add(activator);
                activator.Initialize(activatorNode.NodeName,ConvertQuestBranchesToQuestBranchData(activatorNode.branches), activatorNode.GetObjectsToActivateNames(),activatorNode.GetObjectsToDeactivateNames() ,QSQuestNodeType.Activator, activatorNode.IsStartingNode());
                createdActivators.Add(activatorNode.ID, activator);
                createdNodes.Add(activatorNode.ID,activator);
                if (activatorNode.IsStartingNode())
                {
                    questDataContainer.startingNode = activator;
                }
                SaveAsset(activator);
                break;
                
            }

            case QSQuestNodeType.Condition:
            {
                var conditionNode = (QSConditionNode)node;
                QSConditionSO condition =
                    CreateAsset<QSConditionSO>($"{containerFolderPath}/Conditions", conditionNode.NodeName);
                condition.Initialize(conditionNode.NodeName, ConvertQuestBranchesToQuestBranchData(conditionNode.branches), QSQuestNodeType.Condition, conditionNode.IsStartingNode());
                createdConditions.Add(conditionNode.ID, condition);
                createdNodes.Add(conditionNode.ID, condition);
               
                SaveAsset(condition);
                break;
            }
            
            case QSQuestNodeType.ConditionSetter:
            {
                var conditionSetterNode = (QSConditionSetterNode)node;
                QSConditionSetterSO conditionSetter =
                    CreateAsset<QSConditionSetterSO>($"{containerFolderPath}/ConditionSetters", conditionSetterNode.NodeName);
                conditionSetter.Initialize(conditionSetterNode.NodeName, ConvertQuestBranchesToQuestBranchData(conditionSetterNode.branches), QSQuestNodeType.ConditionSetter, conditionSetterNode.IsStartingNode());
                createdConditionSetters.Add(conditionSetterNode.ID, conditionSetter);
                createdNodes.Add(conditionSetterNode.ID, conditionSetter);
                if (conditionSetterNode.IsStartingNode())
                {
                    questDataContainer.startingNode = conditionSetter;
                }
                SaveAsset(conditionSetter);
                break;

            }

            case QSQuestNodeType.QuestAccepted:
            {
                var questAcceptedNode = (QSQuestAcceptedNode)node;
                QSQuestAcceptedSO questAccepted =
                    CreateAsset<QSQuestAcceptedSO>($"{containerFolderPath}/QuestAcceptors", questAcceptedNode.NodeName);
                questAccepted.Initialize(questAcceptedNode.NodeName, ConvertQuestBranchesToQuestBranchData(questAcceptedNode.branches), QSQuestNodeType.QuestAccepted, questAcceptedNode.IsStartingNode());
                createdQuestAcceptors.Add(questAcceptedNode.ID, questAccepted);
                createdNodes.Add(questAcceptedNode.ID, questAccepted);
                if (questAcceptedNode.IsStartingNode())
                {
                    questDataContainer.startingNode = questAccepted;
                }
                SaveAsset(questAccepted);
                break;

            }
            case QSQuestNodeType.QuestActivator:
            {
                var questActivatorNode = (QSQuestActivatorNode)node;
                QSQuestActivatorSO questActivator =
                    CreateAsset<QSQuestActivatorSO>($"{containerFolderPath}/QuestActivators", questActivatorNode.NodeName);
                questActivator.Initialize(questActivatorNode.NodeName, ConvertQuestBranchesToQuestBranchData(questActivatorNode.branches),questActivatorNode.questHandler, QSQuestNodeType.QuestActivator, questActivatorNode.IsStartingNode());
                createdQuestActivators.Add(questActivatorNode.ID, questActivator);
                createdNodes.Add(questActivatorNode.ID, questActivator);
                if (questActivatorNode.IsStartingNode())
                {
                    questDataContainer.startingNode = questActivator;
                }
                SaveAsset(questActivator);
                break;

            }
            
            
        }


    }
     
     private static List<QSQuestBranchData> ConvertQuestBranchesToQuestBranchData(
         List<QSBranchSaveData> questBranches)
     {
         List<QSQuestBranchData> questBranchData = new List<QSQuestBranchData>();
         foreach (QSBranchSaveData questBranch in questBranches)
         {
             QSQuestBranchData branchData = new QSQuestBranchData()
             {
                 Text = questBranch.Text
             };
             questBranchData.Add(branchData);

         }

         return questBranchData;
     }

     
     private static void UpdateQuestBranchesConnections()
     {
         foreach (QSActivatorNode node in activatorNodes)
         {
             QSActivatorSO activator = createdActivators[node.ID];

             for (int branchIndex = 0; branchIndex < node.branches.Count; ++branchIndex)
             {
                 QSBranchSaveData nodeBranch = node.branches[branchIndex];

                 if (string.IsNullOrEmpty(nodeBranch.NodeID))
                 {
                     continue;
                 }
                
                 activator.Branches[branchIndex].NextQuestNode = createdNodes[nodeBranch.NodeID];
                 SaveAsset(activator);
             }
         }
        
         foreach (QSQuestHandlerNode node in questHandlerNodes)
         {
             QSQuestHandlerSO questHandler = createdQuestHandlers[node.ID];

             for (int branchIndex = 0; branchIndex < node.branches.Count; ++branchIndex)
             {
                 QSBranchSaveData nodeBranch = node.branches[branchIndex];

                 if (string.IsNullOrEmpty(nodeBranch.NodeID))
                 {
                     continue;
                 }
                
                 questHandler.Branches[branchIndex].NextQuestNode = createdNodes[nodeBranch.NodeID];
                 SaveAsset(questHandler);
             }
         }
        
         foreach (QSDialogueGraphNode node in dialogueGraphNodes)
         {
             QSDialogueGraphSO dialogueGraph = createdDialogueGraphs[node.ID];

             for (int branchIndex = 0; branchIndex < node.branches.Count; ++branchIndex)
             {
                 QSBranchSaveData nodeBranch = node.branches[branchIndex];

                 if (string.IsNullOrEmpty(nodeBranch.NodeID))
                 {
                     continue;
                 }
                
                 
                 dialogueGraph.Branches[branchIndex].NextQuestNode = createdNodes[nodeBranch.NodeID];
                 SaveAsset(dialogueGraph);
             }
         }
         
         foreach (QSConditionNode node in conditionNodes)
         {
             QSConditionSO condition = createdConditions[node.ID];

             for (int branchIndex = 0; branchIndex < node.branches.Count; ++branchIndex)
             {
                 QSBranchSaveData nodeBranch = node.branches[branchIndex];

                 if (string.IsNullOrEmpty(nodeBranch.NodeID))
                 {
                     continue;
                 }
                
                 
                 condition.Branches[branchIndex].NextQuestNode = createdNodes[nodeBranch.NodeID];
                 SaveAsset(condition);
             }
         }
         
         foreach (QSQuestAcceptedNode node in questAcceptedNodes)
         {
             QSQuestAcceptedSO questAccepted = createdQuestAcceptors[node.ID];

             for (int branchIndex = 0; branchIndex < node.branches.Count; ++branchIndex)
             {
                 QSBranchSaveData nodeBranch = node.branches[branchIndex];

                 if (string.IsNullOrEmpty(nodeBranch.NodeID))
                 {
                     continue;
                 }
                
                 
                 questAccepted.Branches[branchIndex].NextQuestNode = createdNodes[nodeBranch.NodeID];
                 SaveAsset(questAccepted);
             }
         }
         
         foreach (QSQuestActivatorNode node in questActivatorNodes)
         {
             QSQuestActivatorSO questActivator = createdQuestActivators[node.ID];

             for (int branchIndex = 0; branchIndex < node.branches.Count; ++branchIndex)
             {
                 QSBranchSaveData nodeBranch = node.branches[branchIndex];

                 if (string.IsNullOrEmpty(nodeBranch.NodeID))
                 {
                     continue;
                 }
                
                 
                 questActivator.Branches[branchIndex].NextQuestNode = createdNodes[nodeBranch.NodeID];
                 SaveAsset(questActivator);
             }
         }
         
         foreach (QSConditionSetterNode node in conditionSetterNodes)
         {
             QSConditionSetterSO conditionSetter = createdConditionSetters[node.ID];

             for (int branchIndex = 0; branchIndex < node.branches.Count; ++branchIndex)
             {
                 QSBranchSaveData nodeBranch = node.branches[branchIndex];

                 if (string.IsNullOrEmpty(nodeBranch.NodeID))
                 {
                     continue;
                 }
                 conditionSetter.Branches[branchIndex].NextQuestNode = createdNodes[nodeBranch.NodeID];
                 SaveAsset(conditionSetter);
             }
         }

     }
     private static void UpdateOldConditionNodes(List<string> conditionNodeNames, QSGraphSaveDataSO graphData)
     {
         if (graphData.OldConditionNodeNames != null && graphData.OldConditionNodeNames.Count != 0)
         {
             List<string> nodesToRemove = graphData.OldConditionNodeNames.Except(conditionNodeNames).ToList();

             foreach (string nodeToRemove in nodesToRemove)
             {
                 RemoveAsset($"{containerFolderPath}/Conditions", nodeToRemove);
             }
         }
         graphData.OldConditionNodeNames = new List<string>(conditionNodeNames);

         
     }
     
    private static void UpdateOldActivatorNodes(List<string> activatorNodeNames, QSGraphSaveDataSO graphData)
     {
         if (graphData.OldActivatorNodeNames != null && graphData.OldActivatorNodeNames.Count != 0)
         {
             List<string> nodesToRemove = graphData.OldActivatorNodeNames.Except(activatorNodeNames).ToList();

             foreach (string nodeToRemove in nodesToRemove)
             {
                 RemoveAsset($"{containerFolderPath}/Activators", nodeToRemove);
             }
         }
         graphData.OldActivatorNodeNames = new List<string>(activatorNodeNames);

         
     }
    
    private static void UpdateOldQuestHandlerNodes(List<string> currentQuestHandlerNodeNames, QSGraphSaveDataSO graphData)
    {
        if (graphData.OldQuestHandlerNodeNames != null && graphData.OldQuestHandlerNodeNames.Count != 0)
        {
            List<string> nodesToRemove = graphData.OldQuestHandlerNodeNames.Except(currentQuestHandlerNodeNames).ToList();
            
            foreach (string nodeToRemove in nodesToRemove)
            {
                RemoveAsset($"{containerFolderPath}/QuestHandlers", nodeToRemove);
            }
        }
        graphData.OldQuestHandlerNodeNames = new List<string>(currentQuestHandlerNodeNames);

        
    }
    
    private static void UpdateOldDialogueGraphNodes(List<string> currentDialogueGraphNodeNames, QSGraphSaveDataSO graphData)
    {
        if (graphData.OldDialogueGraphNodeNames != null && graphData.OldDialogueGraphNodeNames.Count != 0)
        {
            List<string> nodesToRemove = graphData.OldDialogueGraphNodeNames.Except(currentDialogueGraphNodeNames).ToList();

            foreach (string nodeToRemove in nodesToRemove)
            {
                
                RemoveAsset($"{containerFolderPath}/DialogueGraphs", nodeToRemove);
            }

        }
        graphData.OldDialogueGraphNodeNames = new List<string>(currentDialogueGraphNodeNames);

        
    }
    
    private static void UpdateOldQuestAcceptedNodes(List<string> currentQuestAcceptedNodeNames, QSGraphSaveDataSO graphData)
    {
        if (graphData.OldQuestAcceptedNodeNames != null && graphData.OldQuestAcceptedNodeNames.Count != 0)
        {
            List<string> nodesToRemove = graphData.OldQuestAcceptedNodeNames.Except(currentQuestAcceptedNodeNames).ToList();

            foreach (string nodeToRemove in nodesToRemove)
            {
                
                RemoveAsset($"{containerFolderPath}/QuestAcceptors", nodeToRemove);
            }
            
        }
        graphData.OldQuestAcceptedNodeNames = new List<string>(currentQuestAcceptedNodeNames);

        
    }
    
    private static void UpdateOldQuestActivatorNodes(List<string> currentQuestActivatorNodeNames, QSGraphSaveDataSO graphData)
    {
        if (graphData.OldQuestActivatorNodeNames != null && graphData.OldQuestActivatorNodeNames.Count != 0)
        {
            List<string> nodesToRemove = graphData.OldQuestActivatorNodeNames.Except(currentQuestActivatorNodeNames).ToList();

            foreach (string nodeToRemove in nodesToRemove)
            {
               
                RemoveAsset($"{containerFolderPath}/QuestActivators", nodeToRemove);
            }
            
        }
        graphData.OldQuestActivatorNodeNames = new List<string>(currentQuestActivatorNodeNames);
    }
    
    private static void UpdateOldQuestConditionSetterNodes(List<string> currentConditionSetterNodeNames, QSGraphSaveDataSO graphData)
    {
        if (graphData.OldConditionSetterNodeNames != null && graphData.OldConditionSetterNodeNames.Count != 0)
        {
            List<string> nodesToRemove = graphData.OldConditionSetterNodeNames.Except(currentConditionSetterNodeNames).ToList();

            foreach (string nodeToRemove in nodesToRemove)
            {
                
                RemoveAsset($"{containerFolderPath}/ConditionSetters", nodeToRemove);
            }
            
        }
        graphData.OldConditionSetterNodeNames = new List<string>(currentConditionSetterNodeNames);
    }

    

    #endregion
   
    
    #endregion

    #region Load Methods

    public static void Load()
    {
        graphData =  LoadAsset<QSGraphSaveDataSO>("Assets/Editor/QuestSystem/Graphs", graphFileName);
        
        if (graphData == null)
        {
            EditorUtility.DisplayDialog(
                "Couldn't load the file!",
                "The file at the following path could not be found: \n\n" +
                $"Assets/Editor/QuestSystem/Graphs/{graphFileName}\n\n" +
                "Make sure you chose the right file and its place at the folder path mentioned above.",
                "Thanks!"
            );
            
            return;
        }
        
        Debug.Log("Loading");
        Debug.Log("File name:" + graphFileName);
        Debug.Log("Container name:" + containerFolderPath);
        QSEditorWindow.UpdateFileName(graphData.FileName);

        LoadNodes(graphData.Nodes);
        LoadNodesConnections();
        
        
        
        foreach (var questHandler in loadedQuestHandlers)
        {   
            graphView.SendQuestHandlerAddedEvent(questHandler.Value);
            
        }
        //savedButNotloaded = false;
    }

    private static void LoadNodes(List<QSNodeSaveData> nodes)
    {
        foreach (var nodeData in nodes)
        {
            List<QSBranchSaveData> branches = CloneNodeBranches(nodeData.Branches);
            switch (nodeData.QuestNodeType)
            {
                case QSQuestNodeType.QuestHandler:
                {
                    var questNodeData = (QSQuestHandlerNodeSaveData) nodeData;
                    QSQuestHandlerNode qsQuestHandlerNode = (QSQuestHandlerNode)graphView.CreateNode(questNodeData.Name,
                        QSQuestNodeType.QuestHandler, nodeData.Position, false);
                    qsQuestHandlerNode.ID = questNodeData.ID;
                    qsQuestHandlerNode.branches = branches;
                    qsQuestHandlerNode.questHandler = questNodeData.QuestHandler;
                    qsQuestHandlerNode.Draw();
                    graphView.AddElement(qsQuestHandlerNode);
                    loadedNodes.Add(qsQuestHandlerNode.ID, qsQuestHandlerNode);
                    loadedQuestHandlers.Add(qsQuestHandlerNode.ID, qsQuestHandlerNode);
                    break;
                }
                case QSQuestNodeType.DialogueGraph:
                {
                    var dialogueGraphNodeData = (QSDialogueGraphNodeSaveData) nodeData;
                    QSDialogueGraphNode qsDialogueGraphNode = (QSDialogueGraphNode)graphView.CreateNode(dialogueGraphNodeData.Name,
                        QSQuestNodeType.DialogueGraph, nodeData.Position, false);
                    qsDialogueGraphNode.ID = dialogueGraphNodeData.ID;
                    qsDialogueGraphNode.branches = branches;
                    qsDialogueGraphNode.dialogueContainerSO = dialogueGraphNodeData.DialogueContainerSO;
                    qsDialogueGraphNode.Draw();
                    graphView.AddElement(qsDialogueGraphNode);
                    loadedNodes.Add(qsDialogueGraphNode.ID, qsDialogueGraphNode);
                    loadedDialogueGraphs.Add(qsDialogueGraphNode.ID, qsDialogueGraphNode);
                    break;
                }
                case QSQuestNodeType.Activator:
                {
                    var activatorNodeData = (QSActivatorNodeSaveData) nodeData;
                    QSActivatorNode qsActivatorNode = (QSActivatorNode)graphView.CreateNode(activatorNodeData.Name,
                        QSQuestNodeType.Activator, nodeData.Position, false);
                    qsActivatorNode.ID = activatorNodeData.ID;
                    qsActivatorNode.branches = branches;
                    qsActivatorNode.gameObjectsToActivateNames = activatorNodeData.GameObjectsToActivateNames;
                    qsActivatorNode.gameObjectsToDeactivateNames = activatorNodeData.GameObjectToDeactivateNames;
                    qsActivatorNode.Draw();
                    graphView.AddElement(qsActivatorNode);
                    loadedNodes.Add(qsActivatorNode.ID, qsActivatorNode);
                    break;
                }
                case QSQuestNodeType.Condition:
                {
                    var conditionNodeData = (QSConditionNodeSaveData)nodeData;
                    QSConditionNode qsConditionNode = (QSConditionNode)graphView.CreateNode(conditionNodeData.Name, QSQuestNodeType.Condition, nodeData.Position, false);
                    qsConditionNode.ID = conditionNodeData.ID;
                    qsConditionNode.branches = branches;
                    qsConditionNode.Draw();
                    graphView.AddElement(qsConditionNode);
                    loadedNodes.Add(qsConditionNode.ID, qsConditionNode);
                    break;
                }
                case QSQuestNodeType.ConditionSetter:
                {
                    var conditionSetterNodeSaveData = (QSConditionSetterNodeSaveData)nodeData;
                    QSConditionSetterNode qsConditionSetterNode = (QSConditionSetterNode)graphView.CreateNode(conditionSetterNodeSaveData.Name, QSQuestNodeType.ConditionSetter, nodeData.Position, false);
                    qsConditionSetterNode.ID = conditionSetterNodeSaveData.ID;
                    qsConditionSetterNode.branches = branches;
                    qsConditionSetterNode.Draw();
                    graphView.AddElement(qsConditionSetterNode);
                    loadedNodes.Add(qsConditionSetterNode.ID, qsConditionSetterNode);
                    break;
                }
                
                case QSQuestNodeType.QuestAccepted:
                {
                    var questAcceptedNodeSaveData = (QSQuestAcceptedNodeSaveData)nodeData;
                    QSQuestAcceptedNode qsQuestAcceptedNode = (QSQuestAcceptedNode)graphView.CreateNode(questAcceptedNodeSaveData.Name, QSQuestNodeType.QuestAccepted, nodeData.Position, false);
                    qsQuestAcceptedNode.ID = questAcceptedNodeSaveData.ID;
                    qsQuestAcceptedNode.branches = branches;
                    qsQuestAcceptedNode.Draw();
                    graphView.AddElement(qsQuestAcceptedNode);
                    loadedNodes.Add(qsQuestAcceptedNode.ID, qsQuestAcceptedNode);
                    break;
                }
                case QSQuestNodeType.QuestActivator:
                {
                    var questActivatorNodeSaveData = (QSQuestActivatorNodeSaveData)nodeData;
                    QSQuestActivatorNode qsQuestActivatorNode =
                        (QSQuestActivatorNode)graphView.CreateNode(questActivatorNodeSaveData.Name,
                            QSQuestNodeType.QuestActivator, nodeData.Position, false);
                    qsQuestActivatorNode.ID = questActivatorNodeSaveData.ID;
                    qsQuestActivatorNode.branches = branches;
                    qsQuestActivatorNode.questHandler = questActivatorNodeSaveData.QuestHandler;
                    qsQuestActivatorNode.Draw();
                    graphView.AddElement(qsQuestActivatorNode);
                    loadedNodes.Add(qsQuestActivatorNode.ID, qsQuestActivatorNode);
                    break;
                }
                
            }

        }
        
        
       
        
        
        //Can probably put the dialoguegraphnodes into a single list
      
    }

    public static void LoadNodesConnections()
    {
        //Go through all the loaded nodes
        foreach (KeyValuePair<string, QSNode> loadedNode in loadedNodes)
        {
            //Go through all output ports. All outward going node connections
            foreach (Port choicePort in loadedNode.Value.outputContainer.Children())
            {
                //Grab the userData from the Port. This info should include the ID and name.
                QSBranchSaveData branchData = (QSBranchSaveData)choicePort.userData;

                if (string.IsNullOrEmpty(branchData.NodeID))
                {
                    continue;
                }

                QSNode nextNode = loadedNodes[branchData.NodeID];

                Port nextNodeInputPort = (Port)nextNode.inputContainer.Children().First();
                //var node = nextNodeInputPort.node;

                Edge edge = choicePort.ConnectTo(nextNodeInputPort);

                graphView.AddElement(edge);

                loadedNode.Value.RefreshPorts();

            }

        }
        
        foreach (var nodeData in loadedDialogueGraphs)
        {
            nodeData.Value.RedrawBranches();
        }
    }

    #endregion
    public static ConvertDialogueSOToQSDialogueNodeSO LoadConvertedNodeDataFromScriptableObject(string name)
    {
        return LoadAsset<ConvertDialogueSOToQSDialogueNodeSO>($"Assets/QuestSystem/Converters/", name);
    }
    
    public static void DeleteConvertedNodeDataScriptableObject(string fileName)
    {
        RemoveAsset("Assets/QuestSystem/Converters/", fileName);
    }
    
    
   #region CreationMethods
   
   private static void CreateStaticFolders()
   {
       CreateFolder("Assets/", " QuestSystem");
       CreateFolder("Assets/Editor/QuestSystem", "Graphs");
       CreateFolder("Assets/QuestSystem", "Quests");
       CreateFolder("Assets/QuestSystem/Quests", graphFileName);
       CreateFolder(containerFolderPath, "QuestHandlers");
       CreateFolder(containerFolderPath, "DialogueGraphs");
       CreateFolder(containerFolderPath, "Activators");
       CreateFolder(containerFolderPath, "Conditions");
       CreateFolder(containerFolderPath, "ConditionSetters");
       CreateFolder(containerFolderPath, "QuestAcceptors");
       CreateFolder(containerFolderPath, "QuestActivators");
       
       CreateFolder("Assets/QuestSystem", "Converters");
       //CreateFolder($"{containerFolderPath}/Global", "Quests");

   }
   
   #endregion

   
    #region Utility Methods
    public static void CreateFolder(string path, string folderName)
    {
        if (AssetDatabase.IsValidFolder($"{path}/{folderName}"))
        {
            return;
        }
        AssetDatabase.CreateFolder(path, folderName);

    }

    public static void RemoveFolder(string fullPath)
    {

        FileUtil.DeleteFileOrDirectory($"{fullPath}.meta");
        FileUtil.DeleteFileOrDirectory($"{fullPath}/");


    }


    public static T CreateAsset<T>(string path, string assetName) where T : ScriptableObject
    {
        string fullPath = $"{path}/{assetName}.asset";
        T asset = LoadAsset<T>(path, assetName);

        if (asset == null)
        {
            asset = ScriptableObject.CreateInstance<T>();

            AssetDatabase.CreateAsset(asset, fullPath);
        }


        return asset;


    }

    public static T LoadAsset<T>(string path, string assetName) where T : ScriptableObject
    {
        string fullPath = $"{path}/{assetName}.asset";
        return AssetDatabase.LoadAssetAtPath<T>(fullPath);
    }
    
    public static void RemoveAsset(string path, string assetName)
    {
        AssetDatabase.DeleteAsset($"{path}/{assetName}.asset");
        //AssetDatabase.Refresh();
    }
    
    public static void SaveAsset(UnityEngine.Object asset)
    {
        EditorUtility.SetDirty(asset);

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
    }

    private static List<QSBranchSaveData> CloneNodeBranches(List<QSBranchSaveData> nodeChoices)
    {
        List<QSBranchSaveData> choices = new List<QSBranchSaveData>();

        foreach (QSBranchSaveData choice in nodeChoices)
        {
            QSBranchSaveData choiceData = new QSBranchSaveData()
            {
                Text = choice.Text,
                NodeID = choice.NodeID
            };

            choices.Add(choiceData);
        }

        return choices;
    }
    

    #endregion

    #region Junk

    public static void CreateNodeDataToScriptableObjectConverter(string fileName)
    {
        
        CreateStaticFolders();
        ConvertDialogueSOToQSDialogueNodeSO dialogueConverter;
        dialogueConverter =
            CreateAsset<ConvertDialogueSOToQSDialogueNodeSO>($"Assets/QuestSystem/Converters", fileName);
        dialogueConverter.Initialize(fileName);
        EditorUtility.SetDirty(dialogueConverter);
        SaveAsset(dialogueConverter);
        



    }

    public static void SaveConvertedNodeDataToScriptableObject(string fileName, ConvertDialogueSOToQSDialogueNodeSO dialogueConverterSO)
    {
        //CreateStaticFolders();
        //ConvertDialogueSOToQSDialogueNodeSO dialogueConverter;
        EditorUtility.SetDirty(dialogueConverterSO);
        SaveAsset(dialogueConverterSO);
        /* dialogueConverter =
             CreateAsset<ConvertDialogueSOToQSDialogueNodeSO>($"Assets/QuestSystem/Converters/", fileName);
         dialogueConverter.Initialize(fileName);*/
        //dialogueConverter.exitDialogues = dialogueConverterSO.exitDialogues;
        //dialogueConverter.allDialogueNodes = dialogueConverterSO.allDialogueNodes;
        //EditorUtility.SetDirty(dialogueConverter);
        //SaveAsset(dialogueConverter);
    }

    #endregion
   
}
