using System;
using System.Collections;
using System.Collections.Generic;
using DS;
using DS.Data.Save;
using DS.Elements;
using DS.Utilities;
using QS.Data.Save;
using QS.Elements;
using QS.Enumerations;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;
using Edge = UnityEditor.Experimental.GraphView.Edge;
using Node = UnityEditor.Graphs.AnimationBlendTree.Node;

namespace QS.Windows
{
    public class QSGraphView : GraphView
    {
        public QSEditorWindow editorWindow;
        private QSSearchWindow searchWindow;

        public HashSet<string> uniqueNodeNames;

        public event Action<QSQuestHandlerNode> questHandlerAdded;
        public event Action<QSQuestHandlerNode> questHandlerRemoved;
        public event Action <QSConditionNode> conditionNodeAdded;
        public event Action <QSConditionNode> conditionNodeRemoved;
        public event Action<QSConditionSetterNode> conditionSetterNodeAdded;
        //public event Action<QSConditionSetterNode> conditionSetterNodeRemoved;
        public event Action <QSQuestAcceptedNode> questAcceptedNodeAdded;
        public event Action <QSQuestAcceptedNode> questAcceptedNodeRemoved;
        //public event Action <QSConditionNode> conditionNodeRemoved;
        //private SerializableDictionary<string, QS>

        //private SerializableDictionary<string, QSNodeErrorData> ungroupedNodes;
        //private SerializableDictionary<string, >

        
        private int nameErrorsAmount;
        public int NameErrorsAmount
        {
            get
            {
                return nameErrorsAmount;
            }

            set
            {
                nameErrorsAmount = value;

                if (nameErrorsAmount == 0)
                {
                    editorWindow.EnableSaving();
                }

                if (nameErrorsAmount == 1)
                {
                    editorWindow.DisableSaving();
                }
            }
        }
        

        public QSGraphView(QSEditorWindow qsEditorWindow)
        {
            editorWindow = qsEditorWindow;
            uniqueNodeNames = new HashSet<string>();
            AddManipulators();
            AddSearchWindow();
            AddGridBackground();
            OnElementsDeleted();
            //OnGraphViewChanged();

            //Change the stylesheet to use one specific for the quest system later
            AddStyles();
            graphViewChanged = OnGraphChanged;

        }

       


        #region Overrided Methods

        public override List<Port> GetCompatiblePorts(Port startPort, NodeAdapter nodeAdapter)
        {
            List<Port> compatiblePorts = new List<Port>();
            ports.ForEach(port =>
            {
                if (startPort == port)
                {
                    return;
                }

                if (startPort.node == port.node)
                {
                    return;
                }

                if (startPort.direction == port.direction)
                {
                    return;
                }
                
                compatiblePorts.Add(port);
            });
            return compatiblePorts;
        }

        #endregion


        #region Manipulators

        public void AddManipulators()
        {
            SetupZoom(ContentZoomer.DefaultMinScale, ContentZoomer.DefaultMaxScale);
            this.AddManipulator(new ContentDragger());
            this.AddManipulator(new SelectionDragger());
            this.AddManipulator(new RectangleSelector());
            
            this.AddManipulator(CreateNodeContextualMenu("Add Node (Quest Handler)",QSQuestNodeType.QuestHandler));
            this.AddManipulator(CreateNodeContextualMenu("Add Node (Activator)",QSQuestNodeType.Activator ));
            this.AddManipulator(CreateNodeContextualMenu("Add Node (DialogueGraph)",QSQuestNodeType.DialogueGraph ));
            this.AddManipulator(CreateNodeContextualMenu("Add Node (Condition)",QSQuestNodeType.Condition ));
            this.AddManipulator(CreateNodeContextualMenu("Add Node (QuestAccepted)",QSQuestNodeType.QuestAccepted ));
            this.AddManipulator(CreateNodeContextualMenu("Add Node (QuestActivator)",QSQuestNodeType.QuestActivator ));
            this.AddManipulator(CreateNodeContextualMenu("Add Node (ConditionSetter)",QSQuestNodeType.ConditionSetter ));
            
            
        }

