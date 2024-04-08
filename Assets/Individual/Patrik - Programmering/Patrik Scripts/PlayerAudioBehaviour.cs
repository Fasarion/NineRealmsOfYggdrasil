using UnityEngine;

namespace Patrik
{
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
                Debug.LogError("Missing Audio Manager, no sound will be played.");
                return;
            }
            _audioManager.weaponAudio.WeaponSwingAudio(weapon, attackType);
        }
    }
}