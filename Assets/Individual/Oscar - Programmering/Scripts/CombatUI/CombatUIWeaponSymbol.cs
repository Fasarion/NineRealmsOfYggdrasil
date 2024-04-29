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
    public Sprite hammerSymbol;
    public Sprite swordSymbol;
    public Sprite meadSymbol;
    public Sprite birdSymbol;
    
    //public Sprite spriteToUpdateTo;
    public Image image;
    [SerializeField]private SymbolType symbolType;
    
    protected Sprite currentlySelectedWeaponSymbol;
    
    // Start is called before the first frame update
    public virtual void Start()
    {
        currentlySelectedWeaponSymbol = hammerSymbol;
        image.sprite = currentlySelectedWeaponSymbol;
        var rect = image.sprite.rect;
        image.SetNativeSize();
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
      
        image.sprite = currentlySelectedWeaponSymbol;
        //var rect = image.sprite.rect;
        image.SetNativeSize();
        //imageTransform.sizeDelta = new Vector2(rect.width, rect.height);
    }

    public void CheckWeaponType(WeaponType weaponType)
    {
        switch (weaponType)
        {
            case WeaponType.Hammer:
            {
                currentlySelectedWeaponSymbol = hammerSymbol;
                break;
            }
            case WeaponType.Sword:
            {
                currentlySelectedWeaponSymbol = swordSymbol;
                break;
            }
            case WeaponType.Mead:
            {
                currentlySelectedWeaponSymbol = meadSymbol;
                break;
            }
           
            case WeaponType.Birds:
            {
                currentlySelectedWeaponSymbol = birdSymbol;
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

    // Update is called once per frame
    public virtual void Update()
    {
        
    }
    
   
}
