using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SmokeBehaviour : MonoBehaviour
{
    private float _currentTime;
    [SerializeField] private float _lifeTime;
    private void OnEnable()
    {
        _currentTime = 0;
    }

    private void OnDisable()
    {
        _currentTime = 0;
    }

    private void Awake()
    {
        this.gameObject.SetActive(false);
    }

    void Update()
    {
        _currentTime += Time.unscaledDeltaTime;
        if (_currentTime > _lifeTime)
        {
            this.gameObject.SetActive(false);
        }
    }
}
