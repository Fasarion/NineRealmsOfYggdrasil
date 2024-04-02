using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UpgradeCardUIManager : MonoBehaviour
{
    [SerializeField] private List<UpgradeCardUIBehaviour> upgradeCards;
    
    private static UpgradeCardUIManager _instance;
    
    public static UpgradeCardUIManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindObjectOfType<UpgradeCardUIManager>();
                
                if (_instance == null)
                {
                    GameObject instanceObject = new GameObject("UpgradeCardUIManager");
                    _instance = instanceObject.AddComponent<UpgradeCardUIManager>();
                }
            }

            return _instance;
        }
    }

    private void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void OnEnable()
    {
        
    }

    private void OnDisable()
    {
        
    }
}
