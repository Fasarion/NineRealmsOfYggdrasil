using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.Audio;

[System.Serializable]

public class Sentence
{

    
    //In order to focus on the textinput from the dialogue system we will remove these for now.
    public string name;
    
    public Color textColor = Color.white;
    [TextArea(3, 10)] public string text;
    public AudioClip voiceLine;
    
    public Sentence(string text)
    {
        textColor = Color.black;
        
        this.text = text;
    }
}
