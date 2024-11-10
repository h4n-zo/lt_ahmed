// using UnityEditor;
// using UnityEngine;

// public class WaypointPlacerTool : Editor
// {
//     private static int waypointCounter = 1; 
//     private static Waypoint.WaypointColor selectedColor = Waypoint.WaypointColor.Blue; 

//     [MenuItem("Hanzo/Waypoint/Activate Waypoint Placement")]
//     public static void ActivateWaypointPlacer()
//     {
//         SceneView.duringSceneGui += OnSceneGUI;
//         Debug.Log("Waypoint Placer Tool Activated");
//     }

//     [MenuItem("Hanzo/Waypoint/Deactivate Waypoint Placement")]
//     public static void DeactivateWaypointPlacer()
//     {
//         SceneView.duringSceneGui -= OnSceneGUI;
//         Debug.Log("Waypoint Placer Tool Deactivated");
//     }

//     private static void OnSceneGUI(SceneView sceneView)
//     {
//         Event e = Event.current;
//         if (e.shift && e.command && e.type == EventType.MouseDown && e.button == 0) // Left mouse button
//         {
//             Ray ray = HandleUtility.GUIPointToWorldRay(e.mousePosition);
//             if (Physics.Raycast(ray, out RaycastHit hit))
//             {
//                 PlaceWaypoint(hit.point);
//             }
//             e.Use(); // Mark event as used to prevent further handling by Unity
//         }
//     }

//     // Place waypoint at clicked position
//     private static void PlaceWaypoint(Vector3 position)
//     {
//         // Create a new waypoint at the clicked position with an incremented name
//         GameObject newWaypoint = new GameObject($"Waypoint {waypointCounter++}");
//         newWaypoint.transform.position = position;

//         // Add Waypoint component and set the selected color
//         Waypoint waypointComponent = newWaypoint.AddComponent<Waypoint>();
//         waypointComponent.gizmoColor = selectedColor;

//         // Parent it under the active object (like a manager)
//         GameObject waypointManager = GameObject.FindObjectOfType<WaypointManager>()?.gameObject;
//         if (waypointManager != null)
//         {
//             newWaypoint.transform.parent = waypointManager.transform;
//         }

//         // Register the undo operation for placement
//         Undo.RegisterCreatedObjectUndo(newWaypoint, "Placed Waypoint");
//         Debug.Log($"Placed {newWaypoint.name} at {position}");
//     }
// }
