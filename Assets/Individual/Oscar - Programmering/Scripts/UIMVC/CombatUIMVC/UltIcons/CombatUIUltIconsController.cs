using System.Collections;
using System.Collections.Generic;
using Patrik;
using UnityEngine;

public class CombatUIUltIconsController : BaseControllerMVC
{
   public override void OnNotification(string p_event_path, Object p_target, params object[] p_data)
   {

      var ultIconsModel = app.model.ultIconsModel; 
      var ultIconsView = app.view.ultIconsView;
      switch (p_event_path)
      {
         case NotificationMVC.UltimateUsed:
         {
            WeaponType weaponType = (WeaponType)p_data[0];
            float currentEnergy = (float)p_data[1];
            float maxEnergy = (float)p_data[2];
            if (currentEnergy == 0)
            {
               for (int i = 0; i < ultIconsView.ultWeaponReadyHolderViews.Count; i++)
               {
                  if (ultIconsView.ultWeaponReadyHolderViews[i].currentWeaponType == weaponType)
                  {
                     ultIconsView.ultWeaponReadyHolderViews[i].objectToSet.SetActive(false);
                     ultIconsModel.activeUltCounter--;
                  }
            
               }
            }
        
            //UltimateReady
            if (currentEnergy >= maxEnergy)
            {
               for (int i = 0; i < ultIconsView.ultWeaponReadyHolderViews.Count; i++)
               {
                  if (ultIconsView.ultWeaponReadyHolderViews[i].currentWeaponType == weaponType)
                  {
                     ultIconsView.ultWeaponReadyHolderViews[i].objectToSet.SetActive(true);
                     ultIconsModel.activeUltCounter++;
                  }
               }
            }

            if (ultIconsModel.activeUltCounter > 0)
            {
               ultIconsView.ultimateActiveText.gameObject.SetActive(true);
            }
            else
            {
               ultIconsView.ultimateActiveText.gameObject.SetActive(false);
            }
            break;
         }

         case NotificationMVC.WeaponSetup:
         {
            var weaponBehaviour = (WeaponBehaviour)p_data[0];
            ultIconsModel.weaponTypes.Add(weaponBehaviour.WeaponType); 
            //if (weaponTypes.Count == 2) 
            //{
            //for (int i = 0; i < weaponTypes.Count; i++)
            //{
            switch (weaponBehaviour.WeaponType)
            {
               case WeaponType.Hammer:
               {
                  ultIconsView.ultWeaponReadyHolderViews[ultIconsModel.symbolCounter].imageTarget.sprite = ultIconsView.SymbolHolder.hammerSymbols[1];
                  break;
               }
               case WeaponType.Sword:
               {
                
                  ultIconsView.ultWeaponReadyHolderViews[ultIconsModel.symbolCounter].imageTarget.sprite = ultIconsView.SymbolHolder.swordSymbols[1];
                  break;
               }
               case WeaponType.Birds:
               {
                  ultIconsView.ultWeaponReadyHolderViews[ultIconsModel.symbolCounter].imageTarget.sprite = ultIconsView.SymbolHolder.birdSymbols[1];
                  break;
               }
               case WeaponType.Mead:
               {
                  ultIconsView.ultWeaponReadyHolderViews[ultIconsModel.symbolCounter].imageTarget.sprite = ultIconsView.SymbolHolder.meadSymbols[1];
                  break;
               }
               default:
               {
                  Debug.LogError("Error, the UltWeaponHolder was not updated correctly.");
                  break;
               }
            }
            ultIconsView.ultWeaponReadyHolderViews[ultIconsModel.symbolCounter].currentWeaponType = weaponBehaviour.WeaponType;
            ultIconsView.ultWeaponReadyHolderViews[ultIconsModel.symbolCounter].imageTarget.SetNativeSize();
            ultIconsView.ultWeaponReadyHolderViews[ultIconsModel.symbolCounter].objectToSet.SetActive(false);
            ultIconsModel.symbolCounter++;
            break;
         }
      }
   }
}
