using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UpgradeCardUIBehaviour : MonoBehaviour
{
    private string _cardTitle;
    private string _cardDescription;
    private bool _isHovered;
    private Sprite _borderImage = null;
    private int _upgradeObjectIndex;

    public void PopulateDisplayValues(UpgradeObject upgradeObject)
    {
        _cardTitle = upgradeObject.upgradeTitle;
        _cardDescription = upgradeObject.upgradeDescription;
        
    }

    public int GetUpgradeObjectIndex()
    {
        return _upgradeObjectIndex;
    }

    private void UpdateCardDisplay()
    {
        
    }
}
