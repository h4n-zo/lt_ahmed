using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Billboard : MonoBehaviour
{
    [SerializeField] private Camera targetCamera;
    [SerializeField] private bool faceCamera = true;
    [SerializeField] private bool useWorldSpace = true;

    private void Start()
    {
        // If no camera is assigned, use the main camera
        if (targetCamera == null)
        {
            targetCamera = Camera.main;
        }
    }

    private void LateUpdate()
    {
        if (faceCamera && targetCamera != null)
        {
            LookAtCamera();
        }
    }

    private void LookAtCamera()
    {
        if (useWorldSpace)
        {
            // Make the object face the camera in world space
            transform.LookAt(transform.position + targetCamera.transform.rotation * Vector3.forward,
                             targetCamera.transform.rotation * Vector3.up);
        }
        else
        {
            // Make the object face the camera in local space (useful for UI elements in world space)
            transform.rotation = targetCamera.transform.rotation;
        }
    }

    // Public method to toggle camera facing
    public void ToggleFaceCamera(bool enable)
    {
        faceCamera = enable;
    }

    // Public method to change the target camera
    public void SetTargetCamera(Camera newCamera)
    {
        targetCamera = newCamera;
    }
}