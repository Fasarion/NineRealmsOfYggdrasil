using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Codice.Client.Common.TreeGrouper;
using DS.ScriptableObjects;
using DS.Utilities;
using KKD;
using QS.Data.Save;
using QS.Enumerations;
using QS.Utilities;
using QS.Windows;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace QS.Elements
{
    public class QSDialogueGraphNode : QSNode
    {
        private VisualElement textBoxContainer;
        //List<PropertyField> propertyFields;
        private ConvertDialogueSOToQSDialogueNodeSO dialogueSoToQuestNodeSoConverter;
        public DSDialogueContainerSO dialogueContainerSO;
        private List<QSBranchSaveData> branchesToStore;
        private List<Port> currentPorts;
        public override void Initialize(string nodeName, QSGraphView qsGraphView, Vector2 position)
        {
            //nodeName = CheckForDuplicateNames(nodeName, qsGraphView);
            
            base.Initialize(nodeName, qsGraphView, position);
            QuestNodeType = QSQuestNodeType.DialogueGraph;
            //dialogueSoToQuestNodeSoConverter = AssetUtility.TryLoad();
            
            propertyFields = new List<PropertyField>();
            branchesToStore = new List<QSBranchSaveData>();
            //Might be able to simplify this, but it should work for now.


            //dialogueSoToQuestNodeSoConverter = QSIOUtility.LoadConvertedNodeDataFromScriptableObject(NodeName);
            //dialogueSoToQuestNodeSoConverter = QSIOUtility.LoadConvertedNodeDataFromScriptableObject(nodeName);
        }
        
        
       

       /* public void DeleteDataConverterSO()
        {
            
            QSIOUtility.DeleteConvertedNodeDataScriptableObject(NodeName);
        }*/

       
        
        


        

        public override void Draw()
        {
            base.Draw();
            propertyFields.Clear();
            //outputContainer.Clear();
            ObjectField testValueObjectField = QSElementUtility.CreateDialogueGraphField(callback =>
            {
                
                
                if (textBoxContainer != null)
                {
                    customDataContainer.Remove(textBoxContainer);
                }
                
                textBoxContainer = new VisualElement();
                customDataContainer.Add(textBoxContainer);
                propertyFields.Clear();
                ObjectField target = (ObjectField)callback.target;
                dialogueContainerSO = (DSDialogueContainerSO)target.value;
                
                
                
                if (dialogueContainerSO != null)
                {
                    SerializedObject dialogueContainerSerialized = new SerializedObject(dialogueContainerSO);
                    var fields = dialogueContainerSO.GetType().GetFields();
                    List<string> propertyNames = new List<string>();
                    foreach (var property in fields)
                    {
                        propertyNames.Add(property.Name);
                    }
                    
                    foreach (var name in propertyNames)
                    {
                        var property = dialogueContainerSerialized.FindProperty(name);
                        PropertyField testValuePropertyField = QSElementUtility.CreatePropertyField(property, callback =>
                        {
                            PropertyField target = (PropertyField)callback.target;
                        });

                        
                        testValuePropertyField.name = name;
                        testValuePropertyField.visible = true;
                        testValuePropertyField.BindProperty(property);
                        propertyFields.Add(testValuePropertyField);
                        
                    }
                    
                    foreach (var propertyField in propertyFields)
                    {
                        propertyField.AddClasses("qs-node__textfield",
                            "qs-node__filename-textfield",
                            "qs-node__textfield__hidden");
                        textBoxContainer.Add(propertyField);
                        
                       
                    }
                    foreach (var visualElement in outputContainer.Children())
                    {
                        if (visualElement.GetType() == typeof(Port))
                        {
                            var port = (Port)visualElement;
                            foreach (var edge in port.connections)
                            {
                                edge.input.Disconnect(edge);
                                
                                edge.parent.Remove(edge);
                            }
                        }
                    }
                    
                    outputContainer.Clear();
                    branches.Clear();
                    
                    foreach (var dialogueSo in dialogueContainerSO.exitDialogues)
                    {
                        QSBranchSaveData saveData = new QSBranchSaveData()
                        {
                            Text = dialogueSo.DialogueName

                        };
                        branches.Add(saveData);
                        Port choicePort = this.CreatePort(dialogueSo.DialogueName);
                        choicePort.userData = saveData;
                        //currentPorts.Add(choicePort);
                        outputContainer.Add(choicePort);
                    }
                    
                    RefreshExpandedState();
                    
                }
                else
                {
                    
                    //Maybe this is the actual source of the problem?
                    outputContainer.Clear();
                    branches.Clear();
                }
            });

            testValueObjectField.AddClasses("qs-node__textfield",
                "qs-node__filename-textfield",
                "qs-node__textfield__hidden");


            if (dialogueContainerSO != null)
            {
                if (textBoxContainer != null)
                {
                    //Might want to double check that you're clearing the right thing here
                    customDataContainer.Clear();
                    //customDataContainer.Remove(textBoxContainer);
                }

                textBoxContainer = new VisualElement();
                customDataContainer.Add(testValueObjectField);
                testValueObjectField.value = dialogueContainerSO;
                customDataContainer.Add(textBoxContainer);
                propertyFields.Clear();

                //ObjectField target = (ObjectField)callback.target;
                //dialogueContainerSO = (DSDialogueContainerSO)target.Value;
                SerializedObject dialogueContainerSerialized = new SerializedObject(dialogueContainerSO);
                
                

                var fields = dialogueContainerSO.GetType().GetFields();
                List<string> propertyNames = new List<string>();
                foreach (var property in fields)
                {
                    propertyNames.Add(property.Name);
                }

                foreach (var name in propertyNames)
                {
                    var property = dialogueContainerSerialized.FindProperty(name);
                    PropertyField testValuePropertyField = QSElementUtility.CreatePropertyField(property, callback =>
                    {
                        PropertyField target = (PropertyField) callback.target;
                    });


                    testValuePropertyField.name = name;
                    testValuePropertyField.visible = true;
                    testValuePropertyField.BindProperty(property);
                    propertyFields.Add(testValuePropertyField);

                }

                foreach (var propertyField in propertyFields)
                {
                    propertyField.AddClasses("qs-node__textfield",
                        "qs-node__filename-textfield",
                        "qs-node__textfield__hidden");
                    textBoxContainer.Add(propertyField);


                }

                //QSIOUtility.LoadNodesConnections();
               
                //outputContainer.Clear();
                //branches.Clear();

                /*if (branches.Count > 0)
                {
                    
                }*/
                if (branches.Count > 0)
                {
                    foreach (var branch in branches)
                    {

                        if (branch.NodeID == null)
                        {
                            continue;
                        }
                        QSBranchSaveData saveData = new QSBranchSaveData()
                        {
                            Text = branch.Text,
                            NodeID = branch.NodeID
                        };
                    
                    
                        //branches.Add(saveData);
                        Port choicePort = this.CreatePort(branch.Text); //this.CreateBranchPort(saveData);
                        choicePort.userData = branch;
                        //choicePort.userData = saveData;
                        outputContainer.Add(choicePort);
                    }
                }
            }
            else
            {
                outputContainer.Clear();
                branches.Clear();
                if (textBoxContainer != null)
                {
                    customDataContainer.Remove(textBoxContainer);
                }
            }
            RefreshExpandedState();
            if (!customDataContainer.Contains(testValueObjectField))
            {
                customDataContainer.Add(testValueObjectField);
            }
            
            
        }

        public void RedrawBranches()
        {

            
            /*foreach (var branch in branches)
            {
                QSNode nextNode = QSIOUtility.loadedNodes[branch.NodeID];
                
                //Port nextNodeInputPort = (Port) nextNode.inputContainer.Children().First();
                //var node = nextNodeInputPort.node;
                
                
                
            }
            
            
            var outputPort = (Port) outputContainer.Children();

            foreach (var element in outputContainer.Children())
            {
                Port port = (Port) element;
                Edge edge = port.ConnectTo()
                graphView.AddElement(edge);
            }*/
        }
        
        
        private Port CreateBranchPort(object userData)
        {
            //Port choicePort = this.CreatePort();

            //choicePort.userData = userData;

            QSBranchSaveData branchData = (QSBranchSaveData)userData;
            Port choicePort = this.CreatePort(branchData.Text);
            choicePort.userData = userData;
            //outputContainer.Add(choicePort);
            return choicePort;
        }
    }
}

