using System;
using System.Collections;
using System.Collections.Generic;
using KKD;
using UnityEngine;

public class QuestsCompletedNeededToWin : MonoBehaviour
{
   
   public List<QuestHandler> handlers;

   public QuestWinScreen winScreen;
   public QuestLoseScreen loseScreen;
   
   public void Awake()
   {
      winScreen = FindObjectOfType<QuestWinScreen>(true);
      loseScreen = FindObjectOfType<QuestLoseScreen>(true);
   }

   public void OnEnable()
   {
      for (int i = 0; i < handlers.Count; i++)
      {
         
      }
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
   
   
    

   public void OnMissionCompleted()
   {
      
   }
}
