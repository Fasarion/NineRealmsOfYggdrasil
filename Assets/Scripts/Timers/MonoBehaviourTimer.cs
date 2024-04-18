using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class MonoBehaviourTimer : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI timerText;
    private float _currentTimerTime;
    
    
    void Start()
    {
        timerText = GetComponent<TextMeshProUGUI>();
        
        _currentTimerTime = 0;
    }
    
    void Update()
    {
        _currentTimerTime += Time.deltaTime;
        string formattedFloat = _currentTimerTime.ToString("F1");
        timerText.text = formattedFloat;
    }
}
