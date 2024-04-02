using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnTest : MonoBehaviour
{
    public Transform centerPoint;
    public float innerRadius = 5f; // Inner radius of the torus
    public float outerRadius = 10f; // Outer radius of the torus
    [Range(0, 3f)]
    public float visualizationThickness = 0.05f;

    
    // Visualize the torus in the editor
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;

        // Draw the torus
        for (float theta = 0; theta < Mathf.PI * 2; theta += 0.1f)
        {
            // Calculate x and z coordinates for inner and outer radii
            float xInner = centerPoint.position.x + innerRadius * Mathf.Cos(theta);
            float zInner = centerPoint.position.z + innerRadius * Mathf.Sin(theta);

            float xOuter = centerPoint.position.x + outerRadius * Mathf.Cos(theta);
            float zOuter = centerPoint.position.z + outerRadius * Mathf.Sin(theta);

            // Draw spheres at each point for inner and outer radii
            Vector3 pointInner = new Vector3(xInner, centerPoint.position.y, zInner);
            Vector3 pointOuter = new Vector3(xOuter, centerPoint.position.y, zOuter);
        
            Gizmos.DrawSphere(pointInner, visualizationThickness);
            Gizmos.DrawSphere(pointOuter, visualizationThickness);
        }
    }
}
