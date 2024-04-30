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
        Left,
        Right
    }
    public List<Sprite> hammerSymbols;
    public List<Sprite> swordSymbols;
    public List<Sprite> meadSymbols;
    public List<Sprite> birdSymbols;
    
    //public Sprite spriteToUpdateTo;
    public Image normalImage;
    public Image ultImage;
    public Image specialImage;

    [SerializeField]private SymbolType symbolType;
    
    protected Sprite currentlySelectedUltSymbol;
    protected Sprite currentlySelectedNormalSymbol;
    protected Sprite currentlySelectedSpecialSymbol;
    
    // Start is called before the first frame update
    public virtual void Start()
    {
        currentlySelectedUltSymbol = hammerSymbols[1];
        currentlySelectedNormalSymbol = hammerSymbols[0];
        currentlySelectedSpecialSymbol = hammerSymbols[2];
        ultImage.sprite = currentlySelectedUltSymbol;
        var rect = ultImage.sprite.rect;
        ultImage.SetNativeSize();
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
                CheckWeaponType(weaponType);
                break;
            }    
            case SymbolType.Left:
            {
                CheckWeaponType(currentLeftInactiveWeapon);
                break;
            } 
            case SymbolType.Right:
            {
                CheckWeaponType(currentRightInactiveWeapon);
                break;
            } 
        }
      
        SetSymbols(ultImage, currentlySelectedUltSymbol);
        SetSymbols(normalImage, currentlySelectedNormalSymbol);
        SetSymbols(specialImage, currentlySelectedSpecialSymbol);
        //imageTransform.sizeDelta = new Vector2(rect.width, rect.height);
    }

    public void CheckWeaponType(WeaponType weaponType)
    {
        switch (weaponType)
        {
            case WeaponType.Hammer:
            {
                currentlySelectedNormalSymbol = hammerSymbols[0];
                currentlySelectedUltSymbol = hammerSymbols[1];
                currentlySelectedSpecialSymbol = hammerSymbols[2];
                break;
            }
            case WeaponType.Sword:
            {
                currentlySelectedNormalSymbol = swordSymbols[0];
                currentlySelectedUltSymbol = swordSymbols[1];
                currentlySelectedSpecialSymbol = swordSymbols[2];
                break;
            }
            case WeaponType.Mead:
            {
                currentlySelectedNormalSymbol = meadSymbols[0];
                currentlySelectedUltSymbol = meadSymbols[1];
                currentlySelectedSpecialSymbol = meadSymbols[2];
                break;
            }
           
            case WeaponType.Birds:
            {
                currentlySelectedNormalSymbol = birdSymbols[0];
                currentlySelectedUltSymbol = birdSymbols[1];
                currentlySelectedSpecialSymbol = birdSymbols[2];
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
        
       
    }
    
    private void SetSymbols(Image image, Sprite current)
    {
        image.sprite = current;
        image.SetNativeSize();
    }

    
    
   
}
