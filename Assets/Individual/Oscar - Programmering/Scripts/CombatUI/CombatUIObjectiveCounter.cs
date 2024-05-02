using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class CombatUIObjectiveCounter : MonoBehaviour
{
    //This is so very hacky.
    public ObjectiveObjectUIElementBehaviour objectiveObjectUIElementBehaviour;

    public TMP_Text text;
    
    void Update()
    {
        text.text = objectiveObjectUIElementBehaviour.text.text;
    }
}
