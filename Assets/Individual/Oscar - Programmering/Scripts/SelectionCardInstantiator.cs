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
    private Vector2 startPos;
    private Vector2 screenCenterPos;
    private float currentTimeToMoveUIIntoPlace;
    [SerializeField]private float timeWhenFinished;
    private bool moveUIToScreen;
    private bool moveUIOffScreen;
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
            if (currentTimeToMoveUIIntoPlace < timeWhenFinished)
            {
                currentTimeToMoveUIIntoPlace += Mathf.Pow(1, 1) * Time.deltaTime;
                thisRect.anchoredPosition = new Vector2(Mathf.SmoothStep(startPos.x, 0, currentTimeToMoveUIIntoPlace/timeWhenFinished)/*Mathf.Lerp(startPos.x,0,currentTimeToMoveUIIntoPlace/timeWhenFinished)*/, 0);
            }
            else
            {
                moveUIToScreen = false;
                currentTimeToMoveUIIntoPlace = 0;
            }
        }

        if (moveUIOffScreen)
        {
            if (currentTimeToMoveUIIntoPlace < timeWhenFinished)
            {
                currentTimeToMoveUIIntoPlace +=  Mathf.Sqrt(Time.deltaTime);
                thisRect.anchoredPosition = new Vector2(Mathf.Lerp(0,startPos.x,currentTimeToMoveUIIntoPlace/timeWhenFinished), 0);
            }
            else
            {
                moveUIOffScreen = false;
                currentTimeToMoveUIIntoPlace = 0;
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
