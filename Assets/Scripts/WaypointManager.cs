using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaypointManager : MonoBehaviour
{
    private void OnDrawGizmos()
    {
        Waypoint[] waypoints = GetComponentsInChildren<Waypoint>();

        if (waypoints.Length < 2)
        {
            Debug.LogWarning("There must be at least 2 waypoints to draw connections.");
            return;
        }

        Gizmos.color = Color.white;

        for (int i = 0; i < waypoints.Length - 1; i++)
        {
            Gizmos.DrawLine(waypoints[i].transform.position, waypoints[i + 1].transform.position);
        }

        Gizmos.DrawLine(waypoints[waypoints.Length - 1].transform.position, waypoints[0].transform.position);
    }
}
