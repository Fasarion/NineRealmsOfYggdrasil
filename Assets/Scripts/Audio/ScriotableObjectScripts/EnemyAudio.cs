using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FMODUnity;
using FMOD.Studio;
using Patrik;

[CreateAssetMenu(menuName = "ScriptableObjects/Audio/EnemyAudio")]
public class EnemyAudio : ScriptableObject
{
    [Header("BaseEnemy")]
    //public EventReference grunts;
    //private EventInstance gruntsIns;
    public EventReference gruntHit;
    private EventInstance gruntHitIns;

    //public EventReference gruntBackup;

    //private EventInstance gruntBackIns;
    //public EventReference gruntAttack;
    //public EventReference ranger;
    //public EventReference rangerAttack;
   
    private int gruntAmount;
    private int rangerAmount;

    public void EnemyAudioCaller(int enemyType, AudioData audioData)
    {
        //enemytyp
        switch (enemyType)
        {
            case 0:
            {
                break;
            }
            case 1:  //grunt
            {
                GruntSoundCaller((int)audioData.audioEventType);
                break;
            }
        }
    }

    public void GruntSoundCaller(int audioEvent)
    {
        switch (audioEvent)
        {
            case 0:
            {
                break;
            }
            case 2:
            {
                RuntimeManager.PlayOneShot(gruntHit);
                break;
            }
        }
    }
    
    public void gruntSound(int nbrGrunts)
    {
        /*gruntsIns = RuntimeManager.CreateInstance(grunts);
        //Instance med parameter ändring i guess? kan också vara en if!!
        //Threshholds för fiende antal, får väl prata med designers/progs
        //kanske threshholds of like 100s? probably easiest
        if (nbrGrunts > gruntAmount) //TODO:Fixa logiken med designers och prog för hur många fiender är rimligt?
        {
            
        }

        gruntsIns = RuntimeManager.CreateInstance(grunts);
        gruntsIns.setParameterByName("idk", 2);*/
        
    }

    //Spelar Ljud med rätt vapen när träffad. OBS JUST NU BROKEN
    /*public void gruntOnHit()
    {
        switch (PlayerAudioBehaviour.GetWeaponTypeAudio())
        {
            case 1:
            {
                RuntimeManager.PlayOneShot(swordImpact);
                break;
            }
            case 2:
            {
                RuntimeManager.PlayOneShot(hammerImpact);
                break;
            }

        }
        gruntHitIns = RuntimeManager.CreateInstance(gruntHit);
        gruntHitIns.setParameterByName("WeaponTest", PlayerAudioBehaviour.GetWeaponTypeAudio());
        gruntHitIns.start();
        gruntHitIns.release();
        Debug.Log("Du kom hit");
    }
    */


}
