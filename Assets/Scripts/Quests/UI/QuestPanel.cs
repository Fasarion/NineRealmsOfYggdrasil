using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using KKD;
using TMPro;
using UnityEngine;

public class QuestPanel : MonoBehaviour
{
    [SerializeField] private GameObject winScreen;
    [SerializeField] private GameObject loseScreen;
    [SerializeField] private TMP_Text questInstructionsText;
    [SerializeField]private TMP_Text questNumbersText;
    
    [SerializeField]private QuestHandler questHandler;
    private QuestType questType;
    private QuestUIUpdate questUIUpdate;


    private string itemToFetchName;
    private string characterToFetchFrom;
    private string characterToFetchFor;

    private string characterToTalkTo;
    private RectTransform rectTransform;

    public void Awake()
    {
         rectTransform = GetComponent<RectTransform>();
        //healthHandler = FindObjectOfType<HealthHandler>();
    }

    public void Start()
    {
        
    }

    public void SetQuest(QuestHandler questHandler, QuestUIUpdate questUIUpdate)
    {
        this.questUIUpdate = questUIUpdate;
        this.questHandler = questHandler;
        questType = questHandler.questType;
        QuestHandler.onQuestCompleted += OnQuestCompleted;
        
        if (questType == QuestType.KillQuest)
        {
            questInstructionsText.text = "Kill " + questHandler.enemyTypeName + ": ";
            questHandler.onMaxKillsAchieved += OnMaxKillsAchieved;
            questHandler.onKill += OnKillCounterUpdated;
            OnKillCounterUpdated();
            questNumbersText.text = questHandler.GetCurrentKills().ToString(CultureInfo.InvariantCulture) + "/" + questHandler.GetKillGoal();
        }
        else if (questType == QuestType.CollectQuest)
        {
            questInstructionsText.text = "Collect " + questHandler.collectableItemName + ": ";
            questHandler.onMaxItemsCollected += OnMaxItemsCollected;
            questHandler.onCollect += OnCollectCounterUpdated;
            OnCollectCounterUpdated(questHandler);
        }
        else if (questType == QuestType.FetchQuest)
        {
            itemToFetchName = questHandler.GetFetchNames(out characterToFetchFrom, out characterToFetchFor);
            questHandler.onItemFetched += OnItemFetched;
            questInstructionsText.text = "Retrieve: " + itemToFetchName;
            questNumbersText.text = "Retrieve " + itemToFetchName + " from " + characterToFetchFrom + ".";
        }
        else if (questType == QuestType.TalkToQuest)
        {
            characterToTalkTo = questHandler.GetTalkToName();
            questInstructionsText.text = "Talk to: " + characterToTalkTo;
            questNumbersText.text = "";
            
        }
        
        

    }

    private void OnQuestCompleted(QuestHandler handler)
    {
        if (questHandler == handler)
        {
            questUIUpdate.RemoveQuestPanel(rectTransform);
        }
        
    }


    public void OnEnable()
    {
        
        /*if (questType == QuestType.KillQuest)
        {
            questHandler.onMaxKillsAchieved += OnMaxKillsAchieved;
            questHandler.onKill += OnKillCounterUpdated;
        }
        else if (questType == QuestType.CollectQuest)
        {
            questHandler.onMaxItemsCollected += OnMaxItemsCollected;
            questHandler.onCollect += OnCollectCounterUpdated;
        }
        else if (questType == QuestType.FetchQuest)
        {
            questHandler.onItemFetched += OnItemFetched;
        }*/
       
        
    }
    private void OnMaxItemsCollected()
    {
        ActivateWinScreen();
    }
    
    private void OnMaxKillsAchieved()
    {
        ActivateWinScreen();
    }
    
    private void ActivateWinScreen()
    {
        if (winScreen != null)
        {
            winScreen.gameObject.SetActive(true);
        }
    }
    

    private void OnPlayerDeath()
    {
        if (loseScreen != null && winScreen.gameObject.activeSelf == false)
        {
            loseScreen.gameObject.SetActive(true);
        }
    }
    

    private void OnItemFetched()
    {
        questNumbersText.text = "Retrieved: " + itemToFetchName + ". Return to " + characterToFetchFor + ".";
        
    }

    private void OnKillCounterUpdated()
    {
        questNumbersText.text = questHandler.GetCurrentKills().ToString(CultureInfo.InvariantCulture) + "/" + questHandler.GetKillGoal();
    }

    private void OnCollectCounterUpdated(QuestHandler handler)
    {
        if (handler == questHandler)
        {
            questNumbersText.text = questHandler.GetCurrentCollectedItems().ToString(CultureInfo.InvariantCulture) + "/" + questHandler.GetCollectGoal();
        }
        
    }

  

    public void OnDisable()
    {
        QuestHandler.onQuestCompleted -= OnQuestCompleted;
        if (questType == QuestType.KillQuest)
        {
            questHandler.onMaxKillsAchieved -= OnMaxKillsAchieved;
            questHandler.onKill -= OnKillCounterUpdated;
        }
        else if (questType == QuestType.CollectQuest)
        {
            questHandler.onMaxItemsCollected -= OnMaxItemsCollected;
            questHandler.onCollect -= OnCollectCounterUpdated;
        }
        else if (questType == QuestType.FetchQuest)
        {
            questHandler.onItemFetched -= OnItemFetched;
        }
    }
}
