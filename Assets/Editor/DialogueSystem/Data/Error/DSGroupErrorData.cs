using System.Collections;
using System.Collections.Generic;
using DS.Elements;
using UnityEditor.Experimental.GraphView;

using UnityEngine;


namespace DS.Data.Error
{
    public class DSGroupErrorData
    {
        public DSErrorData ErrorData { get; set; }
        public List<DSGroup> Groups { get; set; }

        public DSGroupErrorData()
        {
            ErrorData = new DSErrorData();
            Groups = new List<DSGroup>();
            
        }
    }
}

