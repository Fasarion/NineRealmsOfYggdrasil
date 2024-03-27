using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwordFollowBehaviour : MonoBehaviour
{
    public Transform JointToFollow;

    private void Update()
    {
        this.transform.position = JointToFollow.position;
        this.transform.rotation = JointToFollow.rotation;
    }
}
