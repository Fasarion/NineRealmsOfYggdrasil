using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponSelectionTester : MonoBehaviour
{
    private int indexCount = 2;
    public void UpdateWeaponNumber()
    {
        EventManager.OnWeaponCountSet?.Invoke(indexCount);
        if (indexCount < 3)
        {
            indexCount++;
        }
    }
}
