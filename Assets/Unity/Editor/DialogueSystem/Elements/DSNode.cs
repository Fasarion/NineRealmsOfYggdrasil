using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DS.Data.Save;
using DS.Enumerations;
using DS.Utilities;
using DS.Windows;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

namespace DS.Elements
{
    public class DSNode : Node
    {
        public string ID { get; set; }
        public string DialogueName { get; set; }

        public List<DSChoiceSaveData> choices;
        public string Text { get; set; }

        public DSDialogueType DialogueType { get; set; }
        public DSGroup Group { get; set; }

        protected DSGraphView graphView;
        private Color defaultBackgroundColor;

        public virtual void Initialize(string nodeName, DSGraphView dsGraphView, Vector2 position)
        {
            ID = Guid.NewGuid().ToString();
            DialogueName = nodeName;
            choices = new List<DSChoiceSaveData>();
            Text = "Dialogue text.";

            graphView = dsGraphView;
            defaultBackgroundColor = new Color(29f/255f,29f/255f,30f/255f);
            
            SetPosition(new Rect(position, Vector2.zero));
            
            mainContainer.AddToClassList("ds-node__main-container");
            extensionContainer.AddToClassList("ds-node__extension-container");
        }
        
        public void Initialize(string nodeName, Vector2 position)
        {
            ID = Guid.NewGuid().ToString();
            DialogueName = nodeName;
            choices = new List<DSChoiceSaveData>();
            Text = "Dialogue text."; 
            defaultBackgroundColor = new Color(29f/255f,29f/255f,30f/255f);
            
            SetPosition(new Rect(position, Vector2.zero));
            
            mainContainer.AddToClassList("ds-node__main-container");
            extensionContainer.AddToClassList("ds-node__extension-container");
        }

        
        

        public virtual void Draw()
        {
            /*TITLE CONTAINER*/
            //not sure how the callback thing works to be honest.
            TextField dialogueNameTextField = DSElementUtility.CreateTextField(DialogueName, null,callback =>
            {
                TextField target = (TextField) callback.target;
                target.value = callback.newValue.RemoveWhitespaces().RemoveSpecialCharacters();

                if (string.IsNullOrEmpty(target.value))
                {
                    if (!string.IsNullOrEmpty(DialogueName))
                    {
                        ++graphView.NameErrorsAmount;
                        
                    }
                }
                else
                {
                    if (string.IsNullOrEmpty(DialogueName))
                    {
                        --graphView.NameErrorsAmount;
                    }
                }
                
                if (Group == null)
                {
                    graphView.RemoveUngroupedNode(this);

                    DialogueName = target.value;
                
                    graphView.AddUngroupedNode(this);
                    return;
                }

                DSGroup currentGroup = Group;
                
                graphView.RemoveGroupedNode(this, Group);
                DialogueName = target.value;
                
                graphView.AddGroupedNode(this, currentGroup);

            });
            dialogueNameTextField.AddClasses(
                "ds-node__textfield",
                "ds-node__filename-textfield",
                "ds-node__textfield__hidden"
            );



            /* INPUT CONTAINER */
            titleContainer.Insert(0, dialogueNameTextField);
            Port inputPort = this.CreatePort("Dialogue Connection", Orientation.Horizontal, Direction.Input,
                Port.Capacity.Multi);
            
            
            inputPort.portName = "Dialogue Connection";
            inputContainer.Add(inputPort);

            VisualElement customDataContainer = new VisualElement();
            
            customDataContainer.AddToClassList("ds-node__custom-data-container");

            Foldout textFoldout = DSElementUtility.CreateFoldout("Dialogue Text");


            TextField textTextField = DSElementUtility.CreateTextArea(Text, null, callback =>
            {
                Text = callback.newValue;
            });


            textTextField.AddClasses(
                "ds-node__textfield",
                "ds-node__quote-textfield"
                );


            textFoldout.Add(textTextField);

            customDataContainer.Add(textFoldout);
            extensionContainer.Add(customDataContainer);
            
        }

        #region Overrided Methods

        public override void BuildContextualMenu(ContextualMenuPopulateEvent evt)
        {
            evt.menu.AppendAction("Disconnect Input Ports", actionEvent => DisconnectInputPorts());
            evt.menu.AppendAction("Disconnect Output Ports", actionEvent => DisconnectOutputPorts());
            base.BuildContextualMenu(evt);
        }


        #endregion
        
        #region Utility Methods

        public void DisconnectAllPorts()
        {
            DisconnectPorts(inputContainer);
            DisconnectPorts(outputContainer);
        }
        
        
        
        private void DisconnectInputPorts()
        {
            DisconnectPorts(inputContainer);
        }
        
        private void DisconnectOutputPorts()
        {
            DisconnectPorts(outputContainer);
        }
        
        private void DisconnectPorts(VisualElement container)
        {
            foreach (Port port in container.Children())
            {
                if (!port.connected)
                {
                    continue;
                }
                graphView.DeleteElements(port.connections);
            }
        }

        public bool IsStartingNode()
        {
            Port inputPort = (Port)inputContainer.Children().First();
            return !inputPort.connected;

        }

        public void SetErrorStyle(Color color)
        {
            mainContainer.style.backgroundColor = color;
            
        }

        public void ResetStyle()
        {
            mainContainer.style.backgroundColor = defaultBackgroundColor;
        }
        #endregion
    }
}

