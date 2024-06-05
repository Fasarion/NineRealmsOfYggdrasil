using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;
using UnityEngine.Serialization;

public enum ObjectiveObjectType
{
    Green,
    Purple,
    Blue,
}

[System.Serializable]
public struct ObjectiveObjectDataReference
{
    public ObjectiveObjectType type;
    public Sprite sprite;
    public int neededAmount;
}

public class PlayerObjectiveDataManager : MonoBehaviour
{
    public PlayerObjectiveDataHolderObject dataHolder;
    public Dictionary<ObjectiveObjectType, int> objectiveObjectsDictionary;
    public List<ObjectiveObjectUIElementBehaviour> uiElements;
    public ObjectiveObjectUIElementBehaviour neededObjectsText;
    public GameObject victoryUI;
    public List<ObjectiveObjectDataReference> objectiveObjectDataReferences;
    public Sprite defaultSprite;
    private bool winState = false;

    private void OnEnable()
    {
        var upgradeUISystem = World.DefaultGameObjectInjectionWorld.GetExistingSystemManaged<ObjectiveBufferReaderSystem>();
        upgradeUISystem.OnObjectiveObjectPickedUp += RecieveObjectivePickups;
    }

    private void OnDisable()
    {
        if (World.DefaultGameObjectInjectionWorld == null) return;
        var upgradeUISystem = World.DefaultGameObjectInjectionWorld.GetExistingSystemManaged<ObjectiveBufferReaderSystem>();
        upgradeUISystem.OnObjectiveObjectPickedUp -= RecieveObjectivePickups;
    }

    private void RecieveObjectivePickups(List<ObjectiveObjectType> types)
    {
        foreach (var type in types)
        {
            dataHolder.AddObjectiveObject(type, 1);
        }
    }

    private void Awake()
    {
        dataHolder.ClearPlayerInventory();
        objectiveObjectsDictionary = dataHolder.objectiveObjectsDictionary;
        ClearUI();
        dataHolder.SetUpObjectiveObjectDictionary(objectiveObjectDataReferences.ToArray());
        victoryUI.gameObject.SetActive(false);
        neededObjectsText.gameObject.SetActive(false);
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
                uiElements[counter].PopulateUI(sprite, pair.Value.ToString());
                counter++;

                neededObjectsText.gameObject.SetActive(true);
                neededObjectsText.PopulateUI(null, "/ " + objectiveObjectDataReferences[0].neededAmount.ToString());
            }
            
        }

        if (dataHolder.CheckIfObjectiveReached() && !winState)
        {
            DisplayWinScreen();
            EventManager.OnObjectiveReached.Invoke();
            this.winState = true;
        }
    }

    private void DisplayWinScreen()
    {
        victoryUI.SetActive(true);
        Time.timeScale = 0;
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
        foreach (var obj in objectiveObjectDataReferences)
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
