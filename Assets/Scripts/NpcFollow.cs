using System.Collections;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class NpcFollow : MonoBehaviour
{
    public Transform player; // Reference to the player's transform.
    public GameObject sleepAura;
    public float followDistance = 3.0f;
    public float initHostageTime = 4f;
    // Distance threshold to start following.
    public Animator animator;
    public BoxCollider hostageBoxCol;
    public bool isFree = false;

    private NavMeshAgent navMeshAgent;

    void Start()
    {
        navMeshAgent = GetComponent<NavMeshAgent>();
    }

    void Update()
    {
        if (isFree == true)
        {
            float distance = Vector3.Distance(player.position, transform.position);

            if (distance > followDistance)
            {
                navMeshAgent.SetDestination(player.position);
                animator.SetBool("Walk", true);
                animator.SetBool("StandUp", false);
            }
            else
            {
                animator.SetBool("Walk", !true);
                navMeshAgent.ResetPath();
            }
        }

    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            StartCoroutine(InitHostage(initHostageTime, other));
        }
    }

    IEnumerator InitHostage(float t, Collider target)
    {
        animator.SetBool("StandUp", true);
        sleepAura.SetActive(false);
        hostageBoxCol.enabled = false;

        target.GetComponent<NavMeshAgent>().enabled = false;
        target.GetComponent<PlayerScript>().enabled = false;
        target.GetComponent<Animator>().SetBool("Run", false);

        yield return new WaitForSeconds(t);

        animator.SetBool("StandUp", false);
        isFree = true;
        target.GetComponent<NavMeshAgent>().enabled = true;
        target.GetComponent<PlayerScript>().enabled = true;


    }

}
