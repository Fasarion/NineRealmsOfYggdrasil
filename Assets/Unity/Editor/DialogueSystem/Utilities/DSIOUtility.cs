using System;
using System.Collections.Generic;
using System.Linq;
using DS.Data;
using DS.Data.Save;
using DS.Elements;
using DS.ScriptableObjects;
using DS.Windows;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

namespace DS.Utilities
{

    public static class DSIOUtility
    {

        private static DSGraphView graphView;
        private static string graphFileName;
        private static string originalGraphFileName;
        private static string containerFolderPath;

        private static List<DSGroup> groups;
        public static List<DSNode> nodes;

        private static Dictionary<string, DSDialogueGroupSO> createDialogueGroups;
        private static Dictionary<string, DSDialogueSO> createdDialogues;

        private static Dictionary<string, DSGroup> loadedGroups;
        private static Dictionary<string, DSNode> loadedNodes;

        private static bool savedButNotLoaded;

        private static DSGraphSaveDataSO graphData;

        public static void Initialize(DSGraphView dsGraphView, string graphName, bool updateFolderPath = false)
        {
            graphView = dsGraphView;
            graphFileName = graphName;
            if (updateFolderPath)
            {
                containerFolderPath = $"Assets/DialogueSystem/Dialogues/{graphFileName}";
            }

            groups = new List<DSGroup>();
            nodes = new List<DSNode>();

            createDialogueGroups = new Dictionary<string, DSDialogueGroupSO>();
            createdDialogues = new Dictionary<string, DSDialogueSO>();

            loadedGroups = new Dictionary<string, DSGroup>();
            loadedNodes = new Dictionary<string, DSNode>();


        }

        #region Save Methods

        public static void Save()
        {
            CreateStaticFolders();
            GetElementsFromGraphView();

            graphData = CreateAsset<DSGraphSaveDataSO>("Assets/Editor/DialogueSystem/Graphs", $"{graphFileName}Graph");
            graphData.Initialize(graphFileName);

            
            DSDialogueContainerSO dialogueContainer =
                CreateAsset<DSDialogueContainerSO>(containerFolderPath, graphFileName);

            dialogueContainer.Initialize(graphFileName);
            SaveGroups(graphData, dialogueContainer);
            SaveNodes(graphData, dialogueContainer);

            
            
            
            dialogueContainer.ConvertDSContainerToQuestNode();
            SaveAsset(graphData);
            SaveAsset(dialogueContainer);
            Debug.Log("Saving");
            Debug.Log("File name:" + graphFileName);
            Debug.Log("Container name:" + containerFolderPath);
            savedButNotLoaded = true;

        }




        #region Groups

        private static void SaveGroups(DSGraphSaveDataSO graphData, DSDialogueContainerSO dialogueContainer)
        {
            List<string> groupNames = new List<string>();

            foreach (DSGroup group in groups)
            {
                SaveGroupToGraph(group, graphData);
                SaveGroupToScriptableObject(group, dialogueContainer);
                groupNames.Add(group.title);
            }

            UpdateOldGroups(groupNames, graphData);
        }



        private static void SaveGroupToGraph(DSGroup group, DSGraphSaveDataSO graphData)
        {
            DSGroupSaveData groupData = new DSGroupSaveData()
            {
                ID = group.ID,
                Name = group.title,
                Position = group.GetPosition().position

            };

            graphData.groups.Add(groupData);



        }

        private static void SaveGroupToScriptableObject(DSGroup group, DSDialogueContainerSO dialogueContainer)
        {
            string groupName = group.title;

            CreateFolder($"{containerFolderPath}/Groups", groupName);
            CreateFolder($"{containerFolderPath}/Groups/{groupName}", "Dialogues");
            DSDialogueGroupSO dialogueGroup =
                CreateAsset<DSDialogueGroupSO>($"{containerFolderPath}/Groups/{groupName}", groupName);

            dialogueGroup.Initialize(groupName);

            createDialogueGroups.Add(group.ID, dialogueGroup);
            dialogueContainer.dialogueGroups.Add(dialogueGroup, new List<DSDialogueSO>());

            SaveAsset(dialogueGroup);


        }

