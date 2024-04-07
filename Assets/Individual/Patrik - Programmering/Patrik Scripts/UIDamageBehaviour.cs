using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIDamageBehaviour : MonoBehaviour
{
    public static UIDamageBehaviour Instance { get; private set; }
    
    [Header("Flash Times")]
    [SerializeField] private float flashInTime = 1f;
    [SerializeField] private float flashStayTime = 1f;
    [SerializeField] private float flashOutTime = 1f;
    
    [Header("Components")]
    [SerializeField] private Image flashImage;
    [SerializeField] private Color endColor;

    private bool flashIsActive;

    private void Awake()
    {
        Instance = this;
    }

    public void FlashDamage()
    {
        if (flashIsActive) return;

        StartCoroutine(FlashRoutine());
    }

    private IEnumerator FlashRoutine()
    {
        flashIsActive = true;

        float timer = 0;

        // flash in
        while (timer < flashInTime)
        {
            float t = timer / flashInTime;
            flashImage.color = Color.Lerp(Color.clear, endColor, t);
            
            var deltaTime = Time.deltaTime;
            timer += deltaTime;
            yield return new WaitForEndOfFrame();
        }

        flashImage.color = endColor;
        
        timer = 0;

        // stay
        while (timer < flashStayTime)
        {
            var deltaTime = Time.deltaTime;
            timer += deltaTime;
            yield return new WaitForEndOfFrame();
        }
        
        timer = 0;

        // flash out
        while (timer < flashOutTime)
        {
            float t = timer / flashOutTime;
            flashImage.color = Color.Lerp(endColor, Color.clear, t);
            
            var deltaTime = Time.deltaTime;
            timer += deltaTime;
            yield return new WaitForEndOfFrame();
        }

        flashImage.color = Color.clear;
        flashIsActive = false;
    }
}
