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
    public EventReference grunts;
    private EventInstance gruntsIns;
    public EventReference gruntHit;
    private EventInstance gruntHitIns;

    public EventReference gruntBackup;

    private EventInstance gruntBackIns;
    //public EventReference gruntAttack;
    public EventReference ranger;
    //public EventReference rangerAttack;
    [Header("WeaponImpacts")] 
    public EventReference swordImpact;
    public EventReference hammerImpact;

    private int gruntAmount;
    private int rangerAmount;

    public void EnemyAudioCaller(int enemyType)
    {
        //enemytyp, vapentyp?
        switch (enemyType)
        {
            case 0:
            {
                break;
            }
            case 1:
            {
                gruntOnHit();
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

    public void gruntOnHit()
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
        /*gruntHitIns = RuntimeManager.CreateInstance(gruntHit);
        gruntHitIns.setParameterByName("WeaponTest", PlayerAudioBehaviour.GetWeaponTypeAudio());
        gruntHitIns.start();
        gruntHitIns.release();
        Debug.Log("Du kom hit");*/
    }


}
