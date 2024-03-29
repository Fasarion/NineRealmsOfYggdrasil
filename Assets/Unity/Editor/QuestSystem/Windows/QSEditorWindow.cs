using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using DS.Utilities;
using QS.Utilities;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

namespace QS.Windows
{
    public class QSEditorWindow : EditorWindow
    {
        private QSGraphView graphView;

        private string defaultFileName = "QuestsFileName";
        public static TextField fileNameTextField;
        public Button saveButton;
        public Button loadButton;
        public Button clearButton;
        
        public Toolbar toolbar;
        [MenuItem("Window/QS/Quest Graph")]
        public static void ShowExample()
        {
            GetWindow<QSEditorWindow>("Quest Graph");
        }
    
        private void CreateGUI()
        {

            AddGraphView();
            AddToolbar();
            AddStyles();
            
        }

        private void AddToolbar()
        {
            toolbar = new Toolbar();
            fileNameTextField = DSElementUtility.CreateTextField(defaultFileName, "File Name: ",
                callback =>
                {
                    fileNameTextField.value = callback.newValue.RemoveWhitespaces().RemoveSpecialCharacters();
                });
            
            saveButton = QSElementUtility.CreateButton("Save", () => Save());
            loadButton = QSElementUtility.CreateButton("Load", () => Load());
            clearButton = QSElementUtility.CreateButton("Clear", () => Clear());
            
            rootVisualElement.Add(toolbar);
            toolbar.Add(fileNameTextField);
            toolbar.Add(saveButton);
            toolbar.Add(loadButton);
            toolbar.Add(clearButton);
            
            toolbar.AddStyleSheets("QuestSystem/QSToolbarStyles.uss");
        }

        #region Toolbar Actions

        private void Save()
        {
            if (string.IsNullOrEmpty(fileNameTextField.value))
            {
                EditorUtility.DisplayDialog("Invalid FileName", "Please ensure the file name you've type in is valid.",
                    "OK");
                return;
            }
            QSIOUtility.Initialize(graphView, fileNameTextField.value, true);
            QSIOUtility.Save();
        }
        
        private void Load()
        {
            string filePath = EditorUtility.OpenFilePanel("QuestGraphs", "Assets/Editor/QuestSystem/Graphs", "asset");

            if (string.IsNullOrEmpty(filePath))
            {
                return;
            }
            Clear();
            
            QSIOUtility.Initialize(graphView, Path.GetFileNameWithoutExtension(filePath));
            QSIOUtility.Load();
        }
        
        private void Clear()
        {
            graphView.ClearGraph();
        }
        
        #endregion


        private void AddGraphView()
        {
            graphView = new QSGraphView(this);
            graphView.StretchToParentSize();
            rootVisualElement.Add(graphView);
        }
        
        private void AddStyles()
        {
            
            rootVisualElement.AddStyleSheets("QuestSystem/QSVariables.uss");
        }


        #region Utility Methods

        public static void UpdateFileName(string newFileName)
        {
            fileNameTextField.value = newFileName;
            
        }
        public void EnableSaving()
        {
            saveButton.SetEnabled(true);
        }
        
        public void DisableSaving()
        {
            saveButton.SetEnabled(false);
        }

        
        
        
        #endregion
        
    }

}
