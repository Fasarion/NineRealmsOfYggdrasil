using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class CombatUIGoldCounter : MonoBehaviour
{
    public TMP_Text text;
    
    public void OnGoldUpdated(int gold)
    {
        text.text = gold.ToString();
    }

  
}
