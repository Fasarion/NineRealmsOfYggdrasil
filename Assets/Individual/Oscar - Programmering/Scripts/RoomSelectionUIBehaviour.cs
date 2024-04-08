using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class RoomSelectionUIBehaviour : MonoBehaviour
{

    public RoomChoiceObject roomChoiceObject;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    public void OnNextSceneButtonPressed()
    {
        SceneManager.LoadScene(roomChoiceObject.sceneReference.SceneName);
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
