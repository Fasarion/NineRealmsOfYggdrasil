using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ApplicationMVC : MonoBehaviour
{
    public BounceView bounceView;
    public BounceController bounceController;
    public BounceModel bounceModel;

    public FlashingBoxView flashingBoxView;
    public FlashingBoxController flashingBoxController;
    public FlashingBoxModel flashingBoxModel;
    
    
    [SerializeField]private ControllerMVC[] cachedControllers;

    public void Notify(string p_event_path, Object p_target, params object[] p_data)
    {
        ControllerMVC[] controller_list = GetAllControllers();
        foreach (ControllerMVC controller in controller_list)
        {
            controller.OnNotification(p_event_path, p_target, p_data);
        }
    }

    public ControllerMVC[] GetAllControllers()
    {
        //Works as long as the controller list is not updated at runtime.
        
        if (cachedControllers == null || cachedControllers.Length == 0)
        {
            var controllers = FindObjectsOfType<ControllerMVC>();
            cachedControllers = controllers;
        }
        return cachedControllers;
    }
}
public class ElementMVC : MonoBehaviour
{
    
    public ApplicationMVC app => FindObjectOfType<ApplicationMVC>();
}


