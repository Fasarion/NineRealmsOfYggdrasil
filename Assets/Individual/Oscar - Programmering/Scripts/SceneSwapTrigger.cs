using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneSwapTrigger : MonoBehaviour
{
    
    public void OnTriggerEnter(Collider other)
    {
        if (other.GetType() == typeof(PlayerBehaviour) )
        {
            
        }
    }
}
