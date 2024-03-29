using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;


public enum StartUnlockState { StartUnlocked, StartLocked }
public class UpgradeTreeObject : ScriptableObject
{
    [SerializeField] private GameStateHandler gameState;
    public List<UpgradeObject> upgrades = new List<UpgradeObject>();

    public bool unlocked = true;

    public List<UpgradeTreeObject> unlocks= new List<UpgradeTreeObject>();
    public List<UpgradeTreeObject> locks= new List<UpgradeTreeObject>();

    public int currentIndex = 0;

    RarityType rarityType = RarityType.Common;

    public StartUnlockState unlockState;
    
    public int CurrentIndex
    {
        get { return currentIndex; }
        set
        {
            if (currentIndex < upgrades.Count - 1)
            {
                currentIndex = value;
                rarityType = upgrades[currentIndex].rarityType;
                LockAllDependantTrees();
            }
            else
            {
                unlocked = false;
                UnlockAllDependantTrees();
                LockAllDependantTrees();
            }
        }
    }

    public RarityType RarityType
    {
        get { return rarityType; }
    }

    public bool Unlocked
    {
        get { return unlocked; }
        set { unlocked = value; }
    }

    public void Reset()
    {
        currentIndex = 0;

        if(unlockState == StartUnlockState.StartUnlocked)
        {
            this.Unlocked = true;
        }
        else
        {
            this.Unlocked = false;
        }
    }

    public UpgradeObject GetUpgradeObject()
    {
        return upgrades[currentIndex];
    }

    private void UnlockDependantTree(UpgradeTreeObject upgradeTree)
    {
        if (upgradeTree != null)
        {
            upgradeTree.Unlocked = true;
        }
    }

    private void UnlockAllDependantTrees()
    {
        if(unlocks.Count > 0)
        {
            foreach(UpgradeTreeObject treeToUnlock in unlocks)
            {
                treeToUnlock.Unlocked = true;
            }
        }
    }
    
    private void LockAllDependantTrees()
    {
        if(locks.Count > 0)
        {
            foreach(UpgradeTreeObject treeToLock in locks)
            {
                treeToLock.Unlocked = false;
            }
        }
    }


}
