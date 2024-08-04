using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class test : MonoBehaviour, IHear
{

    public bool isDetected = false;
    [SerializeField] Sound sound;
    public NavMeshAgent navMeshAgent;


    private void Update()
    {
        if(isDetected == true)
        {
            MoveToSound(sound.pos);
        }
    }
    public void RespondToSound(Sound _sound)
    {
        if (sound.soundType == Sound.SoundType.Interesting)
        {
            Debug.Log(" has detected sound");
            sound = _sound;

            isDetected = true;
        }
    }

    private void MoveToSound(Vector3 _pos)
    {
        
        navMeshAgent.SetDestination(_pos);
        navMeshAgent.isStopped = false;

    }
}
