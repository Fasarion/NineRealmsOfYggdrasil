using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using FMOD.Studio;
using FMODUnity;


public enum VCAName
    {
        Music,
        SFX,
        Master
    } 


 [RequireComponent(typeof(Slider))]
    public class scr_VcaSlider : MonoBehaviour
    {
        // Vi deklarerar en variabel av vår enum som vi skapade tidigare så att vi kommer åt den i Unity
        public VCAName vcaName;

        // Vi deklarerar en variabel som ska innehålla den specifika slidern i spelets ljudinställningar (options)
        [SerializeField] private Slider m_Slider;

        // Vi deklarerar en variabel som ska innehålla den specifika vca:n vi vill använda för varje unik slider
        // sedan deklarerar vi även tre string-variabler som innehåller sökvägarna till respektive VCA
        private VCA vca;
        private string musicPath = "vca:/Music";
        private string sfxPath = "vca:/SFX";
        private string masterPath = "vca:/Master";

        void Awake()
        {
            // baserat på det val vi gjorde för variabeln vcaName så tilldelar vi en specifik VCA till vår variabel "vca"
            switch (vcaName)
            {
                case VCAName.Music:
                    vca = RuntimeManager.GetVCA(musicPath);
                    break;
                case VCAName.SFX:
                    vca = RuntimeManager.GetVCA(sfxPath);
                    break;
                case VCAName.Master:
                    vca = RuntimeManager.GetVCA(masterPath);
                    break;
            }

            //Ta reda på det nuvarande värdet på denna VCA.
            
            float value;
            vca.getVolume(out value);
            
            //Sätt sedan det nuvarande värdet på VCAn som värde på slidern.
            m_Slider.value = (value);

            //När slidern ändrar värde - kör Methoden "SliderValueChange"
            m_Slider.onValueChanged.AddListener(SliderValueChange);
        }

        void SliderValueChange(float value)
        {
            //Sätt volymen till det värde som slidern har.
            vca.setVolume(value);
        }
    }

