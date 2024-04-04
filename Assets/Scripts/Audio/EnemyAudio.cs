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
}
