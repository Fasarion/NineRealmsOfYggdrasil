using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

[UpdateAfter(typeof(ObjectiveObjectSystem))]
public partial class ObjectiveBufferReaderSystem : SystemBase
{
    public Action<List<ObjectiveObjectType>> OnObjectiveObjectPickedUp;
    private List<ObjectiveObjectType> _objectiveObjects = new List<ObjectiveObjectType>();
    private bool isInitialized;
    private float startUpTimer;
    
    protected override void OnUpdate()
    {
        if (!isInitialized)
        {
            if (startUpTimer < 1)
            {
                startUpTimer += SystemAPI.Time.DeltaTime;
                return;
            }

            isInitialized = true;
            return;
        }
        
        var buffer = SystemAPI.GetSingletonBuffer<ObjectivePickupBufferElement>();
        
         if (!buffer.IsEmpty)
         {
             _objectiveObjects.Clear();
             
             foreach (var element in buffer)
             {
                 _objectiveObjects.Add(element.Value);
             }
        
             buffer.Clear();
             
             OnObjectiveObjectPickedUp?.Invoke(_objectiveObjects);
        }
    }
}
