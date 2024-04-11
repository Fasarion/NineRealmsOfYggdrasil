using Unity.Burst;
using Unity.Entities;
using UnityEngine;

public partial class PlaySoundsSystem : SystemBase
{
    private AudioManager _audioManager;
    
    protected override void OnUpdate()
    {
        if (!_audioManager)
        {
            _audioManager = AudioManager.Instance;

            if (!_audioManager)
            {
                return;
            }
        }

        var audioBuffer = SystemAPI.GetSingletonBuffer<AudioBufferData>();

        foreach (var audioElement in audioBuffer)
        {
            var audioData = audioElement.AudioData;
            _audioManager.PlayAudioData(audioData);
        }
        
        audioBuffer.Clear();
    }
}