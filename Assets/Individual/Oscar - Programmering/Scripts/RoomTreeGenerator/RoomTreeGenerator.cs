using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Transforms;
using UnityEngine;
using UnityEngine.UIElements.Experimental;
using Random = System.Random;

public class RoomTreeGenerator : MonoBehaviour
{
    
    public List<RoomChoiceObject> possibleRoomChoiceObjects;
    public GameObject roomSelectionUIPrefab;
    public GameObject connectionUIPrefab;
    public int roomSeed;
    public ChoiceDataScriptableObject choiceDataScriptableObject;
    
    public static Action<Dictionary<Vector2Int, RoomNode>, ProgressionBarLevelContainer[]> roomTreeGenerated;
    
    [SerializeField] private bool generateTreePreview;
    
    
    public int gridWidth;
    private int gridHeight;
    private Vector2Int roomNodeCoordinates;
    private Dictionary<int, List<RoomNode>> roomNodesAtLevel;
    
    private int numberOfLevels;
    private int currentLevelNumber;
    
    private Canvas canvas;

    private Dictionary<Vector2Int, RoomNode> roomNodeGridMapBehaviour;
    Vector2[][] branchConnections;

    private RoomNode startingRoomNode;
    private RoomNode currentParentNode;


   

    //[SerializeField]private bool cacheGridMap = true;
    private bool roomNodeGridMapCached = true;

    
    private List<Vector2Int> keys;
    
    public void Awake()
    {
        //roomNodeGridMap = choiceDataScriptableObject.roomNodeGridMap;
        canvas = FindObjectOfType<Canvas>();

        if (choiceDataScriptableObject.roomNodeGridMapSO != null)
        {
            Debug.Log(choiceDataScriptableObject.roomNodeGridMapSO.Count);
        }
        

        //levelNodeTree = new Dictionary<int, List<GameObject>>();
        //roomNodeGridMap = new Dictionary<Vector2Int, RoomNode>();

        if (!choiceDataScriptableObject.saveGeneratedGridMap)
        {
            roomNodeGridMapCached = false;
            
        }
        
        if (!roomNodeGridMapCached)
        {
            ClearCachedRoomNode();
        }

      
        


    }

    private void ClearCachedRoomNode()
    {
        roomNodeGridMapBehaviour = new Dictionary<Vector2Int, RoomNode>();
        keys = new List<Vector2Int>();
        choiceDataScriptableObject.ClearCachedData();
    }

    public void OnEnable()
    {
        ProgressionBarContentContainer.onProgressionContentSet += OnMapProgressionContentSet;
    }

    public void OnDisable()
    {
        ProgressionBarContentContainer.onProgressionContentSet -= OnMapProgressionContentSet;
    }

    public void Start()
    {
       
    }
    
    public void OnMapProgressionContentSet(ProgressionBarLevelContainer[] levelContainers)
    {
        var levelsInUI = levelContainers.Length;
        if (!roomNodeGridMapCached)
        {
            gridHeight = levelsInUI;
            Random random = new Random(roomSeed);
            GenerateRoomGrid(random);
            GenerateStartingRoomNode();
           
            
            //GenerateRoomTree();
        }
        else
        {
            GetCachedRoomNodeGridMap();
            //If it doesn't exist, we generate a new one.
            if (roomNodeGridMapBehaviour == null|| roomNodeGridMapBehaviour.Count == 0)
            {
                gridHeight = levelsInUI;
                Random random = new Random(roomSeed);
                GenerateRoomGrid(random);
                GenerateStartingRoomNode();
                
            }
            else if(choiceDataScriptableObject.resetNodeProgression == false)
            {
                currentParentNode = choiceDataScriptableObject.currentRoomNode;
                
            }
            else
            {
                
                GenerateStartingRoomNode();
            }
            
            
        }
        if (generateTreePreview)
        {
            GenerateRoomUINodes();
        }
        roomTreeGenerated?.Invoke(roomNodeGridMapBehaviour, levelContainers);


     
        
    }

    private void GetCachedRoomNodeGridMap()
    {
        if (choiceDataScriptableObject.roomNodeGridMapSO == null)
        {
            Debug.LogWarning("SerializedDictionary was null, was this intended?");
        }
        else
        {
            roomNodeGridMapBehaviour = new Dictionary<Vector2Int, RoomNode>(choiceDataScriptableObject.roomNodeGridMapSO);
        }
        
       
    }


    public List<RoomNode> GetCurrentNodeList()
    {
        return currentParentNode.childNodes;
    }

    public RoomNode GetCurrentNode()
    {
        return currentParentNode;
    }

    public void UpdateNodeLevel(RoomNode nodeChoice)
    {
        currentParentNode = nodeChoice;
    }
    

