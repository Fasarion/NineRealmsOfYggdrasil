using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PostGameStatsBehaviour : MonoBehaviour
{
    public PostGameStatsSO postGameStatsSo;
    public TMP_Text killsText;
    public TMP_Text timeText;
    public TMP_Text winText;
    public TMP_Text loseText;

    public void Start()
    {
        var playTimeInHours = postGameStatsSo.playTime / 60 / 60;
        var playTimeInMinutes = (postGameStatsSo.playTime / 60)% 60;
        var playTimeInSeconds = postGameStatsSo.playTime % 60;
        killsText.text = postGameStatsSo.enemyKills.ToString();
        timeText.text = string.Format("{0:00}:{1:00}:{2:00}",playTimeInHours, playTimeInMinutes, playTimeInSeconds);
        if (postGameStatsSo.hasWon)
        {
            winText.gameObject.SetActive(true);
            loseText.gameObject.SetActive(false);
        }
        else
        {
            winText.gameObject.SetActive(false);
            loseText.gameObject.SetActive(true);
        }
        
    }
}
