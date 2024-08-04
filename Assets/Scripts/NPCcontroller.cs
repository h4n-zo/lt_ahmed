using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;
using System.Linq;

//FINITE STATE MACHINE
public enum EnemyState
{
    Idle,
    Patrol,
    Alert,
    Chase,
    SoundDetected,
    ShootAtSight
}

[RequireComponent(typeof(NavMeshAgent))]
public class NPCController : MonoBehaviour, IHear
{
    #region Variables

    public EnemyState enemyState;
    public GameObject fovMesh;

    [Header("Minimap Features")]
    public SpriteRenderer minimapIcon;
    public Color aliveColor;
    public Color alertColor;
    public Color deathColor;

    [Header("Icon Features")]
    public GameObject alertIcon;
    public GameObject chaseTargetIcon;
    public GameObject soundIcon;

    [Header("Animation Features")]
    private const string BLENDSTATE = "Speed";
    [SerializeField]private NavMeshAgent navMeshAgent;
    [HideInInspector] public Animator animator;

    private float idleTimer = 0f;
    private bool isIdling = false;

    [SerializeField] private float alertDuration = 2.345f; // Adjust as needed
    private float alertTimer = 0f;
    private bool isAlerting = false;

    public Waypoint[] waypoints;
    [SerializeField] private Waypoint currentWaypoint;
    private int currentWaypointIndex = 0;

    [Tooltip("Alert and Detection Features")]
    private Transform target;
    private bool isChasing = false;
    private bool detectPlayer = false;
    [HideInInspector] public float patrolSpeed;
    [HideInInspector] public float chaseSpeed = 4.1f;

    private Vector3 lastKnownPosition;
    private float chaseDistanceThreshold = 28f;
    private float distanceCovered = 0f;

    private float noiseResponseDistance = 10f;

    [HideInInspector] public bool heardSomething = false;
    [HideInInspector] public bool isDead;

    public bool isFiring = false;
    [HideInInspector] public float fireRate = 60f;
    private float nextTimeToFire = 0f;
    private Weapon weapon;

    public static List<NPCController> allNPCs = new List<NPCController>();

    #endregion

    [HideInInspector] public float initialPatrolSpeed;
    [HideInInspector] public float initialChaseSpeed;
    [HideInInspector] public float initialAcceleration;
    [HideInInspector] public float initialAnimatorSpeed;

    // Start is called before the first frame update
    void Start()
    {
        minimapIcon.color = aliveColor;


        weapon = GetComponent<Weapon>();
        navMeshAgent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
        patrolSpeed = navMeshAgent.speed;
        initialPatrolSpeed = patrolSpeed;
        initialChaseSpeed = chaseSpeed;
        initialAnimatorSpeed = animator.speed;
        initialAcceleration = navMeshAgent.acceleration;

        target = GameObject.FindGameObjectWithTag("Player").transform;

        if (waypoints.Length > 0)
        {
            SetNextWaypointIndex();
            PatrolToDestination();
        }
        else
        {
            Debug.LogError("No waypoints assigned to NPCController script on " + gameObject.name);
        }

        allNPCs = FindObjectsOfType<NPCController>().Where(npc => npc != this).ToList();


    }

