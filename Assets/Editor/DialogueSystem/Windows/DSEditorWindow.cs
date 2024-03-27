using System.IO;
using DS.Utilities;
using UnityEditor;
using UnityEngine.UIElements;
using UnityEditor.UIElements;

namespace DS.Windows
{
    public class DSEditorWindow : EditorWindow
    {
        private DSGraphView graphView;
        
        private string defaultFileName = "DialoguesFileName";

        private static TextField fileNameTextField;
        private Button saveButton;
        private Button miniMapButton;
       
        
        [MenuItem("Window/DS/Dialogue Graph")]
        public static void ShowExample()
        {
            GetWindow<DSEditorWindow>("Dialogue Graph");
            
        }

        private void CreateGUI()
        {
            AddGraphView();
            AddToolbar();
            AddStyles();
        }

       
        #region ElementAddition

        private void AddGraphView()
        {
            graphView = new DSGraphView(this);
            graphView.StretchToParentSize();
            rootVisualElement.Add(graphView);
        }
        
        private void AddToolbar()
        {
           
            Toolbar toolbar = new Toolbar();
            fileNameTextField = DSElementUtility.CreateTextField(defaultFileName, "File Name: ", callback =>
            {
                fileNameTextField.value = callback.newValue.RemoveWhitespaces().RemoveSpecialCharacters();
            });
            saveButton = DSElementUtility.CreateButton("Save", () => Save());

            Button loadButton = DSElementUtility.CreateButton("Load", () => Load());
            Button clearButton = DSElementUtility.CreateButton("Clear", () => OpenClearConfirmationMenu());
            Button resetButton = DSElementUtility.CreateButton("Reset", () => ResetGraph());
            Button deleteButton = DSElementUtility.CreateButton("Delete", () => OpenDeleteConfirmationMenu());
            miniMapButton = DSElementUtility.CreateButton("Minimap", () => ToggleMinimap());
            
            toolbar.Add(fileNameTextField);
            toolbar.Add(saveButton);
            toolbar.Add(loadButton);
            toolbar.Add(clearButton);
            toolbar.Add(resetButton);
            toolbar.Add(deleteButton);
            toolbar.Add(miniMapButton);
            
            
            
            toolbar.AddStyleSheets("DialogueSystem/DSToolbarStyles.uss");
            
            rootVisualElement.Add(toolbar);
            
        }

      


        private void AddStyles()
        {
            rootVisualElement.AddStyleSheets("DialogueSystem/DSVariables.uss");
        }
        
        #endregion

        #region Toolbar Actions

        private void Save()
        {
            if (string.IsNullOrEmpty(fileNameTextField.value))
            {
                EditorUtility.DisplayDialog("Invalid FileName", "Please ensure the file name you've typed in is valid.",
                    "Roger!");
                return;
                
            }
            DSIOUtility.Initialize(graphView, fileNameTextField.value, true);
            DSIOUtility.Save();
        }
        
        private void Load()
        {
            string filePath = EditorUtility.OpenFilePanel("DialogueGraphs", "Assets/Editor/DialogueSystem/Graphs", "asset");

            if (string.IsNullOrEmpty(filePath))
            {
                return;
            }
            Clear();
            
            DSIOUtility.Initialize(graphView, Path.GetFileNameWithoutExtension(filePath));
            DSIOUtility.Load();
        }

        private void Clear()
        {
            graphView.ClearGraph();
        }

        private void OpenClearConfirmationMenu()
        {
            var confirmation =  EditorUtility.DisplayDialog("Confirm Clearing Graph", "Are you sure you want to clear the graph?",
               "Yes", "No");
           if (confirmation)
           {
               Clear();
           }
           
        }
        
        private void OpenDeleteConfirmationMenu()
        {
            var confirmation =  EditorUtility.DisplayDialog("Confirm Deleting Graph", "Are you sure you want to delete the graph and all related files?",
                "Yes", "No");
            if (confirmation)
            {
                Delete();
            }
           
        }

        private void Delete()
        {
            DSIOUtility.Delete();
            UpdateFileName(defaultFileName);
        }

        private void ResetGraph()
        {
            OpenClearConfirmationMenu();
            
            UpdateFileName(defaultFileName);
        }
        
        private void ToggleMinimap()
        {
            graphView.ToggleMiniMap();
            
            miniMapButton.ToggleInClassList("ds-toolbar__button__selected");
        }

        #endregion
       

       


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
