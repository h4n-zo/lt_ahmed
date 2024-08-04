using UnityEngine;

public class Waypoint : MonoBehaviour
{
    public enum WaypointColor
    {
        Blue,
        Red,
        Orange,
        Green
    }

    public WaypointColor gizmoColor = WaypointColor.Blue;
    public float gizmoSize = 0.5f;

    // Time each character spends at this waypoint
    public float idleDuration = 10f;

    public bool isOccupied = false;

    [SerializeField] private GameObject visitingNPC;

    private void OnDrawGizmos()
    {
        Color color;
        switch (gizmoColor)
        {
            case WaypointColor.Red:
                color = Color.red;
                break;
            case WaypointColor.Orange:
                color = new Color(1f, 0.5f, 0f); // Orange
                break;
            case WaypointColor.Green:
                color = Color.green;
                break;
            default:
                color = Color.blue;
                break;
        }

        Gizmos.color = color;
        Gizmos.DrawSphere(transform.position, gizmoSize);
    }

    public bool Occupy(GameObject npc)
    {
        if (!isOccupied)
        {
            isOccupied = true;
            visitingNPC = npc;
            return true;
        }
        else
        {
            return false;
        }
    }

    // Release waypoint when the visiting NPC dies
    public void OnNPCDeath(MonoBehaviour npc)
    {
        if (visitingNPC == npc.gameObject)
        {
            Release();
        }
    }

    public void Release()
    {
        isOccupied = false;
        visitingNPC = null;
    }


    public bool IsOccupied()
    {
        return isOccupied;
    }

    public GameObject GetVisitingNPC()
    {
        return visitingNPC;
    }

}
