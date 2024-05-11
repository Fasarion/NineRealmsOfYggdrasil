using System.Collections;
using System.Collections.Generic;
using Patrik;
using UnityEngine;
using UnityEngine.UI;

public class CombatUIWeaponSymbolController : BaseControllerMVC
{
    public override void OnNotification(string p_event_path, Object p_target, params object[] p_data)
    {
        
        switch (p_event_path)
        {
            case NotificationMVC.WeaponSymbolCurrentWeaponUpdated:
            {
                var weaponSymbolModel = (CombatUIWeaponSymbolModel)p_target;
                foreach (var combatUIWeaponSymbolView in app.view.combatUIWeaponSymbolViews)
                {
                    var modelIdentifier = (string)p_data[0];
                   
                    var viewIdentifier = combatUIWeaponSymbolView.identifier;
                    if (modelIdentifier == viewIdentifier)
                    {
                        var mainWeaponType = (WeaponType) p_data[1];
                        var currentLeftInactiveWeapon = (WeaponType) p_data[2];
                        var currentRightInactiveWeapon = (WeaponType) p_data[3];
                        switch ( combatUIWeaponSymbolView.weaponSymbolType)
                        {
                            case WeaponSymbolType.Main:
                            {
                                SetMainWeaponType(mainWeaponType,combatUIWeaponSymbolView, weaponSymbolModel);
                                break;
                            }    
                            case WeaponSymbolType.LeftInactive:
                            {
                                SetInactiveWeaponType(currentLeftInactiveWeapon,combatUIWeaponSymbolView, weaponSymbolModel);
                                break;
                            } 
                            case WeaponSymbolType.RightInactive:
                            {
                                SetInactiveWeaponType(currentRightInactiveWeapon,combatUIWeaponSymbolView, weaponSymbolModel);
                                break;
                            } 
                        }
                    }
                    
                }
               
                break;
            }
        }
        
    }
    
    
    public void SetMainWeaponType(WeaponType weaponType, CombatUIWeaponSymbolView weaponSymbolView,CombatUIWeaponSymbolModel weaponSymbolModel)
    {
        
        switch (weaponType)
        {
            case WeaponType.Hammer:
            {
                weaponSymbolModel.currentlySelectedNormalSymbol = weaponSymbolView.symbolHolderView.hammerSymbols[0];
                weaponSymbolModel.currentlySelectedUltSymbol = weaponSymbolView.symbolHolderView.hammerSymbols[1];
                weaponSymbolModel.currentlySelectedSpecialSymbol = weaponSymbolView.symbolHolderView.hammerSymbols[2];
                weaponSymbolView.selectedKeySymbolNumber.text = weaponSymbolView.symbolHolderView.keyTexts[0];
                break;
            }
            case WeaponType.Sword:
            {
                weaponSymbolModel.currentlySelectedNormalSymbol = weaponSymbolView.symbolHolderView.swordSymbols[0];
                weaponSymbolModel.currentlySelectedUltSymbol = weaponSymbolView.symbolHolderView.swordSymbols[1];
                weaponSymbolModel.currentlySelectedSpecialSymbol = weaponSymbolView.symbolHolderView.swordSymbols[2];
                weaponSymbolView.selectedKeySymbolNumber.text = weaponSymbolView.symbolHolderView.keyTexts[1];
                break;
            }
            case WeaponType.Mead:
            {
                weaponSymbolModel.currentlySelectedNormalSymbol = weaponSymbolView.symbolHolderView.meadSymbols[0];
                weaponSymbolModel.currentlySelectedUltSymbol = weaponSymbolView.symbolHolderView.meadSymbols[1];
                weaponSymbolModel.currentlySelectedSpecialSymbol = weaponSymbolView.symbolHolderView.meadSymbols[2];
                break;
            }
           
            case WeaponType.Birds:
            {
                weaponSymbolModel.currentlySelectedNormalSymbol = weaponSymbolView.symbolHolderView.birdSymbols[0];
                weaponSymbolModel.currentlySelectedUltSymbol = weaponSymbolView.symbolHolderView.birdSymbols[1];
                weaponSymbolModel.currentlySelectedSpecialSymbol = weaponSymbolView.symbolHolderView.birdSymbols[2];
                break;
            }
            case WeaponType.None:
            {
                Debug.Log("The weapon enum was none, which should not be possible!");
                break;
            }
            default:
            {
                Debug.Log("The weapon enum was null which should not be possible!");
                break;
            }
            
            
        }
        
        SetSymbol(weaponSymbolView.ultImage, weaponSymbolModel.currentlySelectedUltSymbol);
        SetSymbol(weaponSymbolView.normalImage, weaponSymbolModel.currentlySelectedNormalSymbol);
        SetSymbol(weaponSymbolView.specialImage, weaponSymbolModel.currentlySelectedSpecialSymbol);
        
       
    }
    
    
    public void SetInactiveWeaponType(WeaponType weaponType, CombatUIWeaponSymbolView weaponSymbolView,CombatUIWeaponSymbolModel weaponSymbolModel)
    {
        switch (weaponType)
        {
            case WeaponType.Hammer:
            {
                weaponSymbolModel.currentlySelectedPassiveSymbol = weaponSymbolView.symbolHolderView.hammerSymbols[3];
                weaponSymbolView.selectedKeySymbolNumber.text = weaponSymbolView.symbolHolderView.keyTexts[0];
                
                break;
            }
            case WeaponType.Sword:
            {
                weaponSymbolModel.currentlySelectedPassiveSymbol = weaponSymbolView.symbolHolderView.swordSymbols[3];
                weaponSymbolView.selectedKeySymbolNumber.text = weaponSymbolView.symbolHolderView.keyTexts[1];
                break;
            }
            case WeaponType.Mead:
            {
                weaponSymbolModel.currentlySelectedPassiveSymbol = weaponSymbolView.symbolHolderView.meadSymbols[3];
                break;
            }
           
            case WeaponType.Birds:
            {
                weaponSymbolModel.currentlySelectedPassiveSymbol = weaponSymbolView.symbolHolderView.birdSymbols[3];
                break;
            }
            case WeaponType.None:
            {
                Debug.Log("The weapon enum was none, which should not be possible!");
                break;
            }
            default:
            {
                Debug.Log("The weapon enum was null which should not be possible!");
                break;
            }
            
            
        }
        SetSymbol(weaponSymbolView.passiveImage, weaponSymbolModel.currentlySelectedPassiveSymbol);
       
    }
    
    private void SetSymbol(Image image, Sprite current)
    {
        //This is so that we don't need to set the symbol for the scripts on the inactive weapons.
        if (image != null && current != null)
        {
            image.sprite = current;
            image.SetNativeSize();
        }
       
    }
}