    // Update is called once per frame
    void Update()
    {

        // Check if the NPC is in the SoundDetected state
        if (enemyState != EnemyState.SoundDetected)
        {
            #region Locomotion
            if (!isIdling && !currentWaypoint.IsOccupied())
            {
                currentWaypoint.Occupy(gameObject);
            }
            else if (!isIdling && currentWaypoint.IsOccupied() && currentWaypoint.GetVisitingNPC() != gameObject)
            {
                // Find another unoccupied waypoint
                FindUnoccupiedWaypoint();
            }


            if (navMeshAgent.enabled && !navMeshAgent.pathPending && navMeshAgent.remainingDistance <= navMeshAgent.stoppingDistance)
            {
                if (!isIdling)
                {
                    animator.SetFloat(BLENDSTATE, 0f);
                    isIdling = true;
                    idleTimer = 0f;
                }
                else
                {
                    idleTimer += Time.deltaTime;
                    if (idleTimer >= currentWaypoint.idleDuration)
                    {
                        isIdling = false;
                        // Release the current waypoint
                        currentWaypoint.Release();
                        SetNextWaypointIndex();
                        SelectedState();
                    }
                }
            }
            #endregion

            // Check for player detection and initiate alerting if detected
            if (detectPlayer && !isChasing && enemyState != EnemyState.Alert && enemyState != EnemyState.ShootAtSight)
            {
                StartAlerting();
            }

            // Handle alerting behavior
            if (isAlerting)
            {
                alertTimer += Time.deltaTime;
                if (alertTimer >= alertDuration)
                {
                    alertTimer = 0f;
                    isAlerting = false;
                    isFiring = true;
                    // StartChasing();

                }
            }

            if (isFiring == true && Time.time >= nextTimeToFire)
            {
                alertTimer = 0f;
                isAlerting = false;
                nextTimeToFire = Time.time + 1f / fireRate;
                ShootAtTarget();
            }

            if (enemyState == EnemyState.Chase)
            {
                ChaseTarget();

                // Check if the player is still within sight during the Chase state
                if (CanSeePlayer())
                {
                    // If the player is still within sight, switch to shooting
                    SwitchToShooting();
                }
                else
                {
                    // If the player is not within sight, continue chasing
                    // Calculate distance covered during chase
                    distanceCovered += navMeshAgent.velocity.magnitude * Time.deltaTime;

                    // Check if the NPC should return to patrol state
                    if (distanceCovered >= chaseDistanceThreshold)
                    {
                        distanceCovered = 0;
                        ReturnToPatrol();
                    }
                }
            }


        }

        if (detectPlayer == true && enemyState == EnemyState.SoundDetected)
        {
            // StartChasing();
            StartAlerting();
        }

        // Only execute chasing behavior if not in SoundDetected state
        if (enemyState != EnemyState.SoundDetected)
        {
            if (isChasing)
            {
                ChaseTarget();
                // Calculate distance covered during chase
                distanceCovered += navMeshAgent.velocity.magnitude * Time.deltaTime;
                // Check if the NPC should return to patrol state
                if (distanceCovered >= chaseDistanceThreshold)
                {
                    distanceCovered = 0;
                    ReturnToPatrol();
                }
            }
            else if (isFiring && Vector3.Distance(transform.position, target.position) > noiseResponseDistance)
            {
                isFiring = false;
                weapon.StopShooting();
                StartChasing();
            }
        }

        if (isDead == true)
        {
            Die();
        }

    }


    void ReturnToPatrol()
    {
        minimapIcon.color = aliveColor;
        weapon.StopShooting();

        Debug.Log("Returned Patrol by " + name);
        enemyState = EnemyState.Patrol;
        isChasing = false;


        alertIcon.SetActive(false);
        chaseTargetIcon.SetActive(false);

        navMeshAgent.speed = patrolSpeed;

        Debug.Log("Returning to patrol");

        // Calculate the index of the previous waypoint
        int previousWaypointIndex = currentWaypointIndex - 1;
        if (previousWaypointIndex < 0)
        {
            previousWaypointIndex = waypoints.Length - 1;
        }

        // Set the current waypoint to the previous waypoint
        currentWaypointIndex = previousWaypointIndex;
        SetNextWaypointIndex();
        PatrolToDestination();
    }

    void StartAlerting()
    {
        minimapIcon.color = alertColor;


        enemyState = EnemyState.Alert;
        isAlerting = true;
        animator.SetFloat(BLENDSTATE, 0f);
        navMeshAgent.isStopped = true;
        alertIcon.SetActive(true);
        Debug.Log("Alerting");
    }

