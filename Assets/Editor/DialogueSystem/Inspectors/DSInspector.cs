
using System.Collections.Generic;
using DS.ScriptableObjects;
using DS.Utilities;
using UnityEditor;


namespace DS.Inspectors
{
   
    [CustomEditor(typeof(DSDialogue))]
    public class DSInspector : Editor
    {
        /*Dialogue Scriptable Objects */
        private SerializedProperty dialogueContainerProperty;
        private SerializedProperty dialogueGroupProperty;
        private SerializedProperty dialogueProperty;

        /*Filters*/
        private SerializedProperty groupedDialoguesProperty;
        private SerializedProperty startingDialoguesOnlyProperty;

        /*Indexes*/

        private SerializedProperty selectedDialogueGroupIndexProperty;
        private SerializedProperty selectedDialogueIndexProperty;
        
        private void OnEnable()
        {
            dialogueContainerProperty = serializedObject.FindProperty("dialogueContainer");
            dialogueGroupProperty = serializedObject.FindProperty("dialogueGroup");
            dialogueProperty = serializedObject.FindProperty("dialogue");
            
            groupedDialoguesProperty = serializedObject.FindProperty("groupedDialogues");
            startingDialoguesOnlyProperty = serializedObject.FindProperty("startingDialoguesOnly");
            
            selectedDialogueGroupIndexProperty = serializedObject.FindProperty("selectedDialogueGroupIndex");
            selectedDialogueIndexProperty = serializedObject.FindProperty("selectedDialogueIndex");
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            
            DrawDialogueContainerArea();

            DSDialogueContainerSO dialogueContainer = (DSDialogueContainerSO)dialogueContainerProperty.objectReferenceValue;

            if (dialogueContainer == null)
            {
                StopDrawing("Select a Dialogue Container to see the rest of the Inspector");
                return;
            }
            DrawFiltersArea();

            bool currentStartingDialoguesOnlyFilter = startingDialoguesOnlyProperty.boolValue;

            List<string> dialogueNames;

            string dialogueFolderPath = $"Assets/DialogueSystem/Dialogues/{dialogueContainer.FileName}";

            string dialogueInfoMessage; 

            if (groupedDialoguesProperty.boolValue)
            {
                List<string> dialogueGroupNames = dialogueContainer.GetDialogueGroupNames();
                if (dialogueGroupNames.Count == 0)
                {
                    StopDrawing("There are no Dialogue Groups in this Dialogue Container.");
                    return;
                }
                DrawDialogueGroupArea(dialogueContainer, dialogueGroupNames);
                DSDialogueGroupSO dialogueGroup = (DSDialogueGroupSO)dialogueGroupProperty.objectReferenceValue;
                dialogueNames = dialogueContainer.GetGroupedDialogueNames(dialogueGroup, currentStartingDialoguesOnlyFilter);

                dialogueFolderPath += $"/Groups/{dialogueGroup.GroupName}/Dialogues";

                dialogueInfoMessage = "There are no " + (currentStartingDialoguesOnlyFilter ? "Starting" : "") +  " Dialogues in this Dialogue Group.";
            }
            else
            {
                dialogueNames = dialogueContainer.GetUngroupedDialogueNames(currentStartingDialoguesOnlyFilter);
                dialogueFolderPath += "/Global/Dialogues";
                dialogueInfoMessage = "There are no " + (currentStartingDialoguesOnlyFilter ? "Starting" : "") + " Ungrouped Dialogues in this Dialogue Container";
            }

            if (dialogueNames.Count == 0)
            {
                StopDrawing(dialogueInfoMessage);
                return;
            }

         
            DrawDialogueArea(dialogueNames, dialogueFolderPath);

            serializedObject.ApplyModifiedProperties();
        }

        

        #region Draw Methods

        private void DrawDialogueContainerArea()
        {
            DSInspectorUtility.DrawHeader("Dialogue Container");
            dialogueContainerProperty.DrawPropertyField();

            EditorGUILayout.Space(4);
            

        }
        
        
        private void DrawFiltersArea()
        {
            DSInspectorUtility.DrawHeader("Filters");
            
            groupedDialoguesProperty.DrawPropertyField();
            startingDialoguesOnlyProperty.DrawPropertyField();

            DSInspectorUtility.DrawSpace();
        }
        