        private IManipulator CreateNodeContextualMenu(string actionTitle, QSQuestNodeType questNodeType)
        {
            string nodeName = ""; 
            switch (questNodeType)
            {
                case QSQuestNodeType.Activator:
                {
                    nodeName = "ActivatorNodeName";
                    break;
                }
                case QSQuestNodeType.QuestHandler:
                {
                    nodeName = "QuestName";
                    break;
                }
                case QSQuestNodeType.DialogueGraph:
                {
                    nodeName = "DialogueGraphNodeName";
                    break;
                }
                case QSQuestNodeType.Condition:
                {
                    nodeName = "ConditionNodeName";
                    break;
                }
                case QSQuestNodeType.QuestAccepted:
                {
                    nodeName = "QuestAcceptedNodeName";
                    break;
                }
                case QSQuestNodeType.QuestActivator:
                {
                    nodeName = "QuestActivatorNodeName";
                    break;
                }
                case QSQuestNodeType.ConditionSetter:
                {
                    nodeName = "ConditionSetterNodeName";
                    break;
                }
            }

            ContextualMenuManipulator contextualMenuManipulator = new ContextualMenuManipulator(menuEvent =>
                menuEvent.menu.AppendAction(actionTitle,
                    actionEvent => AddElement(CreateNode(nodeName, questNodeType,
                        GetLocalMousePosition(actionEvent.eventInfo.localMousePosition)))));
            return contextualMenuManipulator;
        }

        #endregion

        #region ElementCreation

        public QSNode CreateNode(string nodeName,QSQuestNodeType questNodeType, Vector2 position, bool shouldDraw = true)
        {
            Type nodeType = Type.GetType($"QS.Elements.QS{questNodeType}Node");

            QSNode node = (QSNode)Activator.CreateInstance(nodeType);

            node.Initialize(nodeName, this, position);
            if (shouldDraw)
            {
                node.Draw();
            }
            return node;
        }
        
        #endregion
        #region Callbacks

        private void OnElementsDeleted()
        {
            deleteSelection = (operationName, askUser) =>
            {
                Type edgeType = typeof(Edge);
                List<Edge> edgesToDelete = new List<Edge>();
                
                
                List<QSNode> nodesToDelete = new List<QSNode>();
                foreach (GraphElement element in selection)
                {
                    if (element is QSNode node)
                    {
                        nodesToDelete.Add(node);
                        
                        continue;
                    }

                    if (element.GetType() == edgeType)
                    {
                        Edge edge = (Edge)element;
                        edgesToDelete.Add(edge);
                        continue;
                    }
                }
                DeleteElements(edgesToDelete);
                
                foreach (QSNode node in nodesToDelete)
                {
                    Type type = node.GetType();
                    if (type == typeof(QSQuestHandlerNode))
                    {
                        var questHandlerNode = (QSQuestHandlerNode)node;
                        questHandlerRemoved?.Invoke(questHandlerNode);
                    }
                    else if (type == typeof(QSConditionNode))
                    {
                        var conditionNode = (QSConditionNode)node;
                        //conditionNodeRemoved?.Invoke(conditionNode);
                    }
                    uniqueNodeNames.Remove(node.NodeName);
                    node.DisconnectAllPorts();
                    node.OnDestroy(this);
                    RemoveElement(node);
                }
            };
        }
        #region Events
        
        
        public void SendQuestHandlerAddedEvent(QSQuestHandlerNode questHandler)
        {
            var nodes = this.nodes;
            questHandlerAdded?.Invoke(questHandler);
        }
        
        public void SendQuestHandlerRemovedEvent(QSQuestHandlerNode questHandler)
        {
            questHandlerRemoved?.Invoke(questHandler);
        }

        public void SendConditionNodeAddedEvent(QSConditionNode conditionNode)
        {
            conditionNodeAdded?.Invoke(conditionNode);
        }
        public void SendConditionNodeRemovedEvent(QSConditionNode conditionNode)
        {
            conditionNodeRemoved?.Invoke(conditionNode);
        }
        
        public void SendQuestAcceptedNodeAddedEvent(QSQuestAcceptedNode questAcceptedNode)
        {
            questAcceptedNodeAdded?.Invoke(questAcceptedNode);
        }
        public void SendQuestAcceptedNodeRemovedEvent(QSQuestAcceptedNode questAcceptedNode)
        {
            questAcceptedNodeRemoved?.Invoke(questAcceptedNode);
        }

