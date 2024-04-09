using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = System.Random;

public class RoomTreeGenerator : MonoBehaviour
{
    public int numberOfLevels;
    public List<RoomChoiceObject> possibleRoomChoiceObjects; 
    public GameObject roomSelectionUIPrefab;
    public int roomSeed;
    
    private int currentLevelNumber;
    private Dictionary<int, List<GameObject>> levelNodeTree;

    private Canvas canvas;

    public void Awake()
    {
        canvas = FindObjectOfType<Canvas>();
        levelNodeTree = new Dictionary<int, List<GameObject>>();
    }

    public void Start()
    {
        GenerateRoomTree();
    }

    public void GenerateRoomTree()
    {
        int yPos = -500;
        Random random = new Random(roomSeed);

        var previousNodeCount = 0;
        int combinedPreviousNodeCount = 0;
        for (int i = 0; i < numberOfLevels; i++)
        {

            for (int j = 0; j < combinedPreviousNodeCount; j++)
            {
                
            }
            int numberOfNodesForLevel;
            if (previousNodeCount == 3)
            {
                numberOfNodesForLevel = 1;
                previousNodeCount = 0;
                
            }
            else
            {
                numberOfNodesForLevel =  random.Next(1, 4);
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
                uiBehaviour.UpdateRoomSelectionDisplay(possibleRoomChoiceObjects[chosenRoomIndex]); ;
                nodesInCurrentLevel.Add(newNode);
                
            }

            combinedPreviousNodeCount  = numberOfNodesAddedWithPreviousLevel;
            yPos += 200;
            levelNodeTree.Add(i, nodesInCurrentLevel);

        }
    }
    
    
}
