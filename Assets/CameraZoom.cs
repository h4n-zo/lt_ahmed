using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using TMPro;
using UnityEngine.AI;

public class CameraZoom : MonoBehaviour
{
    public CinemachineVirtualCamera cam;
    public float initialZoom;
    public float secondZoom = 82f;
    public TextMeshProUGUI zoomValue;

    private int zoomState = 0; // Tracks current zoom state

    // Start is called before the first frame update
    void Start()
    {
        initialZoom = cam.m_Lens.FieldOfView;
        zoomValue.text = "X1";
    }

    // Update is called once per frame
    public void ExecuteZoom()
    {
        Debug.Log("Zoom button is pressed");
        // transform.Translate(Vector3.zero);
        GetComponent<NavMeshAgent>().isStopped = true;
        GameObject lr = GetComponent<PlayerScript>().lineRenderer;

        if (lr != null)
            Destroy(lr);

        // Check the current zoom state and update accordingly
        switch (zoomState)
        {
            case 0:
                cam.m_Lens.FieldOfView = secondZoom;
                zoomState = 1;
                zoomValue.text = "x2";
                break;

            case 1:
                cam.m_Lens.FieldOfView = initialZoom;
                zoomState = 0;
                zoomValue.text = "x1";
                break;
        }
    }
}