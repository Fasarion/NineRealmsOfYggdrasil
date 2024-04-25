using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu]
public class PlayerObjectiveDataHolderObject : ScriptableObject
{
    public Dictionary<ObjectiveObjectType, int> objectiveObjectsDictionary;
    
    public void AddObjectiveObject(ObjectiveObjectType type, int count)
    {
        if (objectiveObjectsDictionary.ContainsKey(type))
        {
            objectiveObjectsDictionary[type] += count;
        }
        else
        {
            objectiveObjectsDictionary.Add(type, count);
        }
    }

    public int GetObjectiveObjectTypeCount(ObjectiveObjectType type)
    {
        int result = 0;
        
        if(!objectiveObjectsDictionary.ContainsKey(type))
        {
            Debug.LogWarning("Player has no object of that type =(");
            return 0;
        }

        result = objectiveObjectsDictionary[type];
        
        return result;
    }

    public void ClearPlayerInventory()
    {
        objectiveObjectsDictionary = new Dictionary<ObjectiveObjectType, int>();
    }

}
