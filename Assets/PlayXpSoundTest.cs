using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayXpSoundTest : MonoBehaviour
{
    public static PlayXpSoundTest Instance;
    
    //[SerializeField] private 

    private void Awake()
    {
        Instance = this;
    }
}
