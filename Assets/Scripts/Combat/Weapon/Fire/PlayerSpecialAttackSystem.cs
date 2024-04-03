using Player;
using Unity.Entities;
using UnityEngine;

namespace Weapon
{
    public partial struct PlayerSpecialAttackSystem : ISystem
    {
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<PlayerSpecialAttackInput>();
        }
        
        public void OnUpdate(ref SystemState state)
        {
            bool fireButtonPressed = SystemAPI.GetSingleton<PlayerSpecialAttackInput>().FireKeyPressed;
            if (!fireButtonPressed) return;
            
            Debug.Log("Special Attack Button Pressed!");
        }
    }
}