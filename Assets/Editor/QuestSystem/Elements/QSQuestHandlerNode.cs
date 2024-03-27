using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
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
    public class QSQuestHandlerNode : QSNode
    {
        private VisualElement textBoxContainer;
        //List<PropertyField> propertyFields;
        public QuestHandler questHandler;
        
        
        public override void Initialize(string nodeName, QSGraphView dsGraphView, Vector2 position)
        {
            QuestNodeType = QSQuestNodeType.QuestHandler;
            base.Initialize(nodeName, dsGraphView, position);
            propertyFields = new List<PropertyField>();
           
            dsGraphView.conditionNodeAdded += OnConditionNodeAdded;
            dsGraphView.questAcceptedNodeAdded += OnQuestAcceptedNodeAdded;
            dsGraphView.conditionSetterNodeAdded += OnQuestConditionSetterNodeAdded;

            QSBranchSaveData branchSaveData = new QSBranchSaveData()
            {
                Text = "NextNode"
            };
            
            branches.Add(branchSaveData);
            SendQuestHandlerAddedEvent(this);
           

        }

        private void OnQuestConditionSetterNodeAdded(QSConditionSetterNode qsConditionSetterNode)
        {
            SendQuestHandlerAddedEvent(this);
        }

        private void OnQuestAcceptedNodeAdded(QSQuestAcceptedNode questAcceptedNode)
        {
            SendQuestHandlerAddedEvent(this);
        }

        //I can't remember why I need to send this, but I'm sure its for a good reason.
        public void OnConditionNodeAdded(QSConditionNode conditionNode)
        {
            SendQuestHandlerAddedEvent(this);
        }

        public override void OnDestroy(QSGraphView qsGraphView)
        {
            qsGraphView.conditionNodeAdded -= OnConditionNodeAdded;
            qsGraphView.questAcceptedNodeAdded -= OnQuestAcceptedNodeAdded;
            qsGraphView.conditionSetterNodeAdded -= OnQuestConditionSetterNodeAdded;

        }

        public override void Draw()
        {
            //base.Draw();

            //That's right, we wanted to figure out if there's a way to get a callback on deselecting the text field
            TextField dialogueNameTextField = QSElementUtility.CreateTextField(NodeName, null,callback =>
            {
                TextField target = (TextField)callback.target;
                
            });
            oldNameText = NodeName;
            
            
            dialogueNameTextField.RegisterCallback<FocusOutEvent>(callback =>
            {
                
                TextField target = (TextField)callback.target;
                if (target.value == NodeName)
                {
                    
                }
                else
                {
                    var oldValue = NodeName;
                    graphView.uniqueNodeNames.Remove(oldValue);
                    var newValue = CheckForDuplicateNames(target.value, graphView);
               
                    target.value = newValue;
                    NodeName = newValue;
                    
                    /*if (string.IsNullOrEmpty(target.value))
                    {
                        if (!string.IsNullOrEmpty(NodeName))
                        {
                            ++graphView.NameErrorsAmount;
                        }
                    }
                    else
                    {
                        if (string.IsNullOrEmpty(NodeName))
                        {
                            --graphView.NameErrorsAmount;
                        }
                    }*/
                    
                }
                Debug.Log("TextField no longer in focus");
               
                
            });

            dialogueNameTextField.AddClasses(
                "ds-node__textfield",
                "ds-node__filename-textfield",
                "ds-node__textfield__hidden"
            );


            titleContainer.Insert(0,dialogueNameTextField);
            /*Port inputPort = this.CreatePort("Quest Connection", Orientation.Horizontal, Direction.Input,
                Port.Capacity.Multi);
            
            inputPort.portName = "Quest Connection";
            inputContainer.Add(inputPort);*/

            customDataContainer = new VisualElement();
            
            customDataContainer.AddToClassList("ds-node__custom-data-container");
            
            VisualElement selection = this.Q("selection-border", (string)null);


            if (selection != null)
            {
                selection.AddClasses("qs-node__selection");
            }
            
            extensionContainer.Add(customDataContainer);
           
            RefreshExpandedState();
            
            
            /*foreach (QSBranchSaveData branch in branches)
            {
                Port choicePort = this.CreatePort(branch.Text);
                choicePort.userData = branch;
                outputContainer.Add(choicePort);
            }*/
            propertyFields.Clear();
            ObjectField testValueObjectField = QSElementUtility.CreateQuestHandlerField(callback =>
            {
                
                if (textBoxContainer != null)
                {
                    customDataContainer.Remove(textBoxContainer);
                }
                
                textBoxContainer = new VisualElement();
                customDataContainer.Add(textBoxContainer);
                propertyFields.Clear();
                
                ObjectField target = (ObjectField)callback.target;
                
                questHandler = (QuestHandler)target.value;
                
                SendQuestHandlerAddedEvent(this);
                
                if (questHandler != null)
                {
                    SerializedObject questHandlerSerialized = new SerializedObject(questHandler);
                    //Debug.Log("Called on draw!");
                    
                    //var questHandler = (QuestHandler)testValueObjectField.value;
                    var properties = questHandler.GetType().GetFields();
                    List<string> propertyNames = new List<string>();
                    foreach (var property in properties)
                    {
                        propertyNames.Add(property.Name);
                    }

                    foreach (var name in propertyNames)
                    {
                        var property = questHandlerSerialized.FindProperty(name);
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
                        
                        //extensionContainer.Add(propertyField);
                    }
                    //graphView.editorWindow.rootVisualElement.Bind(questHandlerSerialized);
                  
                   
                  
                    
                }
                RefreshExpandedState();
            });
            
            //No idea if this will work. It should be calling draw only on creation.
            if (questHandler != null)
            {
                
                if (textBoxContainer != null)
                {
                    //customDataContainer.Remove(textBoxContainer);
                    customDataContainer.Clear();
                }
            
                textBoxContainer = new VisualElement();
                customDataContainer.Add(testValueObjectField);
                testValueObjectField.value = questHandler;
                SendQuestHandlerAddedEvent(this);
                customDataContainer.Add(textBoxContainer);
                propertyFields.Clear();
            
            
                SerializedObject questHandlerSerialized = new SerializedObject(questHandler);
                //var questHandler = (QuestHandler)testValueObjectField.value;
                var properties = questHandler.GetType().GetFields();
                List<string> propertyNames = new List<string>();
                foreach (var property in properties)
                {
                    propertyNames.Add(property.Name);
                }

                foreach (var name in propertyNames)
                {
                    var property = questHandlerSerialized.FindProperty(name);
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
                        
                    //extensionContainer.Add(propertyField);
                }
            
                
                testValueObjectField.AddClasses("qs-node__textfield",
                    "qs-node__filename-textfield",
                    "qs-node__textfield__hidden");

                
            }
            RefreshExpandedState();

            customDataContainer.Add(testValueObjectField);
        }

        public void SendQuestHandlerAddedEvent(QSQuestHandlerNode questHandlerNode)
        {
            graphView.SendQuestHandlerAddedEvent(questHandlerNode); 
        }
    }

}
