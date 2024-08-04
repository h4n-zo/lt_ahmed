using System.Collections;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class PlayerScript : MonoBehaviour
{
    const string BLEND = "Action";
    const string STAB = "Stab";
    const string RUN = "Run";

    NavMeshAgent agent;
    [SerializeField] float stoppingDistance = 1.2f;
    private float defaultSpeed;
    private float angularSpeed;
    private float acceleration;

    Animator animator;

    [Header("Movement")]
    [SerializeField] ParticleSystem clickEffect;
    [SerializeField] LayerMask clickableLayers;
    [SerializeField] LayerMask enemyLayer;
    [SerializeField] GameObject linePrefab; // Reference to the sprite prefab
    [SerializeField] Material deathTrailMaterial;
    private Vector3 lastPosition;
    private GameObject lineRenderer; // Reference to the instantiated line sprite

    [SerializeField] float lookRotationSpeed = 8f;

    public float killDistance = 10f;
    public float distanceToKill;

    bool isFollowingEnemy = false;
    bool isStabbing = false; // Track if the player is currently stabbing
    bool isStabbed = false; // Track if the player has initiated the stab animation

    public float stabAnimationDuration = 2f;

    [SerializeField] private TriggerSound _npc;
    [SerializeField] AudioSource stabSfx;

    void Start()
    {
        Time.timeScale = 1;

        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
        defaultSpeed = agent.speed;
        angularSpeed = agent.angularSpeed;
        acceleration = agent.acceleration;

    }

    void Update()
    {
        // Check for left mouse button or touch input
        if (!isStabbing && !isStabbed && (Input.GetMouseButtonDown(0) || (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began)))
        {
            Vector2 inputPosition = Input.GetMouseButtonDown(0) ? Input.mousePosition : Input.GetTouch(0).position;
            ClickToMove(inputPosition);
        }

        FaceTarget();
        SetAnimation();

        // Update line renderer while player is moving
        if (agent.velocity != Vector3.zero)
            UpdateLineRenderer();

        // Manually update the agent's position using unscaled delta time
        float adjustedSpeed = defaultSpeed / Time.timeScale;
        agent.speed = adjustedSpeed;
        float adjustedAngularSpeed = angularSpeed / Time.timeScale;
        agent.angularSpeed = adjustedAngularSpeed;
        float adjustedAcceleration = acceleration / Time.timeScale;
        agent.acceleration = adjustedAcceleration;


    }

    void ClickToMove(Vector2 inputPosition)
    {
        // Check if the player is currently stabbing or has already initiated the stab animation, if so, return without executing click-to-move logic
        if (isStabbing || isStabbed)
            return;

        // Rest of the method remains the same
        RaycastHit hit;
        Ray ray = Camera.main.ScreenPointToRay(inputPosition);

        if (Physics.Raycast(ray, out hit, 100, clickableLayers))
        {
            if (hit.collider.CompareTag("Clickable"))
            {
                if (isFollowingEnemy)
                {
                    StopAllCoroutines();
                    isFollowingEnemy = false;
                    animator.SetFloat(BLEND, 0f);
                }

                agent.destination = hit.point;

                if (clickEffect != null)
                {
                    Instantiate(clickEffect, hit.point + new Vector3(0, .1f, 0), clickEffect.transform.rotation);
                    lastPosition = transform.position;
                }

                if (lineRenderer != null)
                    Destroy(lineRenderer);

                lineRenderer = Instantiate(linePrefab, transform.position, Quaternion.identity);

                NavMeshPath path = agent.path;
                lineRenderer.GetComponent<LineRenderer>().positionCount = path.corners.Length;
                for (int i = 0; i < path.corners.Length; i++)
                {
                    lineRenderer.GetComponent<LineRenderer>().SetPosition(i, path.corners[i]);
                }
            }
            else if (hit.collider.CompareTag("Enemy"))
            {
                StartCoroutine(FollowEnemy(hit));
                if (clickEffect != null)
                {
                    Instantiate(clickEffect, hit.point + new Vector3(0, .1f, 0), clickEffect.transform.rotation);
                    lastPosition = transform.position;
                }

                if (lineRenderer != null)
                    Destroy(lineRenderer);

                lineRenderer = Instantiate(linePrefab, transform.position, Quaternion.identity);
                lineRenderer.GetComponent<LineRenderer>().material = deathTrailMaterial;

                NavMeshPath path = agent.path;
                lineRenderer.GetComponent<LineRenderer>().positionCount = path.corners.Length;
                for (int i = 0; i < path.corners.Length; i++)
                {
                    lineRenderer.GetComponent<LineRenderer>().SetPosition(i, path.corners[i]);
                }
            }
        }
    }

    IEnumerator FollowEnemy(RaycastHit enemy)
    {
        isFollowingEnemy = true;

        while (true)
        {
            Vector3 targetPosition = enemy.transform.position - enemy.transform.forward * stoppingDistance;
            agent.SetDestination(targetPosition);

            float distanceToEnemy = Vector3.Distance(transform.position, targetPosition);
            if (distanceToEnemy <= stoppingDistance && enemy.collider != null)
            {
                Vector3 directionToEnemy = enemy.transform.position - transform.position;

                directionToEnemy.y = 0;

                Quaternion targetRot = Quaternion.LookRotation(directionToEnemy);
                transform.rotation = targetRot;
                
                AttackEnemy(distanceToEnemy, killDistance, enemy);
            }

            yield return null;
        }
    }

    private void AttackEnemy(float _distanceToEnemy, float _killDistance, RaycastHit target)
    {
        distanceToKill = _distanceToEnemy;

        if (_distanceToEnemy <= _killDistance)
        {
            Debug.Log("Player in enemy distance");

            // _npc = target.collider.GetComponent<TriggerSound>();

            target.collider.GetComponent<NavMeshAgent>().isStopped = false;
            target.collider.GetComponent<NavMeshAgent>().speed = 0;
            animator.SetBool(STAB, true);
            agent.speed = 0;
            isStabbing = true; // Set stabbing flag to true
            isStabbed = true; // Set stabbed flag to true when initiating stab animation

            StartCoroutine(StopStabAnimation(target));
        }
    }

    IEnumerator StopStabAnimation(RaycastHit _target)
    {
        yield return new WaitForSeconds(stabAnimationDuration);

        NPCController npc = _target.collider.GetComponent<NPCController>();
        // _npc = npc.GetComponent<TriggerSound>();

        npc.isDead = true;
        agent.speed = defaultSpeed;
        animator.SetBool(STAB, false);
        isFollowingEnemy = false;
        isStabbing = false; // Reset stabbing flag to false
        isStabbed = false; // Reset stabbed flag to false after stab animation completes

        npc.GetComponent<TriggerSound>().PlaySound();
        _target.collider.GetComponent<Animator>().Play("Death");
        _target.collider.GetComponent<NavMeshAgent>().enabled = false;
        npc.fovMesh.SetActive(false);
        npc._DisableScript();
        _target.collider.GetComponent<SphereCollider>().enabled = false;

        ScoreSystem scoreSystem = GameObject.FindObjectOfType<ScoreSystem>();
        if (scoreSystem != null)
        {
            scoreSystem.AddScore(1);
        }


        if (lineRenderer != null)
            Destroy(lineRenderer);
        StopAllCoroutines();
    }

    void UpdateLineRenderer()
    {
        if (lineRenderer != null)
        {
            NavMeshPath path = agent.path;
            lineRenderer.GetComponent<LineRenderer>().positionCount = path.corners.Length;
            for (int i = 0; i < path.corners.Length; i++)
            {
                lineRenderer.GetComponent<LineRenderer>().SetPosition(i, path.corners[i]);
            }
        }
        else
        {
            return;
        }
    }

    private void FaceTarget()
    {
        if (agent.velocity != Vector3.zero)
        {
            Vector3 dir = (agent.steeringTarget - transform.position).normalized;
            Quaternion lookRotation = Quaternion.LookRotation(new Vector3(dir.x, 0, dir.z));
            transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * lookRotationSpeed);
        }
    }

    private void SetAnimation()
    {
        if (agent.velocity == Vector3.zero)
        {
            animator.SetBool(RUN, false);
        }
        else
        {
            animator.SetBool(RUN, true);
        }
    }


    //== Animation Event==//
    void Stab()
    {
        Debug.Log("Stab() is carried out");
        stabSfx.Play();

    }
}
