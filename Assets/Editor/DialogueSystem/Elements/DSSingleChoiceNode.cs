using System.Collections;
using System.Collections.Generic;
using DS.Data.Save;
using DS.Enumerations;
using DS.Utilities;
using DS.Windows;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

namespace DS.Elements
{
    public class DSSingleChoiceNode : DSNode
    {
        public override void Initialize(string nodeName, DSGraphView dsGraphView, Vector2 position)
        {
            base.Initialize(nodeName, dsGraphView,position);

            DialogueType = DSDialogueType.SingleChoice;

            DSChoiceSaveData choiceData = new DSChoiceSaveData()
            {
                Text = ("NextDialogue")
            };
           
            

           
            
            choices.Add(choiceData);
            
        }

        public override void Draw()
        {
            base.Draw();
            /* OUTPUT CONTAINER */
            foreach (DSChoiceSaveData choice in choices)
            {
                Port choicePort = this.CreatePort(choice.Text);
                choicePort.userData = choice;
                outputContainer.Add(choicePort);
            }
            
            RefreshExpandedState();
        }
    }
}