    void StartChasing()
    {
        minimapIcon.color = alertColor;


        enemyState = EnemyState.Chase;
        isChasing = true;

        Debug.Log("Chasing");
    }

    void ChaseTarget()
    {
        minimapIcon.color = alertColor;

        navMeshAgent.SetDestination(target.position);
        navMeshAgent.isStopped = false;

        alertIcon.SetActive(false);
        chaseTargetIcon.SetActive(true);
        navMeshAgent.speed = chaseSpeed;
        animator.SetFloat(BLENDSTATE, 0.67f);
    }

    private void SelectedState()
    {
        if (enemyState == EnemyState.Idle)
        {
            animator.SetFloat(BLENDSTATE, 0f);
        }
        else
        {
            PatrolToDestination();
        }
    }

    void SetNextWaypointIndex()
    {
        currentWaypointIndex = Random.Range(0, waypoints.Length);
    }

    public void PatrolToDestination()
    {
        weapon.StopShooting();

        currentWaypoint = waypoints[currentWaypointIndex];
        navMeshAgent.SetDestination(currentWaypoint.transform.position);
        navMeshAgent.speed = patrolSpeed;
        animator.SetFloat(BLENDSTATE, .34f);

    }

    void FindUnoccupiedWaypoint()
    {
        // Create a list to store unoccupied waypoints
        List<int> unoccupiedIndices = new List<int>();

        // Iterate through all waypoints
        for (int i = 0; i < waypoints.Length; i++)
        {
            // Check if the current waypoint is unoccupied
            if (!waypoints[i].IsOccupied())
            {
                // If unoccupied, add its index to the list of unoccupied indices
                unoccupiedIndices.Add(i);
            }
        }

        // If there are unoccupied waypoints
        if (unoccupiedIndices.Count > 0)
        {
            // Choose a random unoccupied waypoint from the list
            int randomIndex = Random.Range(0, unoccupiedIndices.Count);
            int chosenIndex = unoccupiedIndices[randomIndex];

            // Release the current waypoint
            currentWaypoint.Release();

            // Set the current waypoint to the chosen unoccupied waypoint
            currentWaypointIndex = chosenIndex;
            PatrolToDestination();
        }
        else
        {
            // If no unoccupied waypoints are found, make the NPC idle for a while
            StartCoroutine(IdleForSomeTime());
        }
    }



    IEnumerator IdleForSomeTime()
    {
        // Set NPC state to idle
        isIdling = true;

        // Choose a random duration for idling (you can adjust this as needed)
        float idleDuration = Random.Range(5f, 10f); // Idle for 5 to 10 seconds

        // Wait for the specified idle duration
        yield return new WaitForSeconds(idleDuration);

        // Reset idling state
        isIdling = false;

        // Find another unoccupied waypoint after idling
        FindUnoccupiedWaypoint();
    }


    public void DetectedPlayer(bool _detectPlayer)
    {
        detectPlayer = _detectPlayer;
        if (detectPlayer)
        {
            Debug.Log(name + " Detected Player");

        }
        else
        {
            //Debug.Log(gameObject.name + " did not Detect Player");
        }
    }


    void ShootAtTarget()
    {
        enemyState = EnemyState.ShootAtSight;
        animator.SetFloat(BLENDSTATE, 1f);
        weapon.Shoot(target);
        weapon.AlignWithEnemy(target);
        if (target.GetComponent<PlayerHealth>().isDead == true)
        {
            target.gameObject.layer = LayerMask.NameToLayer("Default");
            weapon.StopShooting();
        }

    }

    bool CanSeePlayer()
    {
        // Perform a raycast to check if there are any obstacles between the NPC and the player
        RaycastHit hit;

        if (Physics.Raycast(transform.position, (target.position - transform.position).normalized, out hit, Mathf.Infinity))
        {
            // If the raycast hits the player, return true
            if (hit.transform.CompareTag("Player"))
            {
                return true;
            }
        }

        // If the raycast doesn't hit the player, return false
        return false;
    }

