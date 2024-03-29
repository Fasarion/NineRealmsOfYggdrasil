using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DS.Data.Save
{
    public class DSGraphSaveDataSO : ScriptableObject
    {
        [field: SerializeField] public string FileName { get; set; }
        [field: SerializeField] public List<DSGroupSaveData> groups;
        [field: SerializeField] public List<DSNodeSaveData> nodes { get; set; }
        [field: SerializeField] public List<string> oldGroupNames { get; set; }
        [field: SerializeField] public List<string> oldUngroupedNodeNames { get; set; }
        [field: SerializeField] public SerializableDictionary<string, List<string>> OldGroupedNodeNames { get; set; }

        public void Initialize(string fileName)
        {
            FileName = fileName;
            groups = new List<DSGroupSaveData>();
            nodes = new List<DSNodeSaveData>();
            
        }
    }
}

