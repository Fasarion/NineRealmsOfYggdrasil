using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class WeaponChoiceUIBehaviour : ChoiceUIBehaviour
{
    //public static Action onWeaponChoiceSelected;

    
    protected override void PopulateDisplayValues(ChoiceObject newWeaponChoiceObject)
    {
        WeaponChoiceObject weaponChoiceObject = (WeaponChoiceObject) newWeaponChoiceObject;
        base.PopulateDisplayValues(weaponChoiceObject);
    }
    
    public override void UpdateSelectionDisplay(ChoiceObject newWeaponChoiceObject)
    {
        base.UpdateSelectionDisplay(newWeaponChoiceObject);
        PopulateDisplayValues(newWeaponChoiceObject);

       
    }
    
    public override void RegisterMouseClick()
    {
        base.RegisterMouseClick();
       
        //onWeaponChoiceSelected?.Invoke();

    }
}
