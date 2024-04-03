using System;
using System.Collections;
using System.Collections.Generic;
using Player;
using UnityEngine;

namespace Patrik
{
    public class SwordBehaviour : WeaponBehaviour
    {
        [SerializeField] private Transform tip;

        public static SwordBehaviour Instance;
        
        public Transform Tip => tip;
        
        private void Awake()
        {
            Instance = this;
        }

    }
}

