using System;
using System.Collections;
using System.Collections.Generic;
using DevLocker.Utils;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ChoiceUIManager : MonoBehaviour
{

    public enum SelectionType
    {
        roomChoice,
        weaponChoice,
        shopChoice
    }

    [SerializeField] private SelectionType selectionType;
    
    private static ChoiceUIManager _instance;
    
    [SerializeField] private List<RoomChoiceUIBehaviour> roomSelects;
    //This is probably very ineffective, fix.
    

    [SerializeField] private SelectionCardInstantiator roomSelectionCardsInstantiator;
    [SerializeField] private SelectionCardInstantiator weaponSelectionCardsInstantiator;
    [SerializeField] private SelectionCardInstantiator shopSelectionCardsInstantiator;
    
    
    private SelectionCardInstantiator currentSelectionCardsInstantiator;

    [SerializeField]private int currentSelectionIndex;
    private int maxSelectionIndex = 2;
    
    
    private bool _isUIDisplayed;
    private bool selectionCardsMoving;

    private RoomTreeGenerator roomTreeGenerator;
    private LevelProgressionManager levelProgressionManager;
    
    
    public Action<SceneReference> OnRoomChosen;
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
        RoomChoiceUIBehaviour.onRoomChanged += OnRoomChanged;
        currentSelectionCardsInstantiator = roomSelectionCardsInstantiator;
        roomTreeGenerator = GetComponent<RoomTreeGenerator>();
        //allSelectionCardsHidden = true;
        if (_instance == null)
        {
            _instance = this;
        }

        SelectionCardInstantiator.hasExitedScreen += OnSelectionCardExited;
        SelectionCardInstantiator.hasEnteredScreen += OnSelectionCardEntered;
         

        //HideUI();
    }

    private void OnRoomChanged(RoomNode chosenNode)
    {
        roomTreeGenerator.UpdateNodeLevel(chosenNode);
        
        var nodeList = roomTreeGenerator.GetCurrentNodeList();
        roomSelectionCardsInstantiator.InstantiateSelectionCards(nodeList.Count);
        var cardObjects = roomSelectionCardsInstantiator.GetCardObjects();
        for (int i = 0; i < cardObjects.Count; i++)
        {
            roomTreeGenerator.PopulateRoomPrefab(cardObjects[i], nodeList[i]);
        }
        levelProgressionManager.UpdateTargetObject();
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
        var nodeList = roomTreeGenerator.GetCurrentNodeList();
        roomSelectionCardsInstantiator.InstantiateSelectionCards(nodeList.Count);
        var cardObjects = roomSelectionCardsInstantiator.GetCardObjects();
        for (int i = 0; i < cardObjects.Count; i++)
        {
            roomTreeGenerator.PopulateRoomPrefab(cardObjects[i], nodeList[i]);
        }
        
        shopSelectionCardsInstantiator.InstantiateSelectionCards(3);
        weaponSelectionCardsInstantiator.InstantiateSelectionCards(3);
        currentSelectionCardsInstantiator.MoveSelectionCardsIntoView();
        //DisplayRoomChoiceTree(roomChoiceObjects);
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
        
        for (int i = 0; i < roomChoiceObjects.Count; i++)
        {
            RoomChoiceObject roomChoice = roomChoiceObjects[i];
            roomSelects[i].UpdateSelectionDisplay(roomChoice);
        }
    }
    
    
    private void OnEnable()
    {
        
        //var upgradeUISystem = World.DefaultGameObjectInjectionWorld.GetExistingSystemManaged<UpgradeUISystem>();
        //upgradeUISystem.OnUpgradeUIDisplayCall += DisplayUpgradeCards;
    }

    private void OnDisable()
    {
        //if (World.DefaultGameObjectInjectionWorld == null) return;
        //var upgradeUISystem = World.DefaultGameObjectInjectionWorld.GetExistingSystemManaged<UpgradeUISystem>();
        //upgradeUISystem.OnUpgradeUIDisplayCall -= DisplayUpgradeCards;
    }
    

    private void ShowUI(int roomCount)
    {
        for (int i = 0; i < roomCount; i++)
        {
            roomSelects[i].gameObject.SetActive(true);
        }

        _isUIDisplayed = true;
        Time.timeScale = 0f;
    }

    private void HideUI()
    {
        foreach (var room in roomSelects)
        {
            room.gameObject.SetActive(false);
        }

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
                UpdateSelectionType(currentSelectionIndex);
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
                UpdateSelectionType(currentSelectionIndex);
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
                currentSelectionCardsInstantiator = shopSelectionCardsInstantiator;
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
        HideUI();
       
        OnRoomChosen?.Invoke(roomSceneReference);
        SceneManager.LoadScene(roomSceneReference.SceneName);
    }
}
