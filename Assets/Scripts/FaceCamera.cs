using UnityEngine;

public class FaceCamera : MonoBehaviour
{
    private Camera mainCamera;

    void Start()
    {
        // Find the main camera in the scene
        mainCamera = Camera.main;
    }

    void Update()
    {
        // Ensure the main camera is not null
        if (mainCamera != null)
        {
            // Calculate the direction from the canvas sprite to the camera
            Vector3 lookDir = mainCamera.transform.position - transform.position;
            // Make the canvas sprite face the camera
            // transform.rotation = Quaternion.LookRotation(lookDir, Vector3.up);
            transform.rotation =Quaternion.LookRotation(lookDir, -Vector3.up);
        }
    }
}
