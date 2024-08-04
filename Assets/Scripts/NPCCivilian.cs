using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public enum NPCState
{
    Idle,
    Patrol,
}

[RequireComponent(typeof(NavMeshAgent))]
public class NPCCivilian : MonoBehaviour
{
    public Animator animator;
    private NavMeshAgent navMeshAgent;

    public Waypoint[] waypoints;
    [SerializeField] private Waypoint currentWaypoint;
    private int currentWaypointIndex = 0;
    private NPCState currentState;

    private void Start()
    {
        navMeshAgent = GetComponent<NavMeshAgent>();
        if (waypoints.Length > 0)
        {
            currentWaypoint = waypoints[currentWaypointIndex];
            StartCoroutine(Patrol());
        }
    }

    private IEnumerator Patrol()
    {
        while (true)
        {
            if (currentState == NPCState.Patrol)
            {
                if (!currentWaypoint.isOccupied && currentWaypoint.Occupy(gameObject))
                {
                    navMeshAgent.SetDestination(currentWaypoint.transform.position);
                    currentState = NPCState.Idle;
                    animator.SetBool("Walk", true);
                }
            }
            else if (currentState == NPCState.Idle)
            {
                if (navMeshAgent.remainingDistance < 0.1f)
                {
                    currentWaypoint.Release();
                    animator.SetBool("Walk", false);
                    yield return new WaitForSeconds(currentWaypoint.idleDuration);
                    currentWaypointIndex = (currentWaypointIndex + 1) % waypoints.Length;
                    currentWaypoint = waypoints[currentWaypointIndex];
                    currentState = NPCState.Patrol;
                }
            }
            yield return null;
        }
    }
}