using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShopChoiceUIBehaviour : ChoiceUIBehaviour
{
    protected override void PopulateDisplayValues(ChoiceObject newShopChoiceObject)
    {
        ShopChoiceObject shopChoiceObject = (ShopChoiceObject)newShopChoiceObject;
        base.PopulateDisplayValues(shopChoiceObject);
        
    }
    
    public override void UpdateSelectionDisplay(ChoiceObject newShopChoiceObject)
    {
        base.UpdateSelectionDisplay(newShopChoiceObject);
        PopulateDisplayValues(newShopChoiceObject);
    }
    
    public override void RegisterMouseClick()
    {
       
    }
}
