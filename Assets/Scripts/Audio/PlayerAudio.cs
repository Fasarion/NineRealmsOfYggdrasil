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

    private EventInstance xpIns;
    public void movingAudio()
    {
        RuntimeManager.PlayOneShot(footsteps);
    }

    public void xpAudio()
    {
        //if (value > 2) value = 0;
        xpIns = RuntimeManager.CreateInstance(xpGain);
        //RuntimeManager.AttachInstanceToGameObject(test, gameObject.transform);
        //test.setParameterByName("TestParam", value);
        xpIns.start();
        xpIns.release();
        //value++;
    }
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
