using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BaseControllerMVC : ElementMVC
{
    //Ideally we would include a p_identifer for instanced
    public abstract void OnNotification(string p_event_path, Object p_target, params object[] p_data);
}
