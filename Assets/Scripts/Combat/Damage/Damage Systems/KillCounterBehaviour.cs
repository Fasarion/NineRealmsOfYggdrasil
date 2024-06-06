using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class KillCounterBehaviour : MonoBehaviour
{
    private int kills;
    
    private KillCounterBehaviour killCounterBehaviour;


    
    // Static variable to hold the instance
    private static KillCounterBehaviour _instance;

    // Public property to access the instance
    public static KillCounterBehaviour Instance
    {
        get
        {
            // Check if the instance is null
            if (_instance == null)
            {
                // Find the instance in the scene
                _instance = FindObjectOfType<KillCounterBehaviour>();
            }
            // Return the instance
            return _instance;
        }
    }
    void Start()
    {
        kills = 0;
    }

    public void SetKills(int kills)
    {
        this.kills = kills;
    }
    
    public int GetKills()
    {
        return kills;
    }
    
    private void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
            //DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
    
    void Update()
    {
        
    }
}