        private static void UpdateOldGroups(List<string> currentGroupNames, DSGraphSaveDataSO graphData)
        {
            if (graphData.oldGroupNames != null && graphData.oldGroupNames.Count != 0)
            {
                List<string> groupsToRemove = graphData.oldGroupNames.Except(currentGroupNames).ToList();

                foreach (string groupToRemove in groupsToRemove)
                {
                    RemoveFolder($"{containerFolderPath}/Groups/{groupToRemove}");

                }
            }

            graphData.oldGroupNames = new List<string>(currentGroupNames);
        }



        #endregion

        #region Nodes

        private static void SaveNodes(DSGraphSaveDataSO graphData, DSDialogueContainerSO dialogueContainer)
        {
            SerializableDictionary<string, List<string>> groupedNodeNames =
                new SerializableDictionary<string, List<string>>();
            List<string> ungroupedNodeNames = new List<string>();
            foreach (DSNode node in nodes)
            {
                SaveNodeToGraph(node, graphData);
                SaveNodeToScriptableObject(node, dialogueContainer);
                if (node.Group != null)
                {
                    groupedNodeNames.AddItem(node.Group.title, node.DialogueName);
                    continue;
                }

                ungroupedNodeNames.Add(node.DialogueName);
            }

            UpdateDialoguesChoicesConnections();
            UpdateOldGroupedNodes(groupedNodeNames, graphData);
            UpdateOldUngroupedNodes(ungroupedNodeNames, graphData);

        }




        private static void SaveNodeToGraph(DSNode node, DSGraphSaveDataSO graphSaveData)
        {
            List<DSChoiceSaveData> choices = CloneNodeChoices(node.choices);

            DSNodeSaveData nodeData = new DSNodeSaveData()
            {
                ID = node.ID,
                Name = node.DialogueName,
                Choices = choices,
                Text = node.Text,
                GroupID = node.Group?.ID,
                DialogueType = node.DialogueType,
                Position = node.GetPosition().position
            };

            graphSaveData.nodes.Add(nodeData);
        }



        private static void SaveNodeToScriptableObject(DSNode node, DSDialogueContainerSO dialogueContainer)
        {
            DSDialogueSO dialogue;

            if (node.Group != null)
            {
                dialogue = CreateAsset<DSDialogueSO>($"{containerFolderPath}/Groups/{node.Group.title}/Dialogues",
                    node.DialogueName);
                dialogueContainer.dialogueGroups.AddItem(createDialogueGroups[node.Group.ID], dialogue);
            }
            else
            {
                dialogue = CreateAsset<DSDialogueSO>($"{containerFolderPath}/Global/Dialogues", node.DialogueName);
                dialogueContainer.ungroupedDialogues.Add(dialogue);
            }

            dialogue.Initialize(
                node.DialogueName,
                node.Text,
                ConvertNodeChoicesToDialogueChoices(node.choices),
                node.DialogueType,
                node.IsStartingNode()
            );

            createdDialogues.Add(node.ID, dialogue);

            SaveAsset(dialogue);
        }

        private static List<DSDialogueChoiceData> ConvertNodeChoicesToDialogueChoices(
            List<DSChoiceSaveData> nodeChoices)
        {
            List<DSDialogueChoiceData> dialougeChoices = new List<DSDialogueChoiceData>();
            foreach (DSChoiceSaveData nodeChoice in nodeChoices)
            {
                DSDialogueChoiceData choiceData = new DSDialogueChoiceData()
                {
                    Text = nodeChoice.Text
                };
                dialougeChoices.Add(choiceData);

            }

            return dialougeChoices;
        }

