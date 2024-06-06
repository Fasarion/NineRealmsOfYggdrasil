using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Entities;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using SceneReference = DevLocker.Utils.SceneReference;

public class SceneHandler : MonoBehaviour
{
    public SceneReference sceneToLoad;
    
    public void SwapScene()
    {
        SceneManager.LoadScene(sceneToLoad.ScenePath);
    }
    
    public void CleaECS()
    {
        

        var defaultWorld = World.DefaultGameObjectInjectionWorld;
        defaultWorld.EntityManager.CompleteAllTrackedJobs();;
        foreach (var system in defaultWorld.Systems)
        {
            system.Enabled = false;
        }

        //var entities = defaultWorld.EntityManager.GetAllEntities(Allocator.Temp);
        
        //defaultWorld.EntityManager.DestroyEntity(EntityManager.UniversalQuery);
        
        defaultWorld.Dispose();
        DefaultWorldInitialization.Initialize("Default World", false);
        if (!ScriptBehaviourUpdateOrder.IsWorldInCurrentPlayerLoop(World.DefaultGameObjectInjectionWorld))
        {
            ScriptBehaviourUpdateOrder.AppendWorldToCurrentPlayerLoop(World.DefaultGameObjectInjectionWorld);
        }  
    }

    public void ExitGame()
    {
    
        #if UNITY_EDITOR
            EditorUtility.DisplayDialog("Great!", "You got the pattern right!", "Next Level!");
            if (Application.isEditor)
            {
                EditorApplication.isPlaying = false;
            }
      
           
        #endif
        #if !UNITY_EDITOR
        Application.Quit();
        #endif
       
        
    }
}

public partial class ResetWorldSystem : SystemBase
{
    protected override void OnCreate()
    {
        //EventManager.OnExit += OnExit;
    }
    
    protected override void OnStopRunning()
    {
        //EventManager.OnExit -= OnExit;
    }

    private void OnExit()
    {
        Debug.Log("DESTROY WORLD"); 
        
        var defaultWorld = World.DefaultGameObjectInjectionWorld;
        // defaultWorld.EntityManager.CompleteAllTrackedJobs();;
        // foreach (var system in defaultWorld.Systems)
        // {
        //     system.Enabled = false;
        // }

        //var entities = defaultWorld.EntityManager.GetAllEntities(Allocator.Temp);
        
        defaultWorld.EntityManager.DestroyEntity(EntityManager.UniversalQuery);
        
        defaultWorld.Dispose();
        // DefaultWorldInitialization.Initialize("Default World", false);
        // if (!ScriptBehaviourUpdateOrder.IsWorldInCurrentPlayerLoop(World.DefaultGameObjectInjectionWorld))
        // {
        //     ScriptBehaviourUpdateOrder.AppendWorldToCurrentPlayerLoop(World.DefaultGameObjectInjectionWorld);
        // } 
        
    }

    protected override void OnUpdate()
    {
    }
}
