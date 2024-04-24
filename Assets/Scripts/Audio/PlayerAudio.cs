using System.Collections;
using System.Collections.Generic;
using FMOD.Studio;
using FMODUnity;
using UnityEngine;
using UnityEngine.PlayerLoop;

[CreateAssetMenu(menuName = "ScriptableObjects/Audio/PlayerAudio")]
public class PlayerAudio : ScriptableObject
{
    [Header("Movement")] 
    public EventReference footsteps;

    [Header("Events")] 
    public EventReference xpGain;

    public EventReference playerHit;

    public EventReference playerDeath;

    private EventInstance xpIns;
    
    private int shepard = 1;
    
    //for footstep sounds, raycast, check when hit ground, what ground
    //do instance, set parameter for ground, play.

    public void PlayerAudioCaller(int eventType)
    {
        switch (eventType)
        {
            case 0: //none
            {
                break;
            }
            case 1: //HitAudio
            {
                RuntimeManager.PlayOneShot(playerHit);
                break;
            }
        }
    }
    public void movingAudio()
    {
        RuntimeManager.PlayOneShot(footsteps);
    }
    

    private float lastXpSoundTime;
    private float shepardResetTime = 3f;
    
    public void XpAudio()
    {
        xpIns = RuntimeManager.CreateInstance(xpGain);
        //start coroutine??
        
        float timeOfXpSound = Time.time;
        
        // reset shepard
        if (timeOfXpSound > lastXpSoundTime + shepardResetTime)
        {
            shepard = 1;
            
            lastXpSoundTime = timeOfXpSound;
        }
       // else if (timeOfXpSound >= lastXpSoundTime + 0.1f)
       // {
            xpIns.setParameterByName("XPParam", shepard % 25);
            xpIns.start();
            xpIns.release();
            shepard++;
            //Debug.Log(shepard%25);
            lastXpSoundTime = timeOfXpSound;
            
       // }

        /*else
        {
            xpIns.release();
        }
        */


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
