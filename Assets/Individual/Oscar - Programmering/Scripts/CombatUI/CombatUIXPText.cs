using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class CombatUIXPText : MonoBehaviour
{
    public TMP_Text xpText;
    
    // Start is called before the first frame update
    void OnEnable()
    {
        EventManager.OnPlayerExperienceChange += OnPlayerExperienceChange;
    }

    void OnDisable()
    {
        EventManager.OnPlayerExperienceChange -= OnPlayerExperienceChange;
    }
    private void OnPlayerExperienceChange(ExperienceInfo experienceInfo)
    {
        string text = "XP: " + experienceInfo.currentXP + "/" + experienceInfo.experienceNeededToLevelUp;
        xpText.text = text;
    }

    // Update is called once per frame
    
}
