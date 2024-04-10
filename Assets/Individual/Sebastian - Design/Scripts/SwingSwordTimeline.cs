using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

public class SwingSwordTimeline : MonoBehaviour
{
    public PlayableDirector timeline;

    void Start()
    {
        timeline = GetComponent<PlayableDirector>();
    }

    void Update()
    {
        if (Input.GetKey(KeyCode.Mouse0))
        {
            timeline.Play();
        }
    }
}
