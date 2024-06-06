using System.Collections;
using System.Collections.Generic;
using FMOD.Studio;
using FMODUnity;
using UnityEngine;

public class AudioDataLoader : MonoBehaviour
{
    
    private string musicPath = "vca:/Music";
    private string sfxPath = "vca:/SFX";
    private string masterPath = "vca:/Master";

    public bool loadAudioVolumesFromScriptableObject;
    public VolumeDataScriptableObject volumeDataScriptableObject;

    private VCAName musicName = VCAName.Music;
    private VCAName sfxName = VCAName.SFX;
    
    // Start is called before the first frame update
    void Awake()
    {
        if (loadAudioVolumesFromScriptableObject)
        {
            VCA vcaMusic = RuntimeManager.GetVCA(musicPath);
            VCA vcaSFX = RuntimeManager.GetVCA(sfxPath);
            VCA vcaMaster = RuntimeManager.GetVCA(masterPath);

            float valueMusic = volumeDataScriptableObject.LoadVolume( VCAName.Music);
            float valueSFX = volumeDataScriptableObject.LoadVolume( VCAName.SFX);
            float valueMaster = volumeDataScriptableObject.LoadVolume( VCAName.Master);
        
            vcaMusic.setVolume(valueMusic);
            vcaSFX.setVolume(valueSFX);
            vcaMaster.setVolume(valueMaster);
        }
        
    }
}
