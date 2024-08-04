using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Events;

public class ArrowTrigger : MonoBehaviour
{
    public UnityEvent onTrigger;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            onTrigger?.Invoke();
        }
    }

    public GameObject player; // Reference to the player GameObject
    public GameObject lineRendererPrefab; // Reference to the line renderer prefab

    private LineRenderer lineRenderer; // Reference to the instantiated line renderer
    private NavMeshAgent playerNavMeshAgent; // Reference to the player's NavMeshAgent

    void Start()
    {
        // Instantiate the line renderer prefab
        GameObject lineRendererObj = Instantiate(lineRendererPrefab, transform.position, Quaternion.identity);
        lineRenderer = lineRendererObj.GetComponent<LineRenderer>();

        // Get the player's NavMeshAgent component
        playerNavMeshAgent = player.GetComponent<NavMeshAgent>();
    }

    void Update()
    {
        // Check if the player object, line renderer, and playerNavMeshAgent are assigned
        if (player != null && lineRenderer != null && playerNavMeshAgent != null)
        {
            // Calculate the path from the GameObject's position to the player's position
            NavMeshPath path = new NavMeshPath();
            if (NavMesh.CalculatePath(transform.position, player.transform.position, NavMesh.AllAreas, path))
            {
                // Set the positions of the line renderer to follow the calculated path
                lineRenderer.positionCount = path.corners.Length;
                for (int i = 0; i < path.corners.Length; i++)
                {
                    lineRenderer.SetPosition(i, path.corners[i]);
                }
            }
        }
    }

   
}
