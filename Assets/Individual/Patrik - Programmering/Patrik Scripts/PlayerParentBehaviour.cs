using UnityEngine;

namespace Patrik
{
    public class PlayerParentBehaviour : MonoBehaviour
    {
        public static PlayerParentBehaviour Instance { get; private set; }

        private void Awake()
        {
            Instance = this;
        }
    }
}