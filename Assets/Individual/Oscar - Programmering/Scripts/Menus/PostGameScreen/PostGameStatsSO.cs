using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "PostGameStats", menuName = "ScriptableObjects/PostGameStats")]
public class PostGameStatsSO : ScriptableObject
{
    public bool hasWon;
    public int enemyKills;
    public int playTimeInSeconds;
}
