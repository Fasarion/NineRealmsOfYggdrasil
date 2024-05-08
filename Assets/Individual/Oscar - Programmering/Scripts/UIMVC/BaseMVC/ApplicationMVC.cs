using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ApplicationMVC : MonoBehaviour
{
    public ModelMVC model;
    public ViewMVC view;
    public ControllerMVC controller;


    [SerializeField]private BaseControllerMVC[] cachedControllers;

    public void Notify(string p_event_path, Object p_target, params object[] p_data)
    {
        BaseControllerMVC[] controller_list = GetAllControllers();
        foreach (BaseControllerMVC controller in controller_list)
        {
            controller.OnNotification(p_event_path, p_target, p_data);
        }
    }

    public BaseControllerMVC[] GetAllControllers()
    {
        //Works as long as the controller list is not updated at runtime.
        
        if (cachedControllers == null || cachedControllers.Length == 0)
        {
            var controllers = FindObjectsOfType<BaseControllerMVC>();
            cachedControllers = controllers;
        }
        return cachedControllers;
    }
}
public class ElementMVC : MonoBehaviour
{
    public ApplicationMVC app => FindObjectOfType<ApplicationMVC>();
}


