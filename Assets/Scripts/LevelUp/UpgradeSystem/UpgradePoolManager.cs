using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;
using System.Linq;

public class UpgradePoolManager : MonoBehaviour
{
    public string folderPath = "Assets/Upgrades/UpgradePool";
    public Dictionary<int, UpgradeObject> upgradePool;
    public HashSet<int> unlockedUpgradeSet;
    public HashSet<int> pickedUpgradeSet;
    public UpgradeObject[] upgradeObjects;
    
    private static UpgradePoolManager _instance;
    
    public static UpgradePoolManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindObjectOfType<UpgradePoolManager>();
                
                if (_instance == null)
                {
                    //GameObject instanceObject = new GameObject("UpgradePoolManager");
                    //_instance = instanceObject.AddComponent<UpgradePoolManager>();
                }
            }

            return _instance;
        }
    }

    private void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
            //DontDestroyOnLoad(gameObject);
        }
        else
        {
            //Destroy(gameObject);
        }
        
        GenerateUpgradePool();
        pickedUpgradeSet = new HashSet<int>();
        unlockedUpgradeSet = new HashSet<int>();
    }
    

    public void GenerateUpgradePool()
    {
        //string[] filePaths = Directory.GetFiles(folderPath, "*.asset");

        int generationIndex = 0;

        upgradePool = new Dictionary<int, UpgradeObject>();

        foreach (UpgradeObject upgradeObject in upgradeObjects)
        {
            //UpgradeObject upgradeObject = AssetDatabase.LoadAssetAtPath<UpgradeObject>(filePath);
            if (upgradeObject != null)
            {
                upgradeObject.upgradeIndex = generationIndex;
                if (upgradeObject.startUnlocked) upgradeObject.isUnlocked = true;
                else upgradeObject.isUnlocked = false;
                upgradePool.Add(generationIndex, upgradeObject);
                generationIndex++;
            }
        }
        
    }

    public void GenerateActiveUpgradesSet()
    {
        UpgradeObject[] upgradeObjects = GetAllUpgradesInPool();
        unlockedUpgradeSet = new HashSet<int>();
        foreach (var upgrade in upgradeObjects)
        {
            if(pickedUpgradeSet.Contains(upgrade.upgradeIndex)) continue;
            
            if (upgrade.isUnlocked)
            {
                unlockedUpgradeSet.Add(upgrade.upgradeIndex);
            }
        }

    }

    public UpgradeObject[] GetAllUpgradesInPool()
    {
        ICollection<UpgradeObject> objects = upgradePool.Values;
        UpgradeObject[] upgradeObjects = objects.ToArray();
        return upgradeObjects;
    }

    private int GetRandomRoll(int upgradeIndecisLength)
    {
        if (upgradeIndecisLength == 1) return 0;
        int result = (int)UnityEngine.Random.Range(0, upgradeIndecisLength);
        return result;
    }
    
    public UpgradeObject[] GetRandomUpgrades(UpgradePoolType poolType)
    {
        if (poolType == UpgradePoolType.HammerUnlock)
        {
            UpgradeObject[] hammerArray = new UpgradeObject[1];
            UpgradeObject unlock = GetUpgradeObjectReferenceByKey(0);
            hammerArray[0] = unlock;
            return hammerArray;
        }
        
        if (poolType == UpgradePoolType.BirdUnlock)
        {
            UpgradeObject[] birdArray = new UpgradeObject[1];
            UpgradeObject unlock = GetUpgradeObjectReferenceByKey(1);
            birdArray[0] = unlock;
            return birdArray;
        }
        
        List<int> upgradeIndecis = GetActiveUpgradesIndecisList();
        int numberOfUpgrades = 3;

        if (upgradeIndecis.Count <= 0) return null;
        if (upgradeIndecis.Count <= 2) numberOfUpgrades = upgradeIndecis.Count;
        UpgradeObject[] upgradeObjects = new UpgradeObject[numberOfUpgrades];

        for (int i = 0; i < numberOfUpgrades; i++)
        {
            int rollResult = GetRandomRoll(upgradeIndecis.Count);

            UpgradeObject resultObject = GetUpgradeObjectReferenceByKey(upgradeIndecis[rollResult]);
            upgradeIndecis.RemoveAt(rollResult);
            upgradeObjects[i] = resultObject;
        }

        return upgradeObjects;
    }

    public List<int> GetActiveUpgradesIndecisList()
    {
        GenerateActiveUpgradesSet();
        List<int> unlockedUpgrades = new List<int>();

        foreach (var index in unlockedUpgradeSet)
        {
            unlockedUpgrades.Add(index);
        }
        
        return unlockedUpgrades;
    }

    public UpgradeObject GetUpgradeObjectReferenceByKey(int key)
    {
        UpgradeObject upgradeObject = upgradePool[key];
        return upgradeObject;
    }

    public void RegisterUpgradeAsPicked(int index)
    {
        pickedUpgradeSet.Add(index);
    }
    
}
