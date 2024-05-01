using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DebugGizmoVisualizer : MonoBehaviour
{
    public float radius = 1.0f;
    public Color gizmoColor = Color.red;
    public Vector3 center = Vector3.zero;

    private void OnDrawGizmos()
    {
        Gizmos.color = gizmoColor;
        Gizmos.DrawWireSphere(center, radius);
    }
}
