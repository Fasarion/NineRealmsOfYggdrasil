using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class CombatUIObjectiveCounterView : ElementMVC
{
    //This is so very hacky.
    public ObjectiveObjectUIElementBehaviour objectiveObjectUIElementBehaviour;

    public TMP_Text text;
    
    void Update()
    {
        string newText = objectiveObjectUIElementBehaviour.text.text + "/ 20";
        text.text = newText;
    }
}
