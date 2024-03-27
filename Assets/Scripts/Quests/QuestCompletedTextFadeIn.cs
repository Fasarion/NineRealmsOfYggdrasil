using System;
using System.Collections;
using System.Collections.Generic;
using KKD;
using TMPro;
using UnityEngine;

public class QuestCompletedTextFadeIn : MonoBehaviour
{
    private QuestManager questManager;

    private float fadeTimerEnd = 1;
    private float currentFadeTime;
    private bool timerInverted;
    private TMP_Text text;

    public float changeRate;

    private bool fadeComplete;
    
    private float currentFadePauseDelay;

    public float fadePauseDelay;
    
    // Start is called before the first frame update
    void Start()
    {
        text = GetComponent<TMP_Text>();
        currentFadeTime = 0;
        fadeComplete = true;
        timerInverted = false;
        currentFadePauseDelay = 0;
        //StartFadeEffect();
        text.alpha = currentFadeTime;
    }

    
    private void OnEnable()
    {
        QuestHandler.onQuestCompleted += OnQuestCompleted;
    }

    private void OnDisable()
    {
        QuestHandler.onQuestCompleted -= OnQuestCompleted;
    }

    // Update is called once per frame
    void Update()
    {
        RunFadeEffect();
    }
    
    public void OnQuestCompleted(QuestHandler handler)
    {
        if (handler.questComplete)
        {
            StartFadeEffect();
        }
    }

    public void RunFadeEffect()
    {
        
        //This is ugly code, but it runs
        if (fadeComplete)
        {
            return;
        }
        
        if (timerInverted)
        {
            if (currentFadeTime > 0)
            {
                currentFadeTime -= changeRate * Time.deltaTime;
            }
            else
            {
                fadeComplete = true;
            }
        }
        else
        {
            
            if (currentFadeTime < fadeTimerEnd)
            {
                currentFadeTime += changeRate * Time.deltaTime;
            }
            else
            {
                currentFadePauseDelay += changeRate * Time.deltaTime;

            }
        }

        if (currentFadePauseDelay > fadePauseDelay)
        {
            timerInverted = true;
        }
        
        var transparency = Mathf.Lerp(0, fadeTimerEnd, currentFadeTime);

        text.alpha = transparency;
    }
    public void StartFadeEffect()
    {
        currentFadeTime = 0;
        fadeComplete = false;
        timerInverted = false;
        currentFadePauseDelay = 0;


    }
}
