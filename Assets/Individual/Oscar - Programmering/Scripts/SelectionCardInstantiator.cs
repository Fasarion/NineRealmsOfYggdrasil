using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelectionCardInstantiator : MonoBehaviour
{
    public Transform parentToPopulate;
    public GameObject prefabType;

    [SerializeField] private List<GameObject> cardObjects;

    private RectTransform thisRect;
    [SerializeField]private Vector2 startPos;
    [SerializeField]private Vector2 screenCenterPos;
    [SerializeField]private float currentTimeToMoveUIIntoScreen;
    [SerializeField]private float currentTimeToMoveUIFromScreen;
    [SerializeField]private float timeWhenFinished;
    [SerializeField]private bool moveUIToScreen;
    [SerializeField]private bool moveUIOffScreen;

    public static Action hasEnteredScreen;
    public static Action hasExitedScreen;

    public enum SelectionUIType
    {
        room,
        shop,
        weapon
    }
    
    public void Awake()
    {
        thisRect = GetComponent<RectTransform>();
        startPos = thisRect.anchoredPosition;
    }

    public void Update()
    {

        
        //Very boilerplatey but it should run.
        if (moveUIToScreen)
        {
            if (currentTimeToMoveUIIntoScreen < timeWhenFinished)
            {
                currentTimeToMoveUIIntoScreen += Time.deltaTime;
                thisRect.anchoredPosition = new Vector2(Mathf.SmoothStep(startPos.x, 0, currentTimeToMoveUIIntoScreen/timeWhenFinished)/*Mathf.Lerp(startPos.x,0,currentTimeToMoveUIIntoPlace/timeWhenFinished)*/, 0);
            }
            else
            {
                moveUIToScreen = false;
                currentTimeToMoveUIIntoScreen = 0;
                hasEnteredScreen?.Invoke();
                
            }
        }

        if (moveUIOffScreen)
        {
            if (currentTimeToMoveUIFromScreen < timeWhenFinished)
            {
                currentTimeToMoveUIFromScreen +=  Time.deltaTime;
                thisRect.anchoredPosition = new Vector2(Mathf.SmoothStep(0, startPos.x, currentTimeToMoveUIFromScreen/timeWhenFinished), 0);
            }
            else
            {
                moveUIOffScreen = false;
                currentTimeToMoveUIFromScreen = 0;
                hasExitedScreen?.Invoke();
            }
        }
        
        
      
      
    }

    public void InstantiateSelectionCards(int numberOfCardsToInstantiate)
    {
        

        for (int i = 0; i < numberOfCardsToInstantiate; i++)
        {
           var cardObject =  Instantiate(prefabType, parentToPopulate);
        }
       
    }

    public void MoveSelectionCardsIntoView()
    {
        moveUIToScreen = true;
        moveUIOffScreen = false;

    }
    
    public void MoveSelectionCardsOutOfView()
    {
        moveUIOffScreen = true;
        moveUIToScreen = false;

    }

    public void DestroySelectionCards()
    {
        for (int i = 0; i < cardObjects.Count; i++)
        {
            Destroy(cardObjects[i]);
        }
        cardObjects.Clear();
    }

}