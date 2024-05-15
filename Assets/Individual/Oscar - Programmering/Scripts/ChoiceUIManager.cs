using System;
using System.Collections;
using System.Collections.Generic;
using DevLocker.Utils;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ChoiceUIManager : MonoBehaviour
{
    
    
    //private ChoiceSO choiceSo;
    public enum SelectionType
    {
        roomChoice,
        weaponChoice,
        shopChoice
    }

    [SerializeField] private SelectionType selectionType;
    
    private static ChoiceUIManager _instance;
    
    //[SerializeField] private List<RoomChoiceUIBehaviour> roomSelects;
    //This is probably very ineffective, fix.
    
    
    

    [SerializeField] private SelectionCardInstantiator roomSelectionCardsInstantiator;
    [SerializeField] private SelectionCardInstantiator weaponSelectionCardsInstantiator;
    [SerializeField] private SelectionCardInstantiator relicSelectionCardsInstantiator;
    
    
    private SelectionCardInstantiator currentSelectionCardsInstantiator;

    [SerializeField]private int currentSelectionIndex;
    private int maxSelectionIndex = 2;
    
    
    private bool _isUIDisplayed;
    private bool selectionCardsMoving;

    private RoomTreeGenerator roomTreeGenerator;
    private LevelProgressionManager levelProgressionManager;
    
    
    public Action<SceneReference> OnRoomChosen;

    private Dictionary<Vector2Int, RoomNode> roomNodeGridMap;


    private ProgressionBarLevelContainer[] levelContainers;
    private ProgressionBarBehaviour[] currentSymbols;

    private int currentLevelIndex;
    public ChoiceDataScriptableObject choiceDataScriptableObject;
    
    public ArrowIndicatorUI arrowIndicator;
    public ProgressIndicatorBackgroundUI progressBackgroundIndicator;

    //private Rect runtimeRect;
    
    
    public static ChoiceUIManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindObjectOfType<ChoiceUIManager>();
            }

            return _instance;
        }
    }
    private void Awake()
    {
       
        //ChoiceSO.loadChoiceData;
        roomTreeGenerator = GetComponent<RoomTreeGenerator>();
        //allSelectionCardsHidden = true;
        if (_instance == null)
        {
            _instance = this;
        }

        
        

        //HideUI();
    }

    public void Update()
    {
        arrowIndicator.AddSymbolXPosition(currentSymbols[currentSelectionIndex].symbolXpos); //-126 + currentSymbols[currentSelectionIndex].symbolXpos);
        progressBackgroundIndicator.UpdateProgressBackgroundXPosition();
    }

    private void OnCardClicked()
    {
        
        
       
        SwapScreenRight();
    }


    private void OnRoomChanged(RoomNode chosenNode, SceneReference roomSceneReference)
    {
        //currentSelectionIndex++;
        roomTreeGenerator.UpdateNodeLevel(chosenNode);
        
        //
        //var nodeList = roomTreeGenerator.GetCurrentNodeList();
        //roomSelectionCardsInstantiator.InstantiateSelectionCards(nodeList.Count);
        //var cardObjects = roomSelectionCardsInstantiator.GetCardObjects();
        //for (int i = 0; i < cardObjects.Count; i++)
        //{
          //  roomTreeGenerator.PopulateRoomPrefab(cardObjects[i], nodeList[i]);
        //}
        //levelProgressionManager.UpdateTargetObject();
        
        RegisterRoomSelectionClick(roomSceneReference);
    }
    private void OnSelectionCardEntered()
    {
        selectionCardsMoving = false;
    }

    private void OnSelectionCardExited()
    {
        currentSelectionCardsInstantiator.MoveSelectionCardsIntoView();
        
        // UpdateSelectionType();

    }
    
   

    public void Start()
    {
       
        
        
        
        //DisplayRoomChoiceTree(roomChoiceObjects);
    }

    public void OnRoomTreeGenerated(Dictionary<Vector2Int, RoomNode> generatedRoomNodeGridMap, ProgressionBarLevelContainer[] updatedLevelContainers)
    {
        levelContainers = updatedLevelContainers;
        //We need to make sure that currentRoomNode has been populated here, otherwise it's going to show the wrong level
        //Or lead to a null reference exception.
        currentLevelIndex = choiceDataScriptableObject.currentRoomNode.roomCoordinates.x;
        currentSymbols = levelContainers[currentLevelIndex].symbolBehaviours;
        maxSelectionIndex = currentSymbols.Length - 1;

        
        switch (currentSymbols[0].CardType)
        {
            case ProgressionBarBehaviour.ProgressionBarCardType.room:
            {
                currentSelectionCardsInstantiator = roomSelectionCardsInstantiator;
                break;
            }
                
            case ProgressionBarBehaviour.ProgressionBarCardType.relic:
            {
                currentSelectionCardsInstantiator = relicSelectionCardsInstantiator;
                break;
            }
            case ProgressionBarBehaviour.ProgressionBarCardType.weapon:
            {
                currentSelectionCardsInstantiator = weaponSelectionCardsInstantiator;
                break;
            }
        }
        
        for (int i = 0; i < currentSymbols.Length; i++)
        {
            switch (currentSymbols[i].CardType)
            {
                case ProgressionBarBehaviour.ProgressionBarCardType.room:
                {
                    CreateRoomCards(generatedRoomNodeGridMap);
                    break;
                }
                
                case ProgressionBarBehaviour.ProgressionBarCardType.relic:
                {
                    relicSelectionCardsInstantiator.InstantiateSelectionCards(3);
                    break;
                }
                case ProgressionBarBehaviour.ProgressionBarCardType.weapon:
                {
                    weaponSelectionCardsInstantiator.InstantiateSelectionCards(3);
                    break;
                }
            }
        }

        currentSelectionIndex = 0;
    
        
        //This is for generating room cards
        
        
        //This is for generating relicCards
       
       
        
        currentSelectionCardsInstantiator.MoveSelectionCardsIntoView();
    }


    public void CreateRoomCards(Dictionary<Vector2Int, RoomNode> generatedRoomNodeGridMap)
    {
        roomNodeGridMap = generatedRoomNodeGridMap;
        
        var nodeList = roomTreeGenerator.GetCurrentNodeList();
        roomSelectionCardsInstantiator.InstantiateSelectionCards(nodeList.Count);
        var cardObjects = roomSelectionCardsInstantiator.GetCardObjects();
        for (int i = 0; i < cardObjects.Count; i++)
        {
            roomTreeGenerator.PopulateRoomPrefab(cardObjects[i], nodeList[i]);
        }
    }

    public void PopulateRoomCards()
    {
        Dictionary<Vector2Int, RoomNode> roomGrid = roomTreeGenerator.GetRoomGridMap();
        for (int i = 0; i < roomTreeGenerator.gridWidth; i++)
        {
            
        }

    }

    private void DisplayRoomChoiceTree(List<RoomChoiceObject> roomChoiceObjects)
    {
        ShowUI(roomChoiceObjects.Count);
        
        /*for (int i = 0; i < roomChoiceObjects.Count; i++)
        {
            RoomChoiceObject roomChoice = roomChoiceObjects[i];
            roomSelects[i].UpdateSelectionDisplay(roomChoice);
        }*/
    }
    
    
    private void OnEnable()
    {

        RoomTreeGenerator.roomTreeGenerated += OnRoomTreeGenerated;

        RoomChoiceUIBehaviour.onRoomChanged += OnRoomChanged;
        ChoiceUIBehaviour.onCardMouseClick += OnCardClicked;
        
        SelectionCardInstantiator.hasExitedScreen += OnSelectionCardExited;
        SelectionCardInstantiator.hasEnteredScreen += OnSelectionCardEntered;

        ProgressionBarContentContainer.onRectBoundsSet += OnRectBoundsSet;

    }
    private void OnRectBoundsSet(RectTransform rectTransform)
    {
        //runtimeRect = rectTransform;
    }


    private void OnDisable()
    {
        RoomTreeGenerator.roomTreeGenerated -= OnRoomTreeGenerated;
        
        RoomChoiceUIBehaviour.onRoomChanged -= OnRoomChanged;
        ChoiceUIBehaviour.onCardMouseClick -= OnCardClicked;
        
        SelectionCardInstantiator.hasExitedScreen -= OnSelectionCardExited;
        SelectionCardInstantiator.hasEnteredScreen -= OnSelectionCardEntered;
    }
    

    private void ShowUI(int roomCount)
    {
        /*for (int i = 0; i < roomCount; i++)
        {
            roomSelects[i].gameObject.SetActive(true);
        }*/

        _isUIDisplayed = true;
        Time.timeScale = 0f;
    }

    private void HideUI()
    {
        /*foreach (var room in roomSelects)
        {
            room.gameObject.SetActive(false);
        }*/

        _isUIDisplayed = false;
        Time.timeScale = 1f;
    }

    public void SwapScreenRight()
    {
        if (!selectionCardsMoving)
        {
            if (currentSelectionIndex < maxSelectionIndex)
            {
                currentSelectionIndex++;
                selectionCardsMoving = true;
                currentSelectionCardsInstantiator.MoveSelectionCardsOutOfView();
                //UpdateSelectionType(currentSelectionIndex);
                //Not sure if this is redundant
                arrowIndicator.AddSymbolXPosition(currentSymbols[currentSelectionIndex].symbolXpos); 
                //Turn this into a function for the love of god.
                switch (currentSymbols[currentSelectionIndex].CardType)
                {
                    case ProgressionBarBehaviour.ProgressionBarCardType.room:
                    {
                        currentSelectionCardsInstantiator = roomSelectionCardsInstantiator;
                        break;
                    }
                
                    case ProgressionBarBehaviour.ProgressionBarCardType.relic:
                    {
                        currentSelectionCardsInstantiator = relicSelectionCardsInstantiator;
                        break;
                    }
                    case ProgressionBarBehaviour.ProgressionBarCardType.weapon:
                    {
                        currentSelectionCardsInstantiator = weaponSelectionCardsInstantiator;
                        break;
                    }
                }
            }
        }
     
    }
    
    public void SwapScreenLeft()
    {
        if (!selectionCardsMoving)
        {
            if (currentSelectionIndex > 0)
            {
                selectionCardsMoving = true;
                currentSelectionIndex--;
                currentSelectionCardsInstantiator.MoveSelectionCardsOutOfView();
                //UpdateSelectionType(currentSelectionIndex);
            }
        }
    }
    
    public void UpdateSelectionType(int index)
    {
        
        switch (index)
        {
            case 0:
            {
               
                selectionType = SelectionType.roomChoice;
                UpdateSelectionScreen();
                break;
            }
            case 1:
            {
                selectionType = SelectionType.weaponChoice;
               
                UpdateSelectionScreen();
                break;
            }
            case 2:
            {
                selectionType = SelectionType.shopChoice;
               
                UpdateSelectionScreen();
                break;
            }
            default:
            {
                Debug.LogError("Error! Index out of scope. Did you add an enum for that index?");
                break;
            }
            
          
        }
        //currentSelectionCardsInstantiator.MoveSelectionCardsIntoView();
    }
    

    public void UpdateSelectionScreen()
    {
        switch (selectionType)
        {
            case SelectionType.roomChoice:
            {
                currentSelectionCardsInstantiator = roomSelectionCardsInstantiator;
                break;
            }
            case SelectionType.shopChoice:
            {
                currentSelectionCardsInstantiator = relicSelectionCardsInstantiator;
                break;
            }
            case SelectionType.weaponChoice:
            {
                currentSelectionCardsInstantiator = weaponSelectionCardsInstantiator;
                break;
            }
        }
    }
    
    /*public void OnSelectionTypeClick()
    {
        switch (selectionType)
        {
            case SelectionType.roomChoice:
            {
                break;
            }
            case SelectionType.shopChoice:
            {
                break;
            }
            case SelectionType.weaponChoice:
            {
                break;
            }
        }
    }*/
    
    public void RegisterRoomSelectionClick(SceneReference roomSceneReference)
    {
        //HideUI();
       
        OnRoomChosen?.Invoke(roomSceneReference);
        SceneManager.LoadScene(roomSceneReference.SceneName);
    }
}
