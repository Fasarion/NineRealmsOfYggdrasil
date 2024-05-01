using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu]
public class PlayerObjectiveDataHolderObject : ScriptableObject
{
    public Dictionary<ObjectiveObjectType, int> objectiveObjectsDictionary;
    public Dictionary<ObjectiveObjectType, int> objectiveObjectsNeededDictionary;
    
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

    public void SetUpObjectiveObjectDictionary(ObjectiveObjectDataReference[] dataReferences)
    {
        foreach (var data in dataReferences)
        {
            objectiveObjectsNeededDictionary.Add(data.type, data.neededAmount);
        }
    }

    public bool CheckIfObjectiveReached()
    {
        foreach (var pair in objectiveObjectsNeededDictionary)
        {
            if (objectiveObjectsDictionary.ContainsKey(pair.Key))
            {
                if (objectiveObjectsDictionary[pair.Key] >= objectiveObjectsNeededDictionary[pair.Key])
                {
                    return true;
                }
            }
        }

        return false;
    }

    public void ClearPlayerInventory()
    {
        objectiveObjectsDictionary = new Dictionary<ObjectiveObjectType, int>();
    }

}
