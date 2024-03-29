using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;


namespace DS.ScriptableObjects
{
    public class DSDialogueGroupSO : ScriptableObject
    {
        [field: SerializeField] public string GroupName { get; set; }

        public void Initialize(string groupName)
        {
            GroupName = groupName;
        }
    }

}
