using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Unity.VisualScripting;
using UnityEngine;

public class LevelProgressionManager : MonoBehaviour
{

   public RoomTreeGenerator roomTreeGenerator;
   [SerializeField]private int currentRoomLevel;
   
   
   //This will be defined manually
   [SerializeField] private GameObject contentContainer;
   [SerializeField]private ProgressionBarBehaviour currentTargetObject;
   private List<ProgressionBarBehaviour> objectsInProgressionBar;


   private void UpdateTargetPointer()
   {
       for (int i = 0; i < objectsInProgressionBar.Count; i++)
       {
           if (currentTargetObject.GetType() == typeof(ProgressionBarRoomSymbol))
           {
               var currentNode =  roomTreeGenerator.GetCurrentNode();
               currentRoomLevel = currentNode.roomCoordinates.x;
           }
           else if(currentTargetObject.GetType() == typeof(ProgressionBarShopSymbol))
           {
           
           }
           else if (currentTargetObject.GetType() == typeof(ProgressionBarWeaponSymbol))
           {
           
           }
       }
       
     
     //Counterintuitively, x determines tree depth
        
   }

   private void InstantiateSymbolPrefabs()
   {
       
   }
   public void UpdateTargetObject()
   {
       UpdateTargetPointer();

       if (objectsInProgressionBar != null)
       {
           currentTargetObject = objectsInProgressionBar[currentRoomLevel];
       }
       
   }
   
   
}
