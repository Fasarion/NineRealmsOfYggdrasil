using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class CombatUIUltIconsView : ElementMVC
{
    public TMP_Text ultimateActiveText;
    public CombatUISymbolHolderView SymbolHolderView;
    public List<CombatUIUltWeaponReadyHolderView> ultWeaponReadyHolderViews;
    
    // Start is called before the first frame update
    void Start()
    {
        ultimateActiveText.gameObject.SetActive(false);
    }
}
