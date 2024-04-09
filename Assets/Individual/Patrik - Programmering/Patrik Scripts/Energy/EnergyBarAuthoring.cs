using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnergyBarAuthoring : MonoBehaviour
{
    [Tooltip("How much energy it takes to fill the energy bar.")]
    [SerializeField] private float energyUntilMax = 100;
    
    [Tooltip("How much energy that will fill after a hit.")]
    [SerializeField] private float energyFillPerHit = 5;
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
