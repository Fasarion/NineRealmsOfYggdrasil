using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LanguageButtonSetter : MonoBehaviour
{
    public LanguageLocalizerSO languageLocalizerSo;

    public Toggle toggle;
    // Start is called before the first frame update
    public void Start()
    {
        languageLocalizerSo.useSwedish = false;
        //Fetch the Toggle GameObject
        
        //Add listener for when the state of the Toggle changes, and output the state
        toggle.onValueChanged.AddListener(delegate {
            ToggleValueChanged(toggle);
        });
    }

    private void ToggleValueChanged(Toggle change)
    {
        Debug.Log("Toogle is" + change.isOn);
        
        languageLocalizerSo.useSwedish = change.isOn;
        
        Debug.Log("useSwedish is: " + languageLocalizerSo.useSwedish);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
