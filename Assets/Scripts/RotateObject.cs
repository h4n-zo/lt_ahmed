using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateObject : MonoBehaviour
{
     public float rotationSpeed = 100f; // Speed of rotation
    public float moveDistance = 1f;    // Distance to move up and down
    public float moveSpeed = 1f;       // Speed of the up and down movement

    private Vector3 startPosition;

    void Start()
    {
        // Store the starting position
        startPosition = transform.position;
    }

    // Update is called once per frame
    void Update()
    {
          transform.Rotate(Vector3.up, rotationSpeed * Time.deltaTime);

        float move = Mathf.PingPong(Time.time * moveSpeed, moveDistance) - (moveDistance / 2f);
        transform.position = startPosition + new Vector3(0, move, 0);
 
    }
}
