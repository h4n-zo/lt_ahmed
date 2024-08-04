using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class CameraTrigger : MonoBehaviour
{
    public UnityEvent trigger;
    public float timeToTrigger;

    private void Start()
    {
        StartCoroutine(TriggerCameraEvent());
    }
    IEnumerator TriggerCameraEvent()
    {
        yield return new WaitForSeconds(timeToTrigger);
        trigger?.Invoke();
    }
}
