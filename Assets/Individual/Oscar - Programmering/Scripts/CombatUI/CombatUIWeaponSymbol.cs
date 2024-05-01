using System.Collections;
using System.Collections.Generic;
using Patrik;
using UnityEngine;
using UnityEngine.UI;

public class CombatUIWeaponSymbol : MonoBehaviour
{
    private enum SymbolType
    {
        Main,
        LeftInactive,
        RightInactive
    }

    public CombatUISymbolHolder symbolHolder;
    
    
    //public Sprite spriteToUpdateTo;
    public Image normalImage;
    public Image ultImage;
    public Image specialImage;
    public Image passiveImage;

    [SerializeField]private SymbolType symbolType;
    
    protected Sprite currentlySelectedUltSymbol;
    protected Sprite currentlySelectedNormalSymbol;
    protected Sprite currentlySelectedSpecialSymbol;
    protected Sprite currentlySelectedPassiveSymbol;
    
    // Make sure to update so it get the current weapon from the actual current weapon
    public virtual void Start()
    {
        currentlySelectedNormalSymbol = symbolHolder.hammerSymbols[0];
        currentlySelectedUltSymbol = symbolHolder.hammerSymbols[1];
        currentlySelectedSpecialSymbol = symbolHolder.hammerSymbols[2];
        currentlySelectedPassiveSymbol = symbolHolder.hammerSymbols[3];
        
        SetSymbol(normalImage, currentlySelectedNormalSymbol);
        SetSymbol(ultImage, currentlySelectedUltSymbol);
       
        SetSymbol(specialImage, currentlySelectedSpecialSymbol);
        SetSymbol(passiveImage, currentlySelectedPassiveSymbol);
    }
    
    public void OnEnable()
    {
        CombatUIWeaponHandler.onCurrentWeaponUpdated += OnCurrentWeaponUpdated;
    }
    
    public void OnDisable()
    {
        CombatUIWeaponHandler.onCurrentWeaponUpdated -= OnCurrentWeaponUpdated;
    }
    
    private void OnCurrentWeaponUpdated(WeaponType weaponType, WeaponType currentLeftInactiveWeapon, WeaponType currentRightInactiveWeapon)
    {
        switch (symbolType)
        {
            case SymbolType.Main:
            {
                SetMainWeaponType(weaponType);
                break;
            }    
            case SymbolType.LeftInactive:
            {
                SetInactiveWeaponType(currentLeftInactiveWeapon);
                break;
            } 
            case SymbolType.RightInactive:
            {
                SetInactiveWeaponType(currentRightInactiveWeapon);
                break;
            } 
        }
      
        
        //imageTransform.sizeDelta = new Vector2(rect.width, rect.height);
    }

    public void SetMainWeaponType(WeaponType weaponType)
    {
        switch (weaponType)
        {
            case WeaponType.Hammer:
            {
                currentlySelectedNormalSymbol = symbolHolder.hammerSymbols[0];
                currentlySelectedUltSymbol = symbolHolder.hammerSymbols[1];
                currentlySelectedSpecialSymbol = symbolHolder.hammerSymbols[2];
                break;
            }
            case WeaponType.Sword:
            {
                currentlySelectedNormalSymbol = symbolHolder.swordSymbols[0];
                currentlySelectedUltSymbol = symbolHolder.swordSymbols[1];
                currentlySelectedSpecialSymbol = symbolHolder.swordSymbols[2];
                break;
            }
            case WeaponType.Mead:
            {
                currentlySelectedNormalSymbol = symbolHolder.meadSymbols[0];
                currentlySelectedUltSymbol = symbolHolder.meadSymbols[1];
                currentlySelectedSpecialSymbol = symbolHolder.meadSymbols[2];
                break;
            }
           
            case WeaponType.Birds:
            {
                currentlySelectedNormalSymbol = symbolHolder.birdSymbols[0];
                currentlySelectedUltSymbol = symbolHolder.birdSymbols[1];
                currentlySelectedSpecialSymbol = symbolHolder.birdSymbols[2];
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
        
        SetSymbol(ultImage, currentlySelectedUltSymbol);
        SetSymbol(normalImage, currentlySelectedNormalSymbol);
        SetSymbol(specialImage, currentlySelectedSpecialSymbol);
        
       
    }
    
    
    public void SetInactiveWeaponType(WeaponType weaponType)
    {
        switch (weaponType)
        {
            case WeaponType.Hammer:
            {
                currentlySelectedPassiveSymbol = symbolHolder.hammerSymbols[3];
                
                break;
            }
            case WeaponType.Sword:
            {
                currentlySelectedPassiveSymbol = symbolHolder.swordSymbols[3];
                break;
            }
            case WeaponType.Mead:
            {
                currentlySelectedPassiveSymbol = symbolHolder.meadSymbols[3];
                break;
            }
           
            case WeaponType.Birds:
            {
                currentlySelectedPassiveSymbol = symbolHolder.birdSymbols[3];
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
        SetSymbol(passiveImage, currentlySelectedPassiveSymbol);
       
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
