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
                Debug.Log("Owie");
                RuntimeManager.PlayOneShot(playerHit);
                break;
            }
        }
    }
    //for footstep sounds, raycast, check when hit ground, what ground
    //do instance, set parameter for ground, play.
    public void PlayFootstepAudio()
    {
        RuntimeManager.PlayOneShot(footsteps);
    }
    

    private float lastXpSoundTime;
    private float shepardResetTime = 3f;
    
    public void XpAudio()
    {
        xpIns = RuntimeManager.CreateInstance(xpGain);

        float timeOfXpSound = Time.time;
        
        // reset shepard
        if (timeOfXpSound > lastXpSoundTime + shepardResetTime)
        {
            shepard = 1;
            
            lastXpSoundTime = timeOfXpSound;
        }
        xpIns.setParameterByName("XPParam", shepard % 25);
            xpIns.start();
            xpIns.release();
            shepard++;
            lastXpSoundTime = timeOfXpSound;

    }

    public void DeathAudio()
    {
        RuntimeManager.PlayOneShot(playerDeath);
        //Snapshot, music stuff here too?
    }


}
