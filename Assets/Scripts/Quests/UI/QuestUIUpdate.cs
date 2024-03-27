using System;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using KKD;
using UnityEngine;

public class QuestUIUpdate : MonoBehaviour
{
    public QuestManager questManager;

    public List<RectTransform> questPanels;
    [NonSerialized]public GameObject questUI;
    public GameObject questPanel;
    public float yOffset;
    public float xOffset;
    public Vector2 startPos;
    
    
    /*public void InstantiateQuestPanels()
    {
        
        for (int i = 0; i < questUIManager.questHandlers.Count; i++)
        {
            if (questUIManager.questHandlers[i].questAccepted == true)
            {
                var questPanelInstance = Instantiate(questPanel, transform);
                questPanelInstance.GetComponent<QuestPanel>().SetQuest(questUIManager.questHandlers[i]);
                var questPanelTransform = questPanelInstance.GetComponent<RectTransform>();
                questPanels.Add(questPanelTransform);
            }
            
           
        }
        RedrawQuestUI();
    }*/
    
    public void AddQuestPanel(QuestHandler handler)
    {
        var questPanelInstance = Instantiate(questPanel, transform);
        var questPanelScript = questPanelInstance.GetComponent<QuestPanel>();
        questPanelScript.SetQuest(handler, this);
        var questPanelTransform = questPanelInstance.GetComponent<RectTransform>();
            questPanels.Add(questPanelTransform);
            
        RedrawQuestUI();
        

    }

    public void RemoveQuestPanel(RectTransform panel)
    {
        questPanels.Remove(panel);
        questPanels.RemoveAll(delegate (RectTransform o) { return o == null; });
        Destroy(panel.gameObject);
        RedrawQuestUI();
    }



    public void RedrawQuestUI()
    {
        for (int i = 0; i < questPanels.Count; i++)
        {
            questPanels[i].anchoredPosition = startPos + new Vector2(xOffset, yOffset) *(i);
        }
    }

    
    
    
    public void Awake()
    {
       //InstantiateQuestPanels();
        
    }
}
