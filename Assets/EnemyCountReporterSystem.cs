using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

public partial class EnemyCountReporterSystem : SystemBase
{
    public Action<int> OnEnemyCountChanged;
    protected override void OnUpdate()
    {
        
    }
}
