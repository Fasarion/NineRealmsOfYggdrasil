using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class testaudioevent : MonoBehaviour
{
    public UnityEvent test;
    // Start is called before the first frame update
    void Start()
    {
        test.Invoke();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
