using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting.Dependencies.NCalc;
using UnityEngine;

public class ApplicationMVC : MonoBehaviour
{
    //I think I made the collection classes obsolete by creating a base class of all controllers and then accessing those through the notification system.
    [Header("These ones have to be accessible")]
    public ModelMVC model;
    public ViewMVC view;
    [Header("All controllers are already accessed automatically")]
    public ControllerMVC controller;


    [SerializeField]private BaseControllerMVC[] cachedControllers;

    public void Notify(string p_event_path, Object p_target, params object[] p_data)
    {
        BaseControllerMVC[] controller_list = GetAllControllers();
        foreach (BaseControllerMVC currentController in controller_list)
        {
            currentController.OnNotification(p_event_path, p_target, p_data);
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
    public string identifier;
    public ApplicationMVC app => FindObjectOfType<ApplicationMVC>();
}