        private static void UpdateDialoguesChoicesConnections()
        {
            foreach (DSNode node in nodes)
            {
                DSDialogueSO dialogue = createdDialogues[node.ID];

                for (int choiceIndex = 0; choiceIndex < node.choices.Count; ++choiceIndex)
                {
                    DSChoiceSaveData nodeChoice = node.choices[choiceIndex];

                    if (string.IsNullOrEmpty(nodeChoice.NodeID))
                    {
                        continue;
                    }

                    dialogue.Choices[choiceIndex].NextDialogue = createdDialogues[nodeChoice.NodeID];
                    SaveAsset(dialogue);
                }
            }
        }


        private static void UpdateOldGroupedNodes(SerializableDictionary<string, List<string>> currentGroupedNodeNames,
            DSGraphSaveDataSO graphSaveData)
        {
            if (graphSaveData.OldGroupedNodeNames != null && graphSaveData.OldGroupedNodeNames.Count != 0)
            {
                foreach (KeyValuePair<string, List<string>> oldGroupedNode in graphSaveData.OldGroupedNodeNames)
                {
                    List<string> nodesToRemove = new List<string>();

                    if (currentGroupedNodeNames.ContainsKey(oldGroupedNode.Key))
                    {
                        nodesToRemove = oldGroupedNode.Value.Except(currentGroupedNodeNames[oldGroupedNode.Key])
                            .ToList();

                        foreach (string nodeToRemove in nodesToRemove)
                        {
                            RemoveAsset($"{containerFolderPath}/Groups/{oldGroupedNode.Key}/Dialogues", nodeToRemove);
                        }
                    }
                }

                graphSaveData.OldGroupedNodeNames =
                    new SerializableDictionary<string, List<string>>(currentGroupedNodeNames);
            }
        }

        private static void UpdateOldUngroupedNodes(List<string> currentUngroupedNodeNames, DSGraphSaveDataSO graphData)
        {
            if (graphData.oldUngroupedNodeNames != null)
            {
                List<string> nodesToRemove = graphData.oldUngroupedNodeNames.Except(currentUngroupedNodeNames).ToList();

                foreach (string nodeToRemove in nodesToRemove)
                {
                    RemoveAsset($"{containerFolderPath}/Global/Dialogues", nodeToRemove);
                }

                graphData.oldUngroupedNodeNames = new List<string>(currentUngroupedNodeNames);

            }
            else
            {
                graphData.oldUngroupedNodeNames = new List<string>(currentUngroupedNodeNames);
            }
        }


        #endregion

        #endregion

        #region LoadMethods

        public static void Load()
        {

            graphData =
                LoadAsset<DSGraphSaveDataSO>("Assets/Editor/DialogueSystem/Graphs", graphFileName);
            if (graphData == null)
            {
                EditorUtility.DisplayDialog(
                    "Couldn't load the file!",
                    "The file at the following path could not be found: \n\n" +
                    $"Assets/Editor/DialogueSystem/Graphs/{graphFileName}\n\n" +
                    "Make sure you chose the right file and its place at the folder path mentioned above.",
                    "Thanks!"
                );
                
                return;
            }

            Debug.Log("Loading");
            Debug.Log("File name:" + graphFileName);
            Debug.Log("Container name:" + containerFolderPath);

            DSEditorWindow.UpdateFileName(graphData.FileName);

            LoadGroups(graphData.groups);
            LoadNodes(graphData.nodes);
            LoadNodesConnections();

            savedButNotLoaded = false;

        }



        private static void LoadGroups(List<DSGroupSaveData> groups)
        {
            foreach (DSGroupSaveData groupData in groups)
            {
                DSGroup group = graphView.CreateGroup(groupData.Name, groupData.Position);
                group.ID = groupData.ID;

                loadedGroups.Add(group.ID, group);
            }
        }

