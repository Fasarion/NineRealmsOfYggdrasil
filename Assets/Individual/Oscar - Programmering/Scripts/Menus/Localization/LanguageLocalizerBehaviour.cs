using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LanguageLocalizerBehaviour : MonoBehaviour
{
    public LanguageLocalizerSO languageLocalizerSo;

    private bool useSwedish;
    // Start is called before the first frame update
    void Awake()
    {
        useSwedish = languageLocalizerSo.useSwedish;
    }

    public bool GetLanguage()
    {
        return useSwedish;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
