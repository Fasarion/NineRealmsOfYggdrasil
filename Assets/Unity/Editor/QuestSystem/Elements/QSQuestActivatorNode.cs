using System.Collections;
using System.Collections.Generic;
using KKD;
using QS.Data.Save;
using QS.Elements;
using QS.Enumerations;
using QS.Utilities;
using QS.Windows;
using UnityEditor.Experimental.GraphView;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace QS.Elements
{
    public class QSQuestActivatorNode : QSNode
    {
        public QuestHandler questHandler;
        private VisualElement textBoxContainer;
        public override void Initialize(string nodeName, QSGraphView qsGraphView, Vector2 position)
        {
            base.Initialize(nodeName, qsGraphView, position);
            QuestNodeType = QSQuestNodeType.QuestActivator;
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
            ObjectField testValueObjectField = QSElementUtility.CreateQuestHandlerField(callback =>
            {
                if (textBoxContainer != null)
                {
                    customDataContainer.Remove(textBoxContainer);
                }
                
                textBoxContainer = new VisualElement();
                customDataContainer.Add(textBoxContainer);

                ObjectField target = (ObjectField)callback.target;
                
                questHandler = (QuestHandler)target.value;
                
                RefreshExpandedState();
            });



            if (questHandler != null)
            {
                testValueObjectField.value = questHandler;
            }

            customDataContainer.Add(testValueObjectField);
            RefreshExpandedState();
            
        }
    }
}
