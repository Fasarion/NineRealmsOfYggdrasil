using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwordAnimationController : MonoBehaviour
{
    public SwordActiveWeapon sword;
    [Tooltip("List of values enacted each animation interval. If there are less values in the list than animation interval points the list will loop (which is fine, just be aware of it).")]
    [Header("Mouse over below for details")]
    public List<float> swordAnimationValues = new List<float>();

    private void OnValidate()
    {
        sword.SetAnimationValues(swordAnimationValues);
    }

    private void Start()
    {
        sword.SetAnimationValues(swordAnimationValues);
    }

}
