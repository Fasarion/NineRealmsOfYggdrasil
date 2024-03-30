using Unity.Entities;
using UnityEngine;

namespace Health
{
    public class HealthBarUI : ICleanupComponentData
    {
        public GameObject Value;
    }
}