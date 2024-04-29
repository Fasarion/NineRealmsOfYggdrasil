using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

public class UpgradeCardUIManager : MonoBehaviour
{
    [SerializeField] private List<UpgradeCardUIBehaviour> upgradeCards;
    
    private static UpgradeCardUIManager _instance;

    private bool _isUIDisplayed;

    private UpgradeObject[] _upgradeObjects;

    public Action<int> OnUpgradeChosen;

    public float upgradeUIClickDelay = 0.5f;

    private float _upgradeUIClickDelayTimer = 0;

    private float _cachedTimeStamp;
    
    
    public static UpgradeCardUIManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindObjectOfType<UpgradeCardUIManager>();
            }

            return _instance;
        }
    }

    private void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
        }
        else
        {
            //Destroy(gameObject);
        }
        
        HideUI();
    }

    private void Update()
    {
        _upgradeUIClickDelayTimer = Time.unscaledTime;
    }

    private void DisplayUpgradeCards(UpgradeObject[] upgradeObjects)
    {
        _cachedTimeStamp = Time.unscaledTime;
        
        ShowUI(upgradeObjects.Length);
        
        for (int i = 0; i < upgradeObjects.Length; i++)
        {
            UpgradeObject upg = upgradeObjects[i];
            upgradeCards[i].UpdateCardDisplay(upg);
        }
    }

    private void OnEnable()
    {
        var upgradeUISystem = World.DefaultGameObjectInjectionWorld.GetExistingSystemManaged<UpgradeUISystem>();
        upgradeUISystem.OnUpgradeUIDisplayCall += DisplayUpgradeCards;
    }

    private void OnDisable()
    {
        if (World.DefaultGameObjectInjectionWorld == null) return;
        var upgradeUISystem = World.DefaultGameObjectInjectionWorld.GetExistingSystemManaged<UpgradeUISystem>();
        upgradeUISystem.OnUpgradeUIDisplayCall -= DisplayUpgradeCards;
    }

    private void ShowUI(int cardCount)
    {
        for (int i = 0; i < cardCount; i++)
        {
            upgradeCards[i].gameObject.SetActive(true);
        }

        _isUIDisplayed = true;
        Time.timeScale = 0f;
    }

    private void HideUI()
    {
        foreach (var card in upgradeCards)
        {
            card.gameObject.SetActive(false);
        }

        _isUIDisplayed = false;
        Time.timeScale = 1f;
    }

    public RectTransform[] GetUpgradeCardDimensions(UpgradeObject[] upgradeObjects)
    {
        int cardsToDisplay = upgradeObjects.Length;
        RectTransform[] transforms = new RectTransform[cardsToDisplay];
        for (int i = 0; i < cardsToDisplay; i++)
        {
            transforms[i] = upgradeCards[i].GetComponent<RectTransform>();
        }

        return transforms;
    }
    
    public void RegisterUpgradeCardClick(int index)
    {
        if (_upgradeUIClickDelayTimer < _cachedTimeStamp + upgradeUIClickDelay) return;
        
        HideUI();
        OnUpgradeChosen?.Invoke(index);
    }
}
