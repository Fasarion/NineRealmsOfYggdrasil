using System.Collections;
using System.Collections.Generic;
using FMOD.Studio;
using FMODUnity;
using UnityEngine;
//using FMODUnity;
//using FMOD.Studio;


[CreateAssetMenu(menuName = "ScriptableObjects/Audio/PlayerAudio")]
public class PlayerAudio : ScriptableObject
{
    [Header("Movement")] 
    public EventReference footsteps;

    [Header("Events")] 
    public EventReference xpGain;

    public EventReference playerDeath;

    private EventInstance xpIns;
    
    private int shepard = 0;
    
    public void movingAudio()
    {
        RuntimeManager.PlayOneShot(footsteps);
    }

    public void xpAudio()
    {
        xpIns = RuntimeManager.CreateInstance(xpGain);
        //start coroutine??
        xpIns.setParameterByName("XPParam", shepard % 25);
        xpIns.start();
        xpIns.release();
        shepard++;
        //value++;
    }

    /*public void stepAudio()
    {
        RuntimeManager.PlayOneshot(footsteps);
    }*/
    
    /*   
       [Header("Movement/VO")] 
       public EventReference footsteps;
   
       public EventReference takingDamage;
   
       public EventReference playerDeath;
       //[FMODUnity.EventRef] public string footsteps = null;
   
       public EventReference dash;
   
       [Header("Enemy")] 
       public EventReference wetDeath;
   
   
       [Header("Boss")] 
       public EventReference death;
   
   */
}