        private static void LoadNodes(List<DSNodeSaveData> nodes)
        {
            foreach (DSNodeSaveData nodeData in nodes)
            {
                List<DSChoiceSaveData> choices = CloneNodeChoices(nodeData.Choices);


                DSNode node = graphView.CreateNode(nodeData.Name, nodeData.DialogueType, nodeData.Position, false);

                node.ID = nodeData.ID;
                node.choices = choices;
                node.Text = nodeData.Text;

                node.Draw();

                graphView.AddElement(node);

                loadedNodes.Add(node.ID, node);

                if (string.IsNullOrEmpty(nodeData.GroupID))
                {
                    continue;
                }

                DSGroup group = loadedGroups[nodeData.GroupID];

                node.Group = group;

                group.AddElement(node);

            }
        }


        private static void LoadNodesConnections()
        {
            //Go through all the loaded nodes
            foreach (KeyValuePair<string, DSNode> loadedNode in loadedNodes)
            {
                //Go through all output ports. All outward going node connections
                foreach (Port choicePort in loadedNode.Value.outputContainer.Children())
                {
                    //Grab the userData from the Port. This info should include the ID and name.
                    DSChoiceSaveData choiceData = (DSChoiceSaveData)choicePort.userData;

                    if (string.IsNullOrEmpty(choiceData.NodeID))
                    {
                        continue;
                    }

                    DSNode nextNode = loadedNodes[choiceData.NodeID];

                    Port nextNodeInputPort = (Port)nextNode.inputContainer.Children().First();
                    //var node = nextNodeInputPort.node;

                    Edge edge = choicePort.ConnectTo(nextNodeInputPort);

                    graphView.AddElement(edge);

                    loadedNode.Value.RefreshPorts();

                }

            }
        }



        #endregion

        #region Delete Methods

        public static void Delete()
        {
            List<string> groupNames = new List<string>();
            if (savedButNotLoaded)
            {
                RemoveAsset("Assets/Editor/DialogueSystem/Graphs/", $"{graphFileName}Graph");
                //RemoveFolder(containerFolderPath);



                foreach (DSGroup group in groups)
                {
                    groupNames.Add(group.title);
                }

                foreach (string groupToRemove in groupNames)
                {
                    RemoveFolder($"{containerFolderPath}/Groups/{groupToRemove}");
                }

                return;
            }

            RemoveAsset("Assets/Editor/DialogueSystem/Graphs/", $"{graphFileName}");


            foreach (DSGroup group in groups)
            {
                groupNames.Add(group.title);
            }

            foreach (string groupToRemove in groupNames)
            {
                RemoveFolder($"{containerFolderPath}/Groups/{groupToRemove}");
            }

            //RemoveFolder(containerFolderPath);
            //RemoveFolder("Assets/Dialogues/");
            //RemoveFolder();
            Debug.Log("Deleting");
            Debug.Log("File name:" + graphFileName);
            Debug.Log("Container name:" + containerFolderPath);
            //Debug.Log(assets);

            //DSGraphSaveDataSO graphData = CreateAsset<DSGraphSaveDataSO>("Assets/Editor/DialogueSystem/Graphs", $"{graphFileName}Graph");
        }

        #endregion


        #region Creation Methods

        private static void CreateStaticFolders()
        {
            CreateFolder("Assets/", " DialogueSystem");
            CreateFolder("Assets/Editor/DialogueSystem", "Graphs");
            CreateFolder("Assets/DialogueSystem", "Dialogues");
            CreateFolder("Assets/DialogueSystem/Dialogues", graphFileName);
            CreateFolder(containerFolderPath, "Global");
            CreateFolder(containerFolderPath, "Groups");
            CreateFolder($"{containerFolderPath}/Global", "Dialogues");

        }

        #endregion

        #region Fetch Methods

        public static void GetElementsFromGraphView()
        {
            Type groupType = typeof(DSGroup);
            graphView.graphElements.ForEach(graphElement =>
            {
                //Uses pattern matching, what's that?
                if (graphElement is DSNode node)
                {
                    nodes.Add(node);
                    return;
                }

                if (graphElement.GetType() == groupType)
                {
                    DSGroup group = (DSGroup)graphElement;
                    groups.Add(group);
                    return;
                }
            });
        }

        #endregion

        #region Utility Methods

