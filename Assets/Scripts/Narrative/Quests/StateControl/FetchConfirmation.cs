using System.Collections;
using System.Collections.Generic;
using KKD;
using UnityEngine;

public class FetchConfirmation : MonoBehaviour
{
    public QuestHandler questHandler;
    // Start is called before the first frame update
    void OnEnable()
    {
        questHandler.ItemFetched();
    }
}
