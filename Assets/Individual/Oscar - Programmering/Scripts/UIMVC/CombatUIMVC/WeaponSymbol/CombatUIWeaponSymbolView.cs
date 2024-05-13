using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CombatUIWeaponSymbolView : ElementMVC
{
    public CombatUISymbolHolderView symbolHolderView;
    
    public Image normalImage;
    public Image ultImage;
    public Image specialImage;
    public Image passiveImage;
    
    public TMP_Text selectedKeySymbolNumber;
    public WeaponSymbolType weaponSymbolType;
}
