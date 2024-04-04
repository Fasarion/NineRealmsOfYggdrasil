using System;
using System.Collections;
using System.Collections.Generic;
using Player;
using UnityEngine;

namespace Patrik
{
    public class SwordBehaviour : WeaponBehaviour
    {
        public static SwordBehaviour Instance;

        private void Awake()
        {
            Instance = this;
        }

    }
}