    private void GenerateStartingRoomNode()
    {
        choiceDataScriptableObject.currentRoomNode = null;
        startingRoomNode = new RoomNode(new Vector2Int(0,0));
        for (int i = 0; i < gridWidth; i++)
        {
            roomNodeGridMapBehaviour.TryGetValue(new Vector2Int(1, i), out var roomNode);
            if (roomNode != null)
            {
                roomNode.parentNodes.Add(startingRoomNode);
                startingRoomNode.childNodes.Add(roomNode);
            }
            
        }

        currentParentNode = startingRoomNode;
        choiceDataScriptableObject.currentRoomNode = currentParentNode;
    }

    public Dictionary<Vector2Int, RoomNode> GetRoomGridMap()
    {
        return roomNodeGridMapBehaviour;
    }


    public void PopulateRoomPrefab(GameObject RoomUIInstance, RoomNode node)
    {
       
        var UINodeRectTransform = RoomUIInstance.GetComponent<RectTransform>();
        UINodeRectTransform.anchoredPosition = new Vector2(0 , -1)* 200;
        var UIBehaviour = RoomUIInstance.GetComponent<RoomChoiceUIBehaviour>();
        UIBehaviour.roomNode = node;
        UIBehaviour.UpdateSelectionDisplay(possibleRoomChoiceObjects[node.chosenRoomTypeIndex]);
    }
    public void GenerateRoomUINodes()
    {
        var startingUINode = Instantiate(roomSelectionUIPrefab, canvas.transform);
        var startingUINodeRectTransform = startingUINode.GetComponent<RectTransform>();
        startingUINodeRectTransform.anchoredPosition = new Vector2(0 , -1)* 200;
        var startingUIBehaviour = startingUINode.GetComponent<RoomChoiceUIBehaviour>();
        startingUIBehaviour.roomNode = startingRoomNode;
        startingUIBehaviour.UpdateSelectionDisplay(possibleRoomChoiceObjects[startingRoomNode.chosenRoomTypeIndex]);
        
        foreach (var pair in roomNodeGridMapBehaviour)
        {
            var newUINode = Instantiate(roomSelectionUIPrefab, canvas.transform);
            var uiNodeRectTransform = newUINode.GetComponent<RectTransform>();
            uiNodeRectTransform.anchoredPosition = new Vector2(pair.Key.y , pair.Key.x )* 200;
            var uiBehaviour = newUINode.GetComponent<RoomChoiceUIBehaviour>();
            uiBehaviour.roomNode = pair.Value;
            //What am I doing here? Ah, I am selecting from the possible roomTypes that can be created for a given card. That choice needs to be cached.
            uiBehaviour.UpdateSelectionDisplay(possibleRoomChoiceObjects[pair.Value.chosenRoomTypeIndex]);

            var parentNodes = pair.Value.parentNodes;
            for (int i = 0; i < parentNodes.Count; i++)
            {
                var correctedRoomCoordinates =
                    new Vector2Int(pair.Value.roomCoordinates.y, pair.Value.roomCoordinates.x);
                var correctedParentRoomCoordinates =    new Vector2Int(parentNodes[i].roomCoordinates.y, parentNodes[i].roomCoordinates.x);
                var distanceVectorBetweenNodes = (correctedRoomCoordinates - correctedParentRoomCoordinates);

                var connectionPosition = (Vector2)distanceVectorBetweenNodes / 2f;

                var connectionAngle = -Vector3.SignedAngle((Vector2)distanceVectorBetweenNodes, Vector3.up, Vector3.forward);
                //if(Mathf.Sign(distanceVectorBetweenNodes))
                var connectionRotation = Quaternion.AngleAxis(connectionAngle, Vector3.forward); 
                
                var newConnection = Instantiate(connectionUIPrefab, canvas.transform);

                var uiConnectionRectTransform = newConnection.GetComponent<RectTransform>();

                uiConnectionRectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, distanceVectorBetweenNodes.y * 200);
                uiConnectionRectTransform.anchoredPosition =
                    (Vector2) (connectionPosition + correctedParentRoomCoordinates) * 200;
                uiConnectionRectTransform.rotation = connectionRotation;
            }
            
        }
    }
    /*public void GenerateRoomTree()
    {

        int yPos = -500;
        Random random = new Random(roomSeed);

        var previousNodeCount = 0;
        int combinedPreviousNodeCount = 0;
        for (int i = 0; i < numberOfLevels; i++)
        {


            for (int j = 0; j < combinedPreviousNodeCount; j++)
            {
                //var newUINode = Instantiate(roomSelectionUIPrefab, canvas.transform);
                //newUINode
            }

            int numberOfNodesForLevel;
            if (previousNodeCount == 3)
            {
                numberOfNodesForLevel = 1;
                previousNodeCount = 0;

            }
            else
            {
                numberOfNodesForLevel = random.Next(1, 4);
                previousNodeCount = numberOfNodesForLevel;
            }

            int currentXPos = 0;
            int startingXPos = 0;
            List<GameObject> nodesInCurrentLevel = new List<GameObject>();

            if (numberOfNodesForLevel % 2 == 0)
            {
                startingXPos = -150;
            }
            else
            {
                startingXPos = (numberOfNodesForLevel - 1) / 2 * -300;
            }

            currentXPos = startingXPos;

            var numberOfNodesAddedWithPreviousLevel = numberOfNodesForLevel + previousNodeCount;
            for (int j = 0; j < numberOfNodesAddedWithPreviousLevel; j++)
            {

                currentXPos += 300;
                var chosenRoomIndex = random.Next(0, possibleRoomChoiceObjects.Count);
                var newNode = Instantiate(roomSelectionUIPrefab, canvas.transform);
                var rectTransform = newNode.GetComponent<RectTransform>();
                rectTransform.anchoredPosition = new Vector2(currentXPos, yPos);
                var uiBehaviour = newNode.GetComponent<RoomChoiceUIBehaviour>();
                uiBehaviour.UpdateSelectionDisplay(possibleRoomChoiceObjects[chosenRoomIndex]);
                ;
                nodesInCurrentLevel.Add(newNode);

            }

            combinedPreviousNodeCount = numberOfNodesAddedWithPreviousLevel;
            yPos += 200;
            //levelNodeTree.Add(i, nodesInCurrentLevel);

        }
    }*/

    //Generate a room grid with a particular chance of a room being generated.
    private void GenerateRoomGrid(Random random)
    {
        roomNodeGridMapBehaviour = new Dictionary<Vector2Int, RoomNode>();
        keys = new List<Vector2Int>();
        
        for (int i = 1; i < gridHeight + 1; i++)
        {
            for (int j = 0; j < gridWidth; j++)
            {
                int squareFilled = random.Next(0, 3);
                if (squareFilled < 2)
                {
                    var nodeCoordinates = new Vector2Int(i, j);
                    RoomNode newNode = new RoomNode(nodeCoordinates);
                    var chosenRoomIndex = random.Next(0, possibleRoomChoiceObjects.Count);
                    newNode.chosenRoomTypeIndex = chosenRoomIndex;
                    roomNodeGridMapBehaviour.Add(nodeCoordinates, newNode);
                }
            }
        }
        
        List<Vector2Int> itemsToRemove = new List<Vector2Int>();
        foreach (var pair in roomNodeGridMapBehaviour)
        {
            keys.Add(pair.Key);
            //values.Add(pair.Value);
            //Check if we are on the first level or we have a parent. If we are not at the first level and we do not have a parent, then we can be removed straight away.
            if (pair.Key.x != 1 && pair.Value.parentNodes.Count == 0)
            {
                itemsToRemove.Add(pair.Key);
                continue;
            }
            var coordinate = pair.Key;

            int currentChildNodeIndex = -1;

            List<RoomNode> potentialChildNodes = new List<RoomNode>();

            bool childNodeAdded = false;
            //Loop over the nodes in the coordinates below to the left center and right of the current level.
            for (int i = 0; i < 3; i++)
            {
                var childCoordinates = new Vector2Int(coordinate.x + 1, coordinate.y + currentChildNodeIndex);
                currentChildNodeIndex++;
                roomNodeGridMapBehaviour.TryGetValue(childCoordinates, out RoomNode childNode);

                //Check that there is actually a node on that coordinate if it's within the grid.

                if (childNode != null)
                {
                    potentialChildNodes.Add(childNode);
                    var nodeToRightOfChildCoord = new Vector2Int(childCoordinates.x, childCoordinates.y + 1);
                    RoomNode childNeighbour;
                    roomNodeGridMapBehaviour.TryGetValue(nodeToRightOfChildCoord, out childNeighbour);
                    //Check if that node's right hand neighbour has a parent node.
                    //If it does it means we are trying to cross two connections which is not valid.
                    if (childNeighbour != null)
                    {
                        if (childNeighbour.parentNodes.Count == 0)
                        {
                            //Decide if there should be a connection between the two.
                            var makeConnection = random.Next(0, 3);
                            if (makeConnection < 2)
                            {
                                childNode.parentNodes.Add(pair.Value);
                                pair.Value.childNodes.Add(childNode);
                                childNodeAdded = true;
                            }
                        }
                    }
                    //If there is no neighbour, then it's okay to just add a connection anyways.
                    else
                    {
                        var makeConnection = random.Next(0, 3);
                        if (makeConnection < 2)
                        {
                            childNode.parentNodes.Add(pair.Value);
                            pair.Value.childNodes.Add(childNode);
                            childNodeAdded = true;
                        }
                    }
                }

            }

            //If we haven't added any child nodes 
            //this doesn't respect neighbours
            if (childNodeAdded == false)
            {
              
                
                var guaranteedChildNodeIndex = random.Next(0, potentialChildNodes.Count);
                if (potentialChildNodes.Count > 0)
                {
                    
                    //This is if you want to try and fix the overlapping neighbours
                    //var parentNodes = potentialChildNodes[guaranteedChildNodeIndex].parentNodes;
                    //Get the neighbour to the left.
                    //var currentCoordinates = pair.Value.roomCoordinates;
                    //var childNeighbourCoordinates = new Vector2(currentCoordinates.x + 1, currentCoordinates.y + 1);
                    
                    potentialChildNodes[guaranteedChildNodeIndex].parentNodes.Add(pair.Value); ;
                    pair.Value.childNodes.Add(potentialChildNodes[guaranteedChildNodeIndex]);
                    childNodeAdded = true;
                }

            }

            //If there is still no node to add, we tag the node for removal.
            if (childNodeAdded == false)
            {
                if (pair.Key.x != gridHeight && pair.Value.childNodes.Count == 0)
                {
                    itemsToRemove.Add(pair.Key);
                }
            }
        }


        List<Vector2Int> recursiveRoomNodesToRemove = new List<Vector2Int>();

        for (int i = 0; i < itemsToRemove.Count; i++)
        {
            roomNodeGridMapBehaviour.TryGetValue(itemsToRemove[i], out var node);
            if (node != null)
            {
                TraverseRemovedNodesUpwards(node, recursiveRoomNodesToRemove);
            }
            else
            {
                Debug.LogError("No node to remove, dictionary did not contain node");
            }
            roomNodeGridMapBehaviour.Remove(itemsToRemove[i]);
        }

        for (int i = 0; i < recursiveRoomNodesToRemove.Count; i++)
        {
            roomNodeGridMapBehaviour.Remove(recursiveRoomNodesToRemove[i]);
        }
        
        //foreach (var pair in roomNodeGridMapBehaviour)
        //{
          //  choiceDataScriptableObject.roomNodeGridMapSO.Add(pair.Key,pair.Value);
        //}
        choiceDataScriptableObject.roomNodeGridMapSO = new SerializableDictionary<Vector2Int, RoomNode>(roomNodeGridMapBehaviour);
        //choiceDataScriptableObject.keys = keys;
        if (!choiceDataScriptableObject.saveGeneratedGridMap)
        {
            roomNodeGridMapCached = false;
            
        }

    }
    public void TraverseRemovedNodesUpwards(RoomNode currentNode, List<Vector2Int> recursiveRoomNodesToRemove)
    {
        if (currentNode.childNodes.Count == 0)
        {
            recursiveRoomNodesToRemove.Add(currentNode.roomCoordinates);

            for (int i = 0; i < currentNode.parentNodes.Count; i++)
            {
                var nodeParent = currentNode.parentNodes[i];
            
                if (nodeParent != null)
                {
                    //Remove the current node from its parents child list. 
                    //If the current node is the only node that the parent has in its child list, then the child node list of the parent will be 0
                    //Therefore the parent node is also slated for removal. If not, the parent node has more children and then it is okay to stop here.
                    nodeParent.childNodes.Remove(currentNode);
                    if (nodeParent.childNodes.Count == 0)
                    {
                        TraverseRemovedNodesUpwards(nodeParent, recursiveRoomNodesToRemove);
                    }
                }
            }
           
            
        }
        
    }
    
/*public void BuildRoomTree(RoomNode thisNode, Random randomizer)
{
    var numberOfChildrenToGenerate = randomizer.Next(1, 4);
    /*if (thisNode.nodeLevel < numberOfLevels)
    {
        for (int i = 0; i < numberOfChildrenToGenerate; i++)
        {
            RoomNode childNode = new RoomNode(thisNode, thisNode.nodeLevel + 1);
            thisNode.childNodes.Add(childNode);
            BuildRoomTree(childNode, randomizer);
        }
        
        //roomNodesAtLevel.Add(thisNode.nodeLevel, );
    }
}

/*public void GenerateTreeRecursively()
{ 
    List<RoomNode> rooms = new List<RoomNode>();

    var roomNode = new RoomNode(null, 0);
    Random random = new Random(roomSeed);
    BuildRoomTree(roomNode, random);
}*/


}