        public void SendConditionSetterNodeAddedEvent(QSConditionSetterNode conditionSetterNode)
        {
            conditionSetterNodeAdded?.Invoke(conditionSetterNode);
        }
        
        

       

        public void ClearSubscriptions()
        {
            questHandlerAdded = null;
            questHandlerRemoved = null;
            questAcceptedNodeAdded = null;
            questAcceptedNodeRemoved = null;
            conditionNodeAdded = null;
            conditionNodeRemoved = null;
            conditionSetterNodeAdded = null;
            //conditionSetterNodeRemoved = null;
        }
        
        #endregion
        
        
        //Warning, this callback does not seem to factor in changes to nodes even though they are visual elements.
        //This is asinine since the obvious assumption is that the callback checks changes to all kinds of elements.
        //In true Unity fashion.
        private GraphViewChange OnGraphChanged(GraphViewChange changes)
        {
            if (changes.edgesToCreate != null)
            {
                foreach (Edge edge in changes.edgesToCreate)
                {
                    QSNode nextNode = (QSNode)edge.input.node;
                    QSBranchSaveData branchData = (QSBranchSaveData)edge.output.userData;
                    var node = edge.output.node;
                    branchData.NodeID = nextNode.ID;
                    /*if (node.GetType() == typeof(QSDialogueGraphNode))
                    {
                        QSDialogueGraphNode graphNode = (QSDialogueGraphNode)node;
                        //Debug.Log(graphNode.branches);
                        
                    }
                    if (node.GetType() == typeof(QSQuestHandlerNode))
                    {
                        QSQuestHandlerNode questHandlerNode = (QSQuestHandlerNode)node;
                        //Debug.Log(questHandlerNode.branches);
                        
                    }*/

                }
            }

            if (changes.elementsToRemove != null)
            {
                Type edgeType = typeof(Edge);

                /*var oldNodes = new List<QSNode>();
                graphElements.ForEach(graphElement =>
                {
                    //Uses pattern matching, what's that?
                    if (graphElement is QSNode node)
                    {
                        oldNodes.Add(node);
                        return;
                    }
                });*/
                
                foreach (GraphElement element in changes.elementsToRemove)
                {
                    if (element.GetType() != edgeType)
                    {
                        continue;
                    }

                    Edge edge = (Edge)element;

                    var node = edge.output.node;
                    var qsNode = node as QSNode;

                    QSBranchSaveData branchData = (QSBranchSaveData) edge.output.userData;

                    branchData.NodeID = "";
                    
                    
                }
                //var nodes = oldNodes;
            }
            
            if (changes.elementsToRemove != null)
            {
                
            }

            return changes;
        }
        

        #endregion
        
        //#region Repeated Elements
        

        //#endregion
        
        #region ElementAddition

        private void AddSearchWindow()
        {
            if (searchWindow == null)
            {
                searchWindow = ScriptableObject.CreateInstance<QSSearchWindow>();
                searchWindow.Initialize(this);
            }

            nodeCreationRequest = context =>
                SearchWindow.Open(new SearchWindowContext(context.screenMousePosition), searchWindow);
        }
        
        private void AddGridBackground()
        {
            GridBackground gridBackground = new GridBackground();
            
            gridBackground.StretchToParentSize();
            
            Insert(0,gridBackground);
        }
        
        private void AddStyles()
        {
        
            //Change the stylesheet to use one specific for the quest system later
            this.AddStyleSheets(
                "QuestSystem/QSGraphViewStyles.uss",
                "QuestSystem/QSNodeStyles.uss"
            );
        }
        
        #endregion
        
        #region Utilities
        
        
        

        public Vector2 GetLocalMousePosition(Vector2 mousePosition, bool isSearchWindow = false)
        {
            Vector2 worldMousePosition = mousePosition;
            if (isSearchWindow)
            {
                worldMousePosition -= editorWindow.position.position;
            }
            Vector2 localMousePosition = contentViewContainer.WorldToLocal(worldMousePosition);

            return localMousePosition;
        }
        
        public void ClearGraph()
        {
            graphElements.ForEach(graphElement => RemoveElement(graphElement));
            uniqueNodeNames.Clear();
            
            NameErrorsAmount = 0;
        }
        
        #endregion
    }
}