        private void DrawDialogueGroupArea(DSDialogueContainerSO dialogueContainer, List<string> dialogueGroupNames)
        { 
            DSInspectorUtility.DrawHeader("Dialogue Group");

            int oldSelectedDialogueGroupIndex = selectedDialogueGroupIndexProperty.intValue;

            DSDialogueGroupSO oldDialogueGroup = (DSDialogueGroupSO)dialogueGroupProperty.objectReferenceValue;

            bool isOldDialogueGroupNull = oldDialogueGroup == null;

            string oldDialogueGroupName = isOldDialogueGroupNull ? "" : oldDialogueGroup.GroupName;

            UpdateIndexOnNamesListUpdate(dialogueGroupNames, selectedDialogueGroupIndexProperty, oldSelectedDialogueGroupIndex,  oldDialogueGroupName, isOldDialogueGroupNull);
            
            selectedDialogueGroupIndexProperty.intValue = DSInspectorUtility.DrawPopup("Dialogue Group", selectedDialogueGroupIndexProperty, dialogueGroupNames.ToArray());


            string selectedDialogueGroupName = dialogueGroupNames[selectedDialogueGroupIndexProperty.intValue];

            DSDialogueGroupSO selectedDialogueGroup = DSIOUtility.LoadAsset<DSDialogueGroupSO>(
               $"Assets/DialogueSystem/Dialogues/{dialogueContainer.FileName}/Groups/{selectedDialogueGroupName}",
               selectedDialogueGroupName);

            dialogueGroupProperty.objectReferenceValue = selectedDialogueGroup;
            DSInspectorUtility.DrawDisabledFields(() => dialogueGroupProperty.DrawPropertyField());
           
            DSInspectorUtility.DrawSpace();
            
        }

        
        private void DrawDialogueArea(List<string> dialogueNames, string dialogueFolderPath)
        {
            DSInspectorUtility.DrawHeader("Dialogue");

            int oldSelectedDialogueIndex = selectedDialogueIndexProperty.intValue;

            DSDialogueSO oldDialogue = (DSDialogueSO)dialogueProperty.objectReferenceValue;
            
            bool isOldDialogueNull = oldDialogue == null;

            string oldDialogueName = isOldDialogueNull ? "" : oldDialogue.DialogueName;
            
            UpdateIndexOnNamesListUpdate(dialogueNames, selectedDialogueIndexProperty, oldSelectedDialogueIndex, oldDialogueName, isOldDialogueNull);

            selectedDialogueIndexProperty.intValue = DSInspectorUtility.DrawPopup("Dialogue", selectedDialogueIndexProperty, dialogueNames.ToArray());

            string selectedDialogueName = dialogueNames[selectedDialogueIndexProperty.intValue];
            DSDialogueSO selectedDialogue = DSIOUtility.LoadAsset<DSDialogueSO>(dialogueFolderPath, selectedDialogueName);

            dialogueProperty.objectReferenceValue = selectedDialogue;
            DSInspectorUtility.DrawDisabledFields(() =>  dialogueProperty.DrawPropertyField());
           
        }
        
        private void StopDrawing(string reason, MessageType messageType = MessageType.Info)
        {
            DSInspectorUtility.DrawHelpBox(reason, messageType);
            
            DSInspectorUtility.DrawSpace();
            
            DSInspectorUtility.DrawHelpBox("You need to select a Dialogue for this component to work properly at Runtime!", MessageType.Warning);
            
            serializedObject.ApplyModifiedProperties();
        }
        
        #endregion

        #region Index Methods

        private void UpdateIndexOnNamesListUpdate(List<string> optionNames, SerializedProperty indexProperty, int oldSelectedDPropertyIndex,string oldPropertyName, bool isOldPropertynull)
        {
            if (isOldPropertynull)
            {
                indexProperty.intValue = 0;
                return;
            }

            bool oldIndexIsOutOfBoundsOfNamesListCount = oldSelectedDPropertyIndex > optionNames.Count - 1;
            bool oldNameIsDifferentThanSelectedName = oldIndexIsOutOfBoundsOfNamesListCount ||
                                                      oldPropertyName != optionNames[oldSelectedDPropertyIndex];
            
            if (oldNameIsDifferentThanSelectedName|| oldPropertyName != optionNames[oldSelectedDPropertyIndex])
            {
                if (optionNames.Contains(oldPropertyName))
                {
                    indexProperty.intValue = optionNames.IndexOf(oldPropertyName);
                }
                else
                {
                    indexProperty.intValue = 0;
                    
                }
            }
            
        }


        #endregion
    }
    
    
}

