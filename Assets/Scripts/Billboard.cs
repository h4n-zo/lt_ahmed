using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Billboard : MonoBehaviour
{
    Camera cam;
    public float rotateValue = 180f;

    // Update is called once per frame
    void Update()
    {
        if(cam == null)
            cam = Camera.main;

        if(cam == null)
            return;

        transform.LookAt(cam.transform);
        transform.Rotate(Vector3.up * rotateValue);
        
    }
}