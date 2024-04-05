using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

public class WeaponStatsConfigAuthoring : MonoBehaviour
{
    [Header("Damage")]
    [SerializeField] private float baseDamage;
    
    class Baker : Baker<WeaponStatsConfigAuthoring>
    {
        public override void Bake(WeaponStatsConfigAuthoring authoring)
        {
            throw new System.NotImplementedException();
        }
    }
}