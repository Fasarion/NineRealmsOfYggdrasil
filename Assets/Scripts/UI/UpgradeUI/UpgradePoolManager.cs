using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UpgradePoolManager : MonoBehaviour
{
    [HideInInspector] public Dictionary<int, UpgradeObject> upgradePool;
    [HideInInspector] public HashSet<int> unlockedItems;

    public void GenerateUpgradePool()
    {
        
    }

    public void GenerateDebugLog()
    {
        
    }

}
