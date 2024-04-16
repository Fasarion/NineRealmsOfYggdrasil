using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class ChoiceObject : ScriptableObject
{
   [Header("--Name--")] 
   [Tooltip("The name of the item in question")]
   public string choiceObjectName;
   
   [Tooltip("A description of the item in question")]
   public string choiceObjectDescription;

   [Tooltip("Sprite showing what the item selected will look like")]
   public Sprite choiceObjectSprite;
}
