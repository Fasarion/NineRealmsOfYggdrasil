using UnityEngine;


public class PlayerAudioBehaviour : MonoBehaviour
{
    private AudioManager _audioManager;

    private void Awake()
    {
        _audioManager = AudioManager.Instance;
    }

    public void PlayWeaponSwingAudio(int weapon, int attackType)
    {
        if (_audioManager == null)
        {
            Debug.LogWarning("Missing Audio Manager, no sound will be played.");
            return;
        }
        _audioManager.weaponAudio.WeaponSwingAudio(weapon, attackType);
    }
}