        public static void CreateFolder(string path, string folderName)
        {
            if (AssetDatabase.IsValidFolder($"{path}/{folderName}"))
            {

                return;
            }

            AssetDatabase.CreateFolder(path, folderName);

        }

        public static void RemoveFolder(string fullPath)
        {

            FileUtil.DeleteFileOrDirectory($"{fullPath}.meta");
            FileUtil.DeleteFileOrDirectory($"{fullPath}/");


        }

        public static T CreateAsset<T>(string path, string assetName) where T : ScriptableObject
        {
            string fullPath = $"{path}/{assetName}.asset";
            T asset = LoadAsset<T>(path, assetName);

            if (asset == null)
            {
                asset = ScriptableObject.CreateInstance<T>();

                AssetDatabase.CreateAsset(asset, fullPath);
            }


            return asset;


        }

        public static T LoadAsset<T>(string path, string assetName) where T : ScriptableObject
        {
            string fullPath = $"{path}/{assetName}.asset";
            return AssetDatabase.LoadAssetAtPath<T>(fullPath);
        }

        public static void RemoveAsset(string path, string assetName)
        {
            AssetDatabase.DeleteAsset($"{path}/{assetName}.asset");
            //AssetDatabase.Refresh();
        }

        public static void SaveAsset(UnityEngine.Object asset)
        {
            EditorUtility.SetDirty(asset);

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }

        private static List<DSChoiceSaveData> CloneNodeChoices(List<DSChoiceSaveData> nodeChoices)
        {
            List<DSChoiceSaveData> choices = new List<DSChoiceSaveData>();

            foreach (DSChoiceSaveData choice in nodeChoices)
            {
                DSChoiceSaveData choiceData = new DSChoiceSaveData()
                {
                    Text = choice.Text,
                    NodeID = choice.NodeID
                };

                choices.Add(choiceData);
            }

            return choices;
        }

        #endregion

        #region ConversionMethods
        
        public static void ConstructNodeListsRecursively(DSDialogueSO dialogueSO, List<List<DSDialogueSO>> allLists, List<DSDialogueSO> currentList, HashSet<DSDialogueSO> uniqueNodeSet, int iterationIndex)
        {
            if (dialogueSO == null)
            {
                Debug.LogWarning("dialogueSO was empty, did you attempt to recurse through an empty graph?");
                return;
            }
            //Failsafe to keep me safe in my recursive moments.
            if (iterationIndex > 30)
            {
                Debug.LogWarning("Recursive function suspended prematurely to prevent infinite looping. Did you forget to remove the iteration limit?");
                return;
            }

           
            iterationIndex++;
            currentList.Add(dialogueSO);
            /*if (dialogueSo.isStartingDialogue)
            {
                branchKeys.Add(dialogueSo);
            }*/
            
            
            if (dialogueSO.Choices[0].NextDialogue == null)
            {
                allLists.Add(currentList);
                return;
            }
            if (dialogueSO.Choices.Count == 1)
            {
                var nextDialogue = dialogueSO.Choices[0].NextDialogue;

                if (uniqueNodeSet.Contains(nextDialogue))
                {
                    allLists.Add(currentList);
                    return;
                }

                uniqueNodeSet.Add(nextDialogue);
                ConstructNodeListsRecursively(nextDialogue, allLists, currentList,uniqueNodeSet,  iterationIndex);
                
            }
            else if(dialogueSO.Choices.Count > 1)
            {
                allLists.Add(currentList);
                foreach (var choice in dialogueSO.Choices)
                {
                    var nextDialogue = choice.NextDialogue;
                    if (uniqueNodeSet.Contains(nextDialogue))
                    {
                        continue;
                    }
                    uniqueNodeSet.Add(nextDialogue);
                    currentList = new List<DSDialogueSO>();
                    //branchKeys.Add(dialogueSo);
                    ConstructNodeListsRecursively(nextDialogue, allLists, currentList,uniqueNodeSet,iterationIndex);
                }
            }
        }
        #endregion
    }
}