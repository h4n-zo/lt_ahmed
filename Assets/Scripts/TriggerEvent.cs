using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class TriggerEvent : MonoBehaviour
{
    public UnityEvent stayTrigger;
    public UnityEvent exitTrigger;

    public UnityEvent enterTrigger;

    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            stayTrigger?.Invoke();
        }

    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            exitTrigger?.Invoke();
        }

    }


    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
             enterTrigger?.Invoke();
        }
    }
}
