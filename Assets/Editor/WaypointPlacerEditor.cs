using UnityEditor;
using UnityEngine;
// using Hanzo.Utils;

[CustomEditor(typeof(WaypointManager))]
public class WaypointPlacerEditor : Editor
{
    private WaypointManager waypointManager;
    private bool isPlacingWaypoint = false;
    private int waypointCounter = 1; // Counter for naming waypoints
    private Waypoint.WaypointColor selectedColor = Waypoint.WaypointColor.Blue; // Default waypoint color

    // Adds Waypoint Placer Tool to the Tools menu
    [MenuItem("Tools/Activate Waypoint Placement")]
    public static void ActivateWaypointPlacement()
    {
        WaypointManager waypointManager = FindObjectOfType<WaypointManager>();

        if (waypointManager != null)
        {
            Selection.activeObject = waypointManager; // Select WaypointManager
            EditorGUIUtility.PingObject(waypointManager); // Ping it in the editor
        }
        else
        {
            Debug.LogWarning("No WaypointManager found in the scene.");
        }
    }

    private void OnEnable()
    {
        waypointManager = (WaypointManager)target;
        SceneView.duringSceneGui += OnSceneGUI;
    }

    private void OnDisable()
    {
        SceneView.duringSceneGui -= OnSceneGUI;
    }

    private void OnSceneGUI(SceneView sceneView)
    {
        // Check for Shift + Cmd + Mouse Down
        Event e = Event.current;

        if (e.shift && e.command && e.type == EventType.MouseDown && e.button == 0) // Left mouse button
        {
            Ray ray = HandleUtility.GUIPointToWorldRay(e.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hit))
            {
                PlaceWaypoint(hit.point);
            }
            e.Use(); // Mark the event as used so Unity doesn't process it further
        }
    }

    private void PlaceWaypoint(Vector3 position)
    {
        // Create a new waypoint at the clicked position with incremented name
        GameObject newWaypoint = new GameObject($"Waypoint {waypointCounter++}");
        newWaypoint.transform.position = position;
        Waypoint waypointComponent = newWaypoint.AddComponent<Waypoint>();

        // Set selected color
        waypointComponent.gizmoColor = selectedColor;

        newWaypoint.transform.parent = waypointManager.transform;

        // Register the undo operation
        Undo.RegisterCreatedObjectUndo(newWaypoint, "Placed Waypoint");
    }

    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        // Dropdown for selecting waypoint color
        selectedColor = (Waypoint.WaypointColor)EditorGUILayout.EnumPopup("Waypoint Color", selectedColor);
        
        // Option to reset the waypoint counter
        if (GUILayout.Button("Reset Waypoint Counter"))
        {
            waypointCounter = 1;
        }
    }
}
