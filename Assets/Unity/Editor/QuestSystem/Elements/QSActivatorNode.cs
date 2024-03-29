using System;
using System.Collections;
using System.Collections.Generic;
using QS.Data.Save;
using QS.Elements;
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
    public class QSActivatorNode : QSNode
    {
        public List<string> gameObjectsToActivateNames;
        public List<string> gameObjectsToDeactivateNames;
        public List<TextField> activeObjectFields;
        public List<TextField> inactiveObjectFields;
        //public List<TextField> fields;
        private VisualElement activeObjectFieldContainer;
        private VisualElement inactiveObjectFieldContainer;

        public override void Initialize(string nodeName, QSGraphView qsGraphView, Vector2 position)
        {
            base.Initialize(nodeName, qsGraphView, position);
            QuestNodeType = QSQuestNodeType.Activator;
            //gameObjectNames = new List<string>() {"Unity", "Is", "Stupid"};
            activeObjectFields = new List<TextField>();
            inactiveObjectFields = new List<TextField>();
            
            QSBranchSaveData branchSaveData = new QSBranchSaveData()
            {
                Text = "NextNode"
            };
            
            branches.Add(branchSaveData);
        }

        public override void Draw()
        {
            base.Draw();
            
            foreach (QSBranchSaveData branch in branches)
            {
                Port choicePort = this.CreatePort(branch.Text);
                choicePort.userData = branch;
                outputContainer.Add(choicePort);
                
            }
            
            DrawActiveObjectInterface();
            DrawInactiveObjectInterface();
            
            
            //inactiveObjectFieldContainer = new VisualElement();
            
            /*Func<VisualElement> makeItem = () => new Label();
            Action<VisualElement, int> bindItem = (e, i) => (e as Label).text = gameObjectNames[i];

            ListView view = new ListView(gameObjectNames, 16, makeItem, bindItem )
            {
                selectionType = SelectionType.Multiple
            };
            customDataContainer.Add(view);*/
            RefreshExpandedState();
        }

        public void DrawActiveObjectInterface()
        {
            
            activeObjectFieldContainer = new VisualElement();
           
            Button button = QSElementUtility.CreateButton("Add Object To Activate", () =>
            {
                var field = CreateTextField("", activeObjectFields, activeObjectFieldContainer);
                RefreshExpandedState();
                //fields.Add(field);
            });
            customDataContainer.Add(button);
            customDataContainer.Add(activeObjectFieldContainer);

            

            if (gameObjectsToActivateNames != null)
            {
                
                activeObjectFields.Clear();
                activeObjectFieldContainer.Clear();
                foreach (var name in gameObjectsToActivateNames)
                {
                    CreateTextField(name, activeObjectFields, activeObjectFieldContainer);
                }
            }
        }

        public void DrawInactiveObjectInterface()
        {
            
            inactiveObjectFieldContainer = new VisualElement();
           
            Button button = QSElementUtility.CreateButton("Add Object To Deactivate", () =>
            {
                var field = CreateTextField("", inactiveObjectFields, inactiveObjectFieldContainer);
                RefreshExpandedState();
                //fields.Add(field);
            });
            customDataContainer.Add(button);
            customDataContainer.Add(inactiveObjectFieldContainer);

            

            if (gameObjectsToDeactivateNames != null)
            {
                
                inactiveObjectFields.Clear();
                inactiveObjectFieldContainer.Clear();
                foreach (var name in gameObjectsToDeactivateNames)
                {
                    CreateTextField(name, inactiveObjectFields, inactiveObjectFieldContainer);
                }
            }
        }
        
        public TextField CreateTextField(string value, List<TextField> objectFields, VisualElement fieldContainer)
        {
            TextField field = QSElementUtility.CreateTextField(value,"Object", callback=>
            {
                TextField target = (TextField) callback.target;
                    target.value = callback.newValue;


                //Might be able to mimic what I did to the text fields that I added to the dialogue node container.
            });
          
            
            Button deleteButton = QSElementUtility.CreateButton("X", () =>
            {
                objectFields.Remove(field);
                fieldContainer.Remove(field);
            });
            field.Add(deleteButton);
            field.labelElement.style.minWidth = 50;
            objectFields.Add(field);
            fieldContainer.Add(field);
          
            return field;
        }

        public List<string> GetObjectsToActivateNames()
        {
           return GetTextFieldValues(out gameObjectsToActivateNames, activeObjectFields);
           
        }

        public List<string> GetObjectsToDeactivateNames()
        {
            return GetTextFieldValues(out gameObjectsToDeactivateNames, inactiveObjectFields);
            
        }
        public List<string> GetTextFieldValues(out List<string> objectsToAlterNames, List<TextField> objectFields)
        {
            objectsToAlterNames = new List<string>();
            var objectNames =  new List<string>();
            foreach (var field in objectFields)
            {
                objectNames.Add(field.value);
                objectsToAlterNames.Add(field.value);
            }

            return objectNames;
        }
    }
    
   
}