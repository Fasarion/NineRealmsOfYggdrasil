using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuestCollectible : MonoBehaviour
{
    public MonoBehaviour identityBehaviour;

    public QuestManager questManager;

    public bool destroyCollectibleOnPickup;
    //public QuestCollectionEventHandler collectionEventHandler;
    public void OnTriggerEnter(Collider other)
    {
        
        if (other.CompareTag("Player"))
        {
            questManager.CheckCollectible(identityBehaviour);
            if (destroyCollectibleOnPickup)
            {
                Destroy(gameObject);
            }
        }
    }
}
