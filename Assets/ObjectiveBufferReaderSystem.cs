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
    
    protected override void OnUpdate()
    {
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
