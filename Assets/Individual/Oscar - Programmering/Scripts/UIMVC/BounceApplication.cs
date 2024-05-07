using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BounceApplication : MonoBehaviour
{
   
    public BounceModel model;
    public BounceView view;
    public BounceController controller;
    private BounceController[] cachedControllers;

    public void Notify(string p_event_path, Object p_target, params object[] p_data)
    {
        BounceController[] controller_list = GetAllControllers();
        foreach (BounceController controller in controller_list)
        {
            controller.OnNotification(p_event_path, p_target, p_data);
        }
    }

    public BounceController[] GetAllControllers()
    {
        //Works as long as the controller list is not updated at runtime.
        
        if (cachedControllers == null)
        {
            var controllers = FindObjectsOfType<BounceController>();
            cachedControllers = controllers;
        }
        return cachedControllers;
    }
}

public class BounceElement : MonoBehaviour
{
    
    public BounceApplication app => FindObjectOfType<BounceApplication>();
}
