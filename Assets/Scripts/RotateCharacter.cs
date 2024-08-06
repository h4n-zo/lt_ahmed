using UnityEngine;

public class RotateCharacter : MonoBehaviour
{
    // Rotation speeds for each axis
    public float rotationSpeedX = 10f;
    public float rotationSpeedY = 10f;
    public float rotationSpeedZ = 10f;

    void Update()
    {
        // Calculate rotation for each frame
        float rotationX = rotationSpeedX * Time.deltaTime;
        float rotationY = rotationSpeedY * Time.deltaTime;
        float rotationZ = rotationSpeedZ * Time.deltaTime;

        // Apply rotation to the object
        transform.Rotate(rotationX, rotationY, rotationZ);
    }
}