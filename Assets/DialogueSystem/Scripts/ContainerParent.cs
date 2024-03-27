using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ContainerParent : MonoBehaviour
{
    public List<DialogueContainer> containers;
    public List<DialogueContainer> exitContainers;

    public void Awake()
    {
        containers = new List<DialogueContainer>();
        exitContainers = new List<DialogueContainer>();
    }
}
