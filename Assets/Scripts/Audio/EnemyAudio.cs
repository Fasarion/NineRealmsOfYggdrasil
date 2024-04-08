using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FMODUnity;
using FMOD.Studio;

[CreateAssetMenu(menuName = "ScriptableObjects/Audio/EnemyAudio")]
public class EnemyAudio : ScriptableObject
{
    [Header("BaseEnemy")]
    public EventReference grunts;
    private EventInstance gruntsIns;
    //public EventReference gruntAttack;
    public EventReference ranger;
    //public EventReference rangerAttack;
    [Header("WeaponImpacts")] 
    public EventReference swordImpact;

    private int gruntAmount;
    private int rangerAmount;

    public void gruntSound(int nbrGrunts)
    {
        gruntsIns = RuntimeManager.CreateInstance(grunts);
        //Instance med parameter ändring i guess? kan också vara en if!!
        //Threshholds för fiende antal, får väl prata med designers/progs
        //kanske threshholds of like 100s? probably easiest
        if (nbrGrunts > gruntAmount) //TODO:Fixa logiken med designers och prog för hur många fiender är rimligt?
        {
            
        }

        gruntsIns = RuntimeManager.CreateInstance(grunts);
        gruntsIns.setParameterByName("idk", 2);
        
    }

}