    void SwitchToShooting()
    {
        // Switch the NPC state to shooting
        enemyState = EnemyState.ShootAtSight;
        animator.SetFloat(BLENDSTATE, 1f);

        // Start shooting at the player
        weapon.Shoot(target);
        weapon.AlignWithEnemy(target);

        // Check if the player is dead to stop shooting
        if (target.GetComponent<PlayerHealth>().isDead)
        {
            target.gameObject.layer = LayerMask.NameToLayer("Default");
            weapon.StopShooting();
        }
        if (enemyState == EnemyState.Patrol)
        {
            weapon.StopShooting();
        }
    }

    // Implement RespondToSound method from IHear interface
    public void RespondToSound(Sound sound)
    {
        if (sound.soundType == Sound.SoundType.Interesting)
        {
            Debug.Log(name + " has detected sound");
            heardSomething = true;
            enemyState = EnemyState.SoundDetected;
            Debug.Log("Switched to Sound Detection state");

            //     // If the NPC can't see the player, move towards the sound position
            MoveToSound(sound.pos);
            // }
        }
    }

    private Coroutine moveToSoundCoroutine;

    private void AssignPatrolStateToOthers()
    {
        foreach (NPCController npc in allNPCs)
        {
            // Skip the NPC that reached the sound position
            if (npc == this)
                continue;

            // Assign patrol state to other NPCs
            npc.ReturnToPatrol();
        }
    }


    private void MoveToSound(Vector3 _pos)
    {
        if (isDead)
        {
            navMeshAgent.isStopped = false;
            alertIcon.SetActive(false);
            chaseTargetIcon.SetActive(false);
            navMeshAgent.SetDestination(_pos);
            navMeshAgent.speed = chaseSpeed;
            animator.SetFloat(BLENDSTATE, 0.67f);
        }
        else
        {
            minimapIcon.color = alertColor;
            navMeshAgent.isStopped = false;
            alertIcon.SetActive(true);
            chaseTargetIcon.SetActive(false);
            navMeshAgent.SetDestination(_pos);
            navMeshAgent.speed = chaseSpeed;
            animator.SetFloat(BLENDSTATE, 0.67f);
            StartCoroutine(MonitorSoundPosition(_pos));
        }
    }

    private IEnumerator MonitorSoundPosition(Vector3 soundPosition)
    {
        float startTime = Time.time;
        while (Vector3.Distance(transform.position, soundPosition) > navMeshAgent.stoppingDistance)
        {
            yield return null;

            // Check if it's taking too long to reach the sound position
            if (Time.time - startTime > 8f)
            {
                Debug.Log("Taking too long to reach sound position. Reverting back to patrol.");
                AssignPatrolStateToOthers();
                animator.SetFloat(BLENDSTATE, 0f); // Switch the animator state to idle
                moveToSoundCoroutine = null;
                alertIcon.SetActive(false);
                ReturnToPatrol();
                yield break; // Exit the coroutine
            }
        }

        // NPC has reached the sound position within the allowed time
        AssignPatrolStateToOthers();
        animator.SetFloat(BLENDSTATE, 0f); // Switch the animator state to idle
        moveToSoundCoroutine = null;

        // Wait for a while before returning to patrol
        yield return new WaitForSeconds(10f); // Adjust this time as needed
        alertIcon.SetActive(false);
        ReturnToPatrol();
    }


    public void _DisableScript()
    {
        StartCoroutine(DisableScript(.9f));
    }
    IEnumerator DisableScript(float t)
    {

        yield return new WaitForSeconds(t);
        this.enabled = false;
    }

    void Die()
    {
        GetComponent<Die>().enabled = true;

        this.tag = "Untagged";
        // Call Waypoint script to release if occupied
        if (currentWaypoint != null)
        {
            currentWaypoint.OnNPCDeath(this);
        }
    }


}