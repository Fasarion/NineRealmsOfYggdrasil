using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ObjectiveObjectType
{
    Green,
    Purple,
    Blue,
}

[System.Serializable]
public struct ObjectiveObjectSpriteReference
{
    public ObjectiveObjectType type;
    public Sprite sprite;
}

public class PlayerObjectiveDataManager : MonoBehaviour
{
    public PlayerObjectiveDataHolderObject dataHolder;
    public Dictionary<ObjectiveObjectType, int> objectiveObjectsDictionary;
    public List<ObjectiveObjectUIElementBehaviour> uiElements;
    public List<ObjectiveObjectSpriteReference> ObjectiveObjectSpriteReferences;
    public Sprite defaultSprite;
    

    private void Awake()
    {
        objectiveObjectsDictionary = dataHolder.objectiveObjectsDictionary;
        ClearUI();
    }

    private void Update()
    {
        int counter = 0;
        if (objectiveObjectsDictionary != null)
        {
            foreach (KeyValuePair<ObjectiveObjectType, int> pair in objectiveObjectsDictionary)
            {
                uiElements[counter].gameObject.SetActive(true);
                Sprite sprite = GetObjectiveObjectSprite(pair.Key);
                uiElements[counter].PopulateUI(sprite, pair.Value);
                counter++;
            }
        }
    }

    public void ClearUI()
    {
        foreach (var ui in uiElements)
        {
            ui.gameObject.SetActive(false);
        }
    }

    public Sprite GetObjectiveObjectSprite(ObjectiveObjectType type)
    {
        Sprite result = defaultSprite;
        foreach (var obj in ObjectiveObjectSpriteReferences)
        {
            if (obj.type == type)
            {
                result = obj.sprite;
                return result;
            }
        }

        return result;
    }
}